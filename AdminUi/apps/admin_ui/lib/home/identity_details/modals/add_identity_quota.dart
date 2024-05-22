import 'dart:async';

import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:get_it/get_it.dart';

import '/core/core.dart';

Future<void> showAddIdentityQuotaDialog({required BuildContext context, required String address, required VoidCallback onQuotaAdded}) async {
  await showDialog<void>(
    context: context,
    builder: (BuildContext context) => _AddIdentityQuotaDialog(
      address: address,
      onQuotaAdded: onQuotaAdded,
    ),
  );
}

class _AddIdentityQuotaDialog extends StatefulWidget {
  final String address;
  final VoidCallback onQuotaAdded;

  const _AddIdentityQuotaDialog({required this.address, required this.onQuotaAdded});

  @override
  State<_AddIdentityQuotaDialog> createState() => _AddIdentityQuotaDialogState();
}

class _AddIdentityQuotaDialogState extends State<_AddIdentityQuotaDialog> {
  final _maxAmountController = TextEditingController();
  List<Metric> _availableMetrics = [];

  bool _saving = false;
  String? _errorMessage;

  String? _selectedMetric;
  int? _maxAmount;
  String? _selectedPeriod;
  bool get _isValid => _selectedMetric != null && _maxAmount != null && _selectedPeriod != null;

  @override
  void initState() {
    super.initState();

    _maxAmountController.addListener(() => setState(() => _maxAmount = int.tryParse(_maxAmountController.text)));

    _loadMetrics();
  }

  @override
  void dispose() {
    _maxAmountController.dispose();

    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return PopScope(
      canPop: !_saving,
      child: AlertDialog(
        title: const Text('Add Quota'),
        content: SizedBox(
          width: 500,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              DropdownButtonFormField(
                items: _availableMetrics.map((metric) => DropdownMenuItem(value: metric.key, child: Text(metric.displayName))).toList(),
                onChanged: _saving ? null : (String? selected) => setState(() => _selectedMetric = selected),
                decoration: const InputDecoration(
                  border: OutlineInputBorder(),
                  labelText: 'Metric*',
                ),
              ),
              Gaps.h24,
              TextField(
                controller: _maxAmountController,
                enabled: !_saving,
                decoration: const InputDecoration(
                  border: OutlineInputBorder(),
                  labelText: 'Max Amount*',
                  helperText: 'Only numbers greater or equal to 0 are valid.',
                ),
                inputFormatters: <TextInputFormatter>[FilteringTextInputFormatter.digitsOnly],
                keyboardType: TextInputType.number,
              ),
              Gaps.h24,
              DropdownButtonFormField(
                items: const [
                  DropdownMenuItem(value: 'Hour', child: Text('Hour')),
                  DropdownMenuItem(value: 'Day', child: Text('Day')),
                  DropdownMenuItem(value: 'Week', child: Text('Week')),
                  DropdownMenuItem(value: 'Month', child: Text('Month')),
                  DropdownMenuItem(value: 'Year', child: Text('Year')),
                ],
                onChanged: _saving ? null : (String? selected) => setState(() => _selectedPeriod = selected),
                decoration: const InputDecoration(
                  border: OutlineInputBorder(),
                  labelText: 'Period*',
                ),
              ),
              if (_errorMessage != null)
                Padding(
                  padding: const EdgeInsets.only(top: 24),
                  child: Text(
                    _errorMessage!,
                    style: TextStyle(color: Theme.of(context).colorScheme.error),
                  ),
                ),
            ],
          ),
        ),
        actions: [
          OutlinedButton(
            onPressed: _saving ? null : () => Navigator.of(context, rootNavigator: true).pop(),
            child: Text(context.l10n.cancel),
          ),
          FilledButton(
            onPressed: _isValid && !_saving ? _addQuota : null,
            child: const Text('Add'),
          ),
        ],
      ),
    );
  }

  Future<void> _addQuota() async {
    setState(() => _saving = true);

    final response = await GetIt.I.get<AdminApiClient>().quotas.createIdentityQuota(
          address: widget.address,
          metricKey: _selectedMetric!,
          max: _maxAmount!,
          period: _selectedPeriod!,
        );

    if (response.hasError) {
      setState(() {
        _errorMessage = response.error.message;
        _saving = false;
      });

      return;
    }

    if (mounted) Navigator.of(context, rootNavigator: true).pop();
    widget.onQuotaAdded();
  }

  Future<void> _loadMetrics() async {
    final metrics = await GetIt.I.get<AdminApiClient>().quotas.getMetrics();
    setState(() => _availableMetrics = metrics.data);
  }
}

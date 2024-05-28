import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:get_it/get_it.dart';

import '../constants.dart';
import '../extensions.dart';

Future<void> showAddQuotaDialog({
  required BuildContext context,
  required VoidCallback onQuotaAdded,
  String? tierId,
  String? identityAddress,
}) async {
  assert(tierId != null || identityAddress != null, 'Either tierId or address must be provided');
  assert(tierId == null || identityAddress == null, 'Only one of tierId or address can be provided');

  final metrics = await GetIt.I.get<AdminApiClient>().quotas.getMetrics();

  if (!context.mounted) return;

  await showDialog<void>(
    context: context,
    builder: (BuildContext context) => _AddQuotaDialog(
      availableMetrics: metrics.data,
      addQuota: ({required String metricKey, required int max, required String period}) {
        if (tierId != null) {
          return GetIt.I.get<AdminApiClient>().quotas.createTierQuota(tierId: tierId, metricKey: metricKey, max: max, period: period);
        }

        return GetIt.I.get<AdminApiClient>().quotas.createIdentityQuota(address: identityAddress!, metricKey: metricKey, max: max, period: period);
      },
      onQuotaAdded: onQuotaAdded,
    ),
  );
}

class _AddQuotaDialog extends StatefulWidget {
  final List<Metric> availableMetrics;
  final Future<ApiResponse<dynamic>> Function({required String metricKey, required int max, required String period}) addQuota;
  final VoidCallback onQuotaAdded;

  const _AddQuotaDialog({
    required this.availableMetrics,
    required this.addQuota,
    required this.onQuotaAdded,
  });

  @override
  State<_AddQuotaDialog> createState() => _AddQuotaDialogState();
}

class _AddQuotaDialogState extends State<_AddQuotaDialog> {
  final _maxAmountController = TextEditingController();

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
                value: _selectedMetric,
                items: widget.availableMetrics.map((metric) => DropdownMenuItem(value: metric.key, child: Text(metric.displayName))).toList(),
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
                value: _selectedPeriod,
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

    assert(_selectedMetric != null && _maxAmount != null && _selectedPeriod != null, 'Invalid state');

    final response = await widget.addQuota(
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
}

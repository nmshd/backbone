import 'dart:async';

import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:admin_ui/core/modals/add_quota_dialog.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';

Future<void> showAddQuotaDialog({required BuildContext context, required String tierId, required VoidCallback onQuotaAdded}) async {
  await showDialog<void>(
    context: context,
    builder: (BuildContext context) => _AddTierQuotaDialog(
      tierId: tierId,
      onQuotaAdded: onQuotaAdded,
    ),
  );
}

class _AddTierQuotaDialog extends StatefulWidget {
  final String tierId;
  final VoidCallback onQuotaAdded;

  const _AddTierQuotaDialog({required this.tierId, required this.onQuotaAdded});

  @override
  State<_AddTierQuotaDialog> createState() => _AddTierQuotaDialogState();
}

class _AddTierQuotaDialogState extends State<_AddTierQuotaDialog> {
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
    return AddQuotaDialog(
      saving: _saving,
      errorMessage: _errorMessage,
      maxAmountController: _maxAmountController,
      availableMetrics: _availableMetrics,
      isValid: _isValid,
      addQuota: _addQuota,
      selectedMetric: _updateSelectedMetric,
      selectedPeriod: _updateSelectedPeriod,
    );
  }

  void _updateSelectedMetric(String? metric) {
    setState(() {
      _selectedMetric = metric;
    });
  }

  void _updateSelectedPeriod(String? period) {
    setState(() {
      _selectedPeriod = period;
    });
  }

  Future<void> _addQuota() async {
    setState(() => _saving = true);

    final response = await GetIt.I.get<AdminApiClient>().quotas.createTierQuota(
          tierId: widget.tierId,
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

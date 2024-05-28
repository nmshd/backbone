import 'package:admin_api_types/admin_api_types.dart';
import 'package:admin_ui/core/constants.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

class AddQuotaDialog extends StatefulWidget {
  final bool saving;
  final String? errorMessage;
  final TextEditingController maxAmountController;
  final List<Metric> availableMetrics;
  final bool isValid;
  final VoidCallback addQuota;
  final ValueChanged<String?> selectedMetric;
  final ValueChanged<String?> selectedPeriod;

  const AddQuotaDialog({
    required this.saving,
    required this.errorMessage,
    required this.maxAmountController,
    required this.availableMetrics,
    required this.isValid,
    required this.addQuota,
    required this.selectedMetric,
    required this.selectedPeriod,
    super.key,
  });

  @override
  State<AddQuotaDialog> createState() => _AddQuotaDialogState();
}

class _AddQuotaDialogState extends State<AddQuotaDialog> {
  String? selectedMetric;
  String? selectedPeriod;

  @override
  Widget build(BuildContext context) {
    return PopScope(
      canPop: !widget.saving,
      child: AlertDialog(
        title: const Text('Add Quota'),
        content: SizedBox(
          width: 500,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              DropdownButtonFormField(
                value: selectedMetric,
                items: widget.availableMetrics.map((metric) => DropdownMenuItem(value: metric.key, child: Text(metric.displayName))).toList(),
                onChanged: widget.saving ? null : widget.selectedMetric,
                decoration: const InputDecoration(
                  border: OutlineInputBorder(),
                  labelText: 'Metric*',
                ),
              ),
              Gaps.h24,
              TextField(
                controller: widget.maxAmountController,
                enabled: !widget.saving,
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
                value: selectedPeriod,
                items: const [
                  DropdownMenuItem(value: 'Hour', child: Text('Hour')),
                  DropdownMenuItem(value: 'Day', child: Text('Day')),
                  DropdownMenuItem(value: 'Week', child: Text('Week')),
                  DropdownMenuItem(value: 'Month', child: Text('Month')),
                  DropdownMenuItem(value: 'Year', child: Text('Year')),
                ],
                onChanged: widget.saving ? null : widget.selectedPeriod,
                decoration: const InputDecoration(
                  border: OutlineInputBorder(),
                  labelText: 'Period*',
                ),
              ),
              if (widget.errorMessage != null)
                Padding(
                  padding: const EdgeInsets.only(top: 24),
                  child: Text(
                    widget.errorMessage!,
                    style: TextStyle(color: Theme.of(context).colorScheme.error),
                  ),
                ),
            ],
          ),
        ),
        actions: [
          OutlinedButton(
            onPressed: widget.saving ? null : () => Navigator.of(context, rootNavigator: true).pop(),
            child: Text(MaterialLocalizations.of(context).cancelButtonLabel),
          ),
          ElevatedButton(
            onPressed: widget.isValid && !widget.saving ? widget.addQuota : null,
            child: const Text('Add'),
          ),
        ],
      ),
    );
  }
}

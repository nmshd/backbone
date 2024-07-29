import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';

import '/core/core.dart';

Future<void> showChangeMaxIdentitiesDialog({
  required BuildContext context,
  required VoidCallback onMaxIdentitiesUpdated,
  required Client clientDetails,
  required int numberOfIdentities,
}) async {
  await showDialog<void>(
    context: context,
    builder: (BuildContext context) => _ShowChangeMaxIdentitiesDialog(
      onMaxIdentitiesUpdated: onMaxIdentitiesUpdated,
      clientDetails: clientDetails,
      numberOfIdentities: numberOfIdentities,
      updateMaxIdentities: ({required int maxIdentities}) {
        return GetIt.I.get<AdminApiClient>().clients.updateClient(
              clientDetails.clientId,
              defaultTier: clientDetails.defaultTier,
              maxIdentities: maxIdentities,
            );
      },
    ),
  );
}

class _ShowChangeMaxIdentitiesDialog extends StatefulWidget {
  final VoidCallback onMaxIdentitiesUpdated;
  final Future<ApiResponse<dynamic>> Function({required int maxIdentities}) updateMaxIdentities;
  final Client clientDetails;
  final int numberOfIdentities;

  const _ShowChangeMaxIdentitiesDialog({
    required this.onMaxIdentitiesUpdated,
    required this.updateMaxIdentities,
    required this.clientDetails,
    required this.numberOfIdentities,
  });

  @override
  State<_ShowChangeMaxIdentitiesDialog> createState() => _ShowChangeMaxIdentitiesDialogState();
}

class _ShowChangeMaxIdentitiesDialogState extends State<_ShowChangeMaxIdentitiesDialog> {
  bool _saving = false;
  int? _maxIdentities = 0;
  late TextEditingController textController;
  String? _errorMessage;

  @override
  void initState() {
    super.initState();
    textController = TextEditingController();
  }

  @override
  Widget build(BuildContext context) {
    return PopScope(
      canPop: !_saving,
      child: AlertDialog(
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
        title: Text(context.l10n.maxIdentities_title, textAlign: TextAlign.center),
        contentPadding: const EdgeInsets.only(left: 24, right: 24, top: 20, bottom: 32),
        content: SizedBox(
          width: 500,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              TextFormField(
                controller: textController,
                decoration: InputDecoration(
                  hintText: context.l10n.maxIdentities_inputHint,
                  labelText: context.l10n.maxIdentities,
                  errorText: _errorMessage,
                  alignLabelWithHint: true,
                  border: const OutlineInputBorder(),
                ),
                keyboardType: TextInputType.number,
                onChanged: _saving ? null : (String? newValue) => setState(() => _maxIdentities = int.tryParse(newValue ?? '0')),
              ),
            ],
          ),
        ),
        actions: [
          OutlinedButton(
            onPressed: _saving ? null : () => Navigator.of(context, rootNavigator: true).pop(),
            child: Text(context.l10n.cancel),
          ),
          FilledButton(onPressed: _saving ? null : _changeMaxIdentities, child: Text(context.l10n.update)),
        ],
      ),
    );
  }

  Future<void> _changeMaxIdentities() async {
    setState(() {
      _saving = true;
      _errorMessage = null;
    });

    assert(_maxIdentities != null, 'Invalid State');

    if (_maxIdentities != null && _maxIdentities! <= widget.numberOfIdentities) {
      setState(() {
        _saving = false;
        _errorMessage = context.l10n.maxIdentities_error_message_maxIdentitiesLowerThanNumberOfIdentities;
      });
      return;
    }

    final response = await widget.updateMaxIdentities(maxIdentities: _maxIdentities!);

    if (!mounted) return;

    if (response.hasError) {
      setState(() {
        _saving = false;
        _errorMessage = context.l10n.maxIdentities_error_message;
      });
      return;
    }

    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(context.l10n.maxIdentities_success_message),
        duration: const Duration(seconds: 3),
      ),
    );

    widget.onMaxIdentitiesUpdated();

    Navigator.of(context, rootNavigator: true).pop();
  }
}

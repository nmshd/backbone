import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:get_it/get_it.dart';
import 'package:go_router/go_router.dart';

import '/core/core.dart';

Future<void> showChangeClientSecretDialog({
  required BuildContext context,
  required String clientId,
}) async {
  await showDialog<void>(
    context: context,
    builder: (BuildContext context) => _ChangeClientSecretDialog(clientId: clientId),
  );
}

class _ChangeClientSecretDialog extends StatefulWidget {
  final String clientId;

  const _ChangeClientSecretDialog({required this.clientId});

  @override
  State<_ChangeClientSecretDialog> createState() => _ChangeClientSecretDialogState();
}

class _ChangeClientSecretDialogState extends State<_ChangeClientSecretDialog> {
  final _newClientSecretController = TextEditingController();
  final _focusNode = FocusNode();

  bool _isClientSecretVisible = true;
  bool _saving = false;
  bool _saveSucceeded = false;

  String? _errorMessage;

  @override
  void initState() {
    super.initState();

    _newClientSecretController.addListener(() => setState(() {}));
    _focusNode.requestFocus();
  }

  @override
  void dispose() {
    _newClientSecretController.dispose();
    _focusNode.dispose();

    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return PopScope(
      canPop: !_saving,
      child: AlertDialog(
        title: const Text('Change Client Secret', textAlign: TextAlign.center),
        content: SizedBox(
          width: 500,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              TextField(
                controller: _newClientSecretController,
                focusNode: _focusNode,
                readOnly: _saveSucceeded,
                obscureText: _isClientSecretVisible,
                decoration: InputDecoration(
                  border: const OutlineInputBorder(),
                  labelText: 'Client Secret',
                  helperText: 'A Client Secret will be generated if this field is left blank.',
                  suffixIcon: Padding(
                    padding: const EdgeInsets.symmetric(horizontal: 8),
                    child: Row(
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        IconButton(
                          icon: Icon(_isClientSecretVisible ? Icons.visibility_off : Icons.visibility),
                          onPressed: () => setState(() => _isClientSecretVisible = !_isClientSecretVisible),
                        ),
                        Gaps.w4,
                        IconButton(
                          icon: const Icon(Icons.copy),
                          tooltip: 'Copy to clipboard.',
                          onPressed: _newClientSecretController.text.isNotEmpty
                              ? () => Clipboard.setData(ClipboardData(text: _newClientSecretController.text))
                              : null,
                        ),
                      ],
                    ),
                  ),
                ),
              ),
              if (_saveSucceeded)
                Padding(
                  padding: const EdgeInsets.symmetric(vertical: 8),
                  child: Text(
                    'Please save the Client Secret since it will be inaccessible after exiting.',
                    style: TextStyle(color: Theme.of(context).colorScheme.primary),
                  ),
                ),
              if (_errorMessage != null)
                Padding(
                  padding: const EdgeInsets.symmetric(vertical: 8),
                  child: Text(
                    _errorMessage!,
                    style: TextStyle(color: Theme.of(context).colorScheme.error),
                  ),
                ),
            ],
          ),
        ),
        actions: [
          OutlinedButton(onPressed: _saving ? null : () => context.pop(), child: Text(_saveSucceeded ? 'Close' : 'Cancel')),
          if (!_saveSucceeded) FilledButton(onPressed: _saving ? null : _changeClientSecret, child: const Text('Save')),
        ],
      ),
    );
  }

  Future<void> _changeClientSecret() async {
    setState(() => _saving = true);

    final response = await GetIt.I.get<AdminApiClient>().clients.changeClientSecret(widget.clientId, newSecret: _newClientSecretController.text);
    if (response.hasError) {
      setState(() {
        _errorMessage = response.error.message;
        _saving = false;
      });

      return;
    }

    _newClientSecretController.text = response.data.clientSecret;
    setState(() {
      _saveSucceeded = true;
      _saving = false;
    });
  }
}

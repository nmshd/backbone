import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:get_it/get_it.dart';
import 'package:go_router/go_router.dart';

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
  final TextEditingController _newClientSecretController = TextEditingController();

  bool _isClientSecretVisible = true;
  bool _saving = false;
  bool _saveFinished = false;

  String _errorMessage = '';
  String _saveClientSecretMessage = '';

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      content: SizedBox(
        width: 500,
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            const Text(
              'Change Client Secret',
              style: TextStyle(fontSize: 22, fontWeight: FontWeight.bold),
            ),
            Padding(
              padding: const EdgeInsets.all(8),
              child: Row(
                children: [
                  SizedBox(
                    width: 385,
                    child: TextField(
                      controller: _newClientSecretController,
                      readOnly: _saveFinished,
                      obscureText: _isClientSecretVisible,
                      decoration: const InputDecoration(
                        labelText: 'Client Secret',
                        helperText: 'A Client Secret will be generated if this field is left blank.',
                      ),
                    ),
                  ),
                  const Spacer(),
                  IconButton(
                    icon: Icon(_isClientSecretVisible ? Icons.visibility_off : Icons.visibility),
                    onPressed: () => setState(() => _isClientSecretVisible = !_isClientSecretVisible),
                  ),
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
            if (_saveClientSecretMessage.isNotEmpty)
              Padding(
                padding: const EdgeInsets.symmetric(vertical: 8),
                child: Text(
                  _saveClientSecretMessage,
                  style: TextStyle(color: Theme.of(context).colorScheme.primary),
                ),
              ),
            if (_errorMessage.isNotEmpty)
              Padding(
                padding: const EdgeInsets.symmetric(vertical: 8),
                child: Text(
                  _errorMessage,
                  style: TextStyle(color: Theme.of(context).colorScheme.error),
                ),
              ),
          ],
        ),
      ),
      actions: [
        OutlinedButton(onPressed: _saving ? null : () => context.pop(), child: Text(_saveFinished ? 'Cancel' : 'Close')),
        if (_saveFinished) FilledButton(onPressed: _saving ? null : _changeClientSecret, child: const Text('Save')),
      ],
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
      _saveClientSecretMessage = 'Please save the Client Secret since it will be inaccessible after exiting.';
      _saveFinished = false;
      _saving = false;
    });
  }
}

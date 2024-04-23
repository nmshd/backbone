import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_ui/core/core.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:get_it/get_it.dart';

class ChangeClientSecretDialog extends StatefulWidget {
  final String clientId;
  final void Function() loadClients;

  const ChangeClientSecretDialog({required this.clientId, required this.loadClients, super.key});

  @override
  State<ChangeClientSecretDialog> createState() => _ChangeClientSecretDialogState();
}

class _ChangeClientSecretDialogState extends State<ChangeClientSecretDialog> {
  final TextEditingController _newClientSecretController = TextEditingController();
  bool _isClientSecretVisible = true;
  bool _isEnabled = true;

  String _errorMessage = '';
  String _saveClientSecretMessage = '';

  @override
  Widget build(BuildContext context) {
    return Dialog(
      child: SizedBox(
        width: 500,
        height: 220,
        child: Column(
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
                      enabled: _isEnabled,
                      onChanged: (value) {
                        setState(() {
                          _newClientSecretController.text = value;
                        });
                      },
                      obscureText: _isClientSecretVisible,
                      decoration: const InputDecoration(
                        labelText: 'Client Secret',
                        helperText: 'A Client Secret will be generated if this field is left blank.',
                      ),
                    ),
                  ),
                  IconButton(
                    icon: Icon(_isClientSecretVisible ? Icons.visibility_off : Icons.visibility),
                    onPressed: () {
                      setState(() {
                        _isClientSecretVisible = !_isClientSecretVisible;
                      });
                    },
                  ),
                  IconButton(
                    icon: const Icon(Icons.copy),
                    tooltip: 'Copy to clipboard.',
                    onPressed: _newClientSecretController.text.isNotEmpty
                        ? () {
                            Clipboard.setData(ClipboardData(text: _newClientSecretController.text));
                          }
                        : null,
                  ),
                ],
              ),
            ),
            Flexible(
              child: Padding(
                padding: const EdgeInsets.symmetric(vertical: 2),
                child: Column(
                  children: [
                    if (_saveClientSecretMessage.isNotEmpty)
                      Text(
                        _saveClientSecretMessage,
                        style: TextStyle(color: Theme.of(context).colorScheme.primary),
                      ),
                    if (_errorMessage.isNotEmpty)
                      Text(
                        _errorMessage,
                        style: TextStyle(color: Theme.of(context).colorScheme.error),
                      ),
                  ],
                ),
              ),
            ),
            const Spacer(),
            Padding(
              padding: const EdgeInsets.only(left: 8, right: 8, top: 4, bottom: 8),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.end,
                children: [
                  ElevatedButton(
                    onPressed: _changeClientSecret,
                    child: const Text('Save'),
                  ),
                  Gaps.w8,
                  ElevatedButton(
                    onPressed: () {
                      _clearField();
                      Navigator.of(context).pop();
                    },
                    child: const Text('Cancel'),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  Future<void> _changeClientSecret() async {
    final response = await GetIt.I.get<AdminApiClient>().clients.changeClientSecret(widget.clientId, newSecret: _newClientSecretController.text);
    if (response.hasError) {
      setState(() {
        _errorMessage = response.error.message;
      });
    } else {
      setState(() {
        if (_newClientSecretController.text.isEmpty) {
          _newClientSecretController.text = response.data.clientSecret;
        }

        widget.loadClients();
        _saveClientSecretMessage = 'Please save the Client Secret since it will be inaccessible after exiting.';
        _isEnabled = false;
      });
    }
  }

  void _clearField() {
    _newClientSecretController.clear();
    _saveClientSecretMessage = '';
    _isEnabled = true;
  }
}

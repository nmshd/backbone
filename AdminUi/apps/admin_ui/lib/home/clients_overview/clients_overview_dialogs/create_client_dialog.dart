import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:admin_ui/core/constants.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:get_it/get_it.dart';

class CreateClientDialog extends StatefulWidget {
  final List<Tiers> defaultTiers;
  final void Function() loadClients;

  const CreateClientDialog({required this.defaultTiers, required this.loadClients, super.key});

  @override
  State<CreateClientDialog> createState() => _CreateClientDialogState();
}

class _CreateClientDialogState extends State<CreateClientDialog> {
  final TextEditingController _clientIdController = TextEditingController();
  final TextEditingController _displayNameController = TextEditingController();
  final TextEditingController _clientSecretController = TextEditingController();
  final TextEditingController _maxIdentitiesController = TextEditingController();

  String _chosenDefaultTier = '';
  String _errorMessage = '';
  String _saveClientSecretMessage = '';

  bool _isEnabled = true;
  bool _isPasswordVisible = true;

  @override
  Widget build(BuildContext context) {
    return Dialog(
      child: Container(
        padding: const EdgeInsets.all(16),
        width: 500,
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            const Text(
              'Create Client',
              style: TextStyle(fontSize: 22, fontWeight: FontWeight.bold),
            ),
            Gaps.h8,
            TextField(
              controller: _clientIdController,
              enabled: _isEnabled,
              decoration: const InputDecoration(
                labelText: 'Client ID',
                helperText: 'A Client ID will be generated if this field is left blank.',
              ),
            ),
            Gaps.h8,
            TextField(
              controller: _displayNameController,
              enabled: _isEnabled,
              decoration: const InputDecoration(
                labelText: 'Display Name',
                helperText: 'Client ID will be used as a Display Name if no value is provided.',
              ),
            ),
            Gaps.h8,
            Row(
              mainAxisSize: MainAxisSize.min,
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                SizedBox(
                  width: 385,
                  child: TextField(
                    controller: _clientSecretController,
                    enabled: _isEnabled,
                    obscureText: _isPasswordVisible,
                    decoration: const InputDecoration(
                      labelText: 'Client Secret',
                      helperText: 'A Client Secret will be generated if this field is left blank.',
                    ),
                  ),
                ),
                IconButton(
                  icon: Icon(
                    _isPasswordVisible ? Icons.visibility_off : Icons.visibility,
                  ),
                  onPressed: () {
                    setState(() {
                      _isPasswordVisible = !_isPasswordVisible;
                    });
                  },
                ),
                IconButton(
                  icon: const Icon(Icons.copy),
                  tooltip: 'Copy to clipboard.',
                  onPressed: _clientSecretController.text.isNotEmpty
                      ? () {
                          Clipboard.setData(ClipboardData(text: _clientSecretController.text));
                        }
                      : null,
                ),
              ],
            ),
            Gaps.h8,
            if (_saveClientSecretMessage.isNotEmpty)
              Text(
                _saveClientSecretMessage,
                style: TextStyle(color: Theme.of(context).colorScheme.primary),
              ),
            Gaps.h8,
            TextField(
              controller: _maxIdentitiesController,
              enabled: _isEnabled,
              decoration: const InputDecoration(
                labelText: 'Max Identities',
                helperText: 'The maximum number of Identities that can be created with this Client.'
                    '\nNo Identity limit will be assigned if this field is left blank.',
              ),
              keyboardType: TextInputType.number,
            ),
            Gaps.h16,
            DropdownButton<String>(
              hint: const Text('Select Default Tier'),
              isExpanded: true,
              value: _chosenDefaultTier.isNotEmpty ? _chosenDefaultTier : null,
              onChanged: !_isEnabled
                  ? null
                  : (String? newChosenTier) {
                      setState(() {
                        _chosenDefaultTier = newChosenTier ?? '';
                      });
                    },
              items: widget.defaultTiers.map((tier) {
                return DropdownMenuItem<String>(
                  value: tier.id,
                  child: Text(tier.name),
                );
              }).toList(),
            ),
            Gaps.h8,
            if (_errorMessage.isNotEmpty)
              Text(
                _errorMessage,
                style: TextStyle(color: Theme.of(context).colorScheme.error),
              ),
            Row(
              crossAxisAlignment: CrossAxisAlignment.end,
              mainAxisAlignment: MainAxisAlignment.end,
              children: [
                ElevatedButton(
                  onPressed: _chosenDefaultTier.isNotEmpty && _isEnabled ? _createClient : null,
                  child: const Text('Save'),
                ),
                Gaps.w8,
                ElevatedButton(
                  onPressed: () {
                    _clearAllFields();
                    Navigator.of(context).pop();
                  },
                  child: const Text('Close'),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Future<void> _createClient() async {
    int? maxNumberOfIdentities;
    if (_maxIdentitiesController.text.isNotEmpty) {
      maxNumberOfIdentities = int.parse(_maxIdentitiesController.text);
    }

    final response = await GetIt.I.get<AdminApiClient>().clients.createClient(
          defaultTier: _chosenDefaultTier,
          clientId: _clientIdController.text.isNotEmpty ? _clientIdController.text : null,
          clientSecret: _clientSecretController.text.isNotEmpty ? _clientSecretController.text : null,
          displayName: _displayNameController.text.isNotEmpty ? _displayNameController.text : null,
          maxIdentities: maxNumberOfIdentities,
        );

    if (response.hasError) {
      setState(() {
        _errorMessage = response.error.message;
      });
    } else {
      setState(() {
        if (_clientIdController.text.isEmpty) {
          _clientIdController.text = response.data.clientId;
        }
        if (_clientSecretController.text.isEmpty) {
          _displayNameController.text = response.data.displayName;
        }
        if (_clientSecretController.text.isEmpty) {
          _clientSecretController.text = response.data.clientSecret;
        }

        _saveClientSecretMessage = 'Please save the Client Secret since it will be inaccessible after exiting.';
        _isEnabled = false;
        widget.loadClients();
      });
    }
  }

  void _clearAllFields() {
    _clientIdController.clear();
    _displayNameController.clear();
    _clientSecretController.clear();
    _maxIdentitiesController.clear();
    _chosenDefaultTier = '';
    _saveClientSecretMessage = '';
    _errorMessage = '';
  }
}

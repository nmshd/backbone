import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:admin_ui/core/constants.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:get_it/get_it.dart';
import 'package:go_router/go_router.dart';

Future<void> showCreateClientDialog({
  required BuildContext context,
  required List<TierOverview> defaultTiers,
  required VoidCallback onClientCreated,
}) async {
  await showDialog<void>(
    context: context,
    builder: (BuildContext context) => _CreateClientDialog(defaultTiers: defaultTiers, onClientCreated: onClientCreated),
  );
}

class _CreateClientDialog extends StatefulWidget {
  final List<TierOverview> defaultTiers;
  final VoidCallback onClientCreated;

  const _CreateClientDialog({required this.defaultTiers, required this.onClientCreated});

  @override
  State<_CreateClientDialog> createState() => _CreateClientDialogState();
}

class _CreateClientDialogState extends State<_CreateClientDialog> {
  final TextEditingController _clientIdController = TextEditingController();
  final TextEditingController _displayNameController = TextEditingController();
  final TextEditingController _clientSecretController = TextEditingController();
  final TextEditingController _maxIdentitiesController = TextEditingController();
  String? _chosenDefaultTier;

  bool _isClientSecretVisible = true;
  bool _saving = false;
  bool _saveSucceeded = false;

  String? _errorMessage;

  @override
  void initState() {
    super.initState();

    _clientSecretController.addListener(() => setState(() {}));
  }

  @override
  void dispose() {
    _clientIdController.dispose();
    _displayNameController.dispose();
    _clientSecretController.dispose();
    _maxIdentitiesController.dispose();

    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return PopScope(
      canPop: !_saving,
      child: AlertDialog(
        title: const Text('Create Client', textAlign: TextAlign.center),
        content: SizedBox(
          width: 500,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              TextField(
                controller: _clientIdController,
                readOnly: _saveSucceeded,
                decoration: const InputDecoration(
                  border: OutlineInputBorder(),
                  labelText: 'Client ID',
                  helperText: 'A Client ID will be generated if this field is left blank.',
                ),
              ),
              Gaps.h24,
              TextField(
                controller: _displayNameController,
                readOnly: _saveSucceeded,
                decoration: const InputDecoration(
                  border: OutlineInputBorder(),
                  labelText: 'Display Name',
                  helperText: 'Client ID will be used as a Display Name if no value is provided.',
                ),
              ),
              Gaps.h24,
              TextField(
                controller: _clientSecretController,
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
                          onPressed: _clientSecretController.text.isNotEmpty
                              ? () => Clipboard.setData(ClipboardData(text: _clientSecretController.text))
                              : null,
                        ),
                      ],
                    ),
                  ),
                ),
              ),
              if (_saveSucceeded) ...[
                Gaps.h16,
                Text(
                  'Please save the Client Secret since it will be inaccessible after exiting.',
                  style: TextStyle(color: Theme.of(context).colorScheme.primary),
                ),
              ],
              Gaps.h24,
              TextField(
                controller: _maxIdentitiesController,
                readOnly: _saveSucceeded,
                decoration: const InputDecoration(
                  border: OutlineInputBorder(),
                  labelText: 'Max Identities',
                  helperText: 'The maximum number of Identities that can be created with this Client.'
                      '\nNo Identity limit will be assigned if this field is left blank.',
                ),
                inputFormatters: [FilteringTextInputFormatter.digitsOnly],
                keyboardType: TextInputType.number,
              ),
              Gaps.h24,
              DropdownButtonFormField<String>(
                isExpanded: true,
                decoration: const InputDecoration(
                  border: OutlineInputBorder(),
                  labelText: 'Default Tier*',
                ),
                value: _chosenDefaultTier,
                onChanged: _saveSucceeded ? null : (tier) => setState(() => _chosenDefaultTier = tier),
                items: widget.defaultTiers.map((tier) {
                  return DropdownMenuItem<String>(
                    value: tier.id,
                    child: Text(tier.name),
                  );
                }).toList(),
              ),
              if (_errorMessage != null) ...[
                Gaps.h16,
                Text(
                  _errorMessage!,
                  style: TextStyle(color: Theme.of(context).colorScheme.error),
                ),
              ],
            ],
          ),
        ),
        actions: [
          OutlinedButton(onPressed: _saving ? null : () => context.pop(), child: Text(_saveSucceeded ? 'Close' : 'Cancel')),
          if (!_saveSucceeded)
            FilledButton(
              onPressed: _chosenDefaultTier != null && !_saveSucceeded && !_saving ? _createClient : null,
              child: const Text('Save'),
            ),
        ],
      ),
    );
  }

  Future<void> _createClient() async {
    if (_chosenDefaultTier == null) return;

    setState(() => _saving = true);

    final maxNumberOfIdentities = _maxIdentitiesController.text.isNotEmpty ? int.parse(_maxIdentitiesController.text) : null;

    final response = await GetIt.I.get<AdminApiClient>().clients.createClient(
          defaultTier: _chosenDefaultTier!,
          clientId: _clientIdController.text.isNotEmpty ? _clientIdController.text : null,
          clientSecret: _clientSecretController.text.isNotEmpty ? _clientSecretController.text : null,
          displayName: _displayNameController.text.isNotEmpty ? _displayNameController.text : null,
          maxIdentities: maxNumberOfIdentities,
        );

    setState(() => _saving = false);

    if (response.hasError) return setState(() => _errorMessage = response.error.message);

    widget.onClientCreated();

    _clientIdController.text = response.data.clientId;
    _displayNameController.text = response.data.displayName;
    _clientSecretController.text = response.data.clientSecret;

    setState(() => _saveSucceeded = true);
  }
}

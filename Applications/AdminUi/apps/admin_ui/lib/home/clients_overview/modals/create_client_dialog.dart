import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:enmeshed_ui_kit/enmeshed_ui_kit.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:get_it/get_it.dart';
import 'package:go_router/go_router.dart';

import '/core/core.dart';

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
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
        title: Text(context.l10n.createClientDialog_title, textAlign: TextAlign.center),
        contentPadding: const EdgeInsets.only(left: 24, right: 24, top: 20, bottom: 32),
        content: SizedBox(
          width: 500,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              TextField(
                controller: _clientIdController,
                readOnly: _saveSucceeded,
                decoration: InputDecoration(
                  border: const OutlineInputBorder(),
                  labelText: context.l10n.clientID,
                  helperText: context.l10n.createClientDialog_clientID_message,
                ),
              ),
              Gaps.h24,
              TextField(
                controller: _clientSecretController,
                readOnly: _saveSucceeded,
                obscureText: _isClientSecretVisible,
                decoration: InputDecoration(
                  border: const OutlineInputBorder(),
                  labelText: context.l10n.clientSecret,
                  helperText: context.l10n.clientSecret_message,
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
                        CopyToClipboardButton(
                          tooltip: context.l10n.createClientDialog_clientSecret_copyToClipboard,
                          clipboardText: _clientSecretController.text,
                          successMessage: context.l10n.clientSecret_copiedToClipboard,
                        ),
                      ],
                    ),
                  ),
                ),
              ),
              if (_saveSucceeded) ...[
                Gaps.h16,
                Text(context.l10n.clientSecret_save_message, style: TextStyle(color: Theme.of(context).colorScheme.primary)),
              ],
              Gaps.h24,
              TextField(
                controller: _displayNameController,
                readOnly: _saveSucceeded,
                decoration: InputDecoration(
                  border: const OutlineInputBorder(),
                  labelText: context.l10n.displayName,
                  helperText: context.l10n.createClientDialog_displayName_message,
                ),
              ),
              Gaps.h24,
              TextField(
                controller: _maxIdentitiesController,
                readOnly: _saveSucceeded,
                decoration: InputDecoration(
                  border: const OutlineInputBorder(),
                  labelText: context.l10n.maxIdentities,
                  helperText:
                      '${context.l10n.createClientDialog_maxIdentities_message}'
                      '\n${context.l10n.createClientDialog_maxIdentities_noLimit_message}',
                ),
                inputFormatters: [FilteringTextInputFormatter.digitsOnly],
                keyboardType: TextInputType.number,
              ),
              Gaps.h24,
              DropdownButtonFormField<String>(
                isExpanded: true,
                decoration: InputDecoration(border: const OutlineInputBorder(), labelText: '${context.l10n.defaultTier}*'),
                value: _chosenDefaultTier,
                onChanged: _saveSucceeded ? null : (tier) => setState(() => _chosenDefaultTier = tier),
                items:
                    widget.defaultTiers.map((tier) {
                      return DropdownMenuItem<String>(value: tier.id, child: Text(tier.name));
                    }).toList(),
              ),
              if (_errorMessage != null) ...[Gaps.h16, Text(_errorMessage!, style: TextStyle(color: Theme.of(context).colorScheme.error))],
            ],
          ),
        ),
        actions: [
          OutlinedButton(onPressed: _saving ? null : () => context.pop(), child: Text(_saveSucceeded ? context.l10n.close : context.l10n.cancel)),
          if (!_saveSucceeded)
            FilledButton(
              onPressed: _chosenDefaultTier != null && !_saveSucceeded && !_saving ? _createClient : null,
              child: Text(context.l10n.create),
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

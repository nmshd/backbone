import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '/core/core.dart';

Future<void> showCreateAnnouncementDialog({
  required BuildContext context,
  required VoidCallback onAnnouncementCreated,
}) async {
  await showDialog<void>(
    context: context,
    builder: (BuildContext context) => _CreateAnnouncementDialog(onAnnouncementCreated: onAnnouncementCreated),
  );
}

class _CreateAnnouncementDialog extends StatefulWidget {
  final VoidCallback onAnnouncementCreated;

  const _CreateAnnouncementDialog({required this.onAnnouncementCreated});

  @override
  State<_CreateAnnouncementDialog> createState() => _CreateAnnouncementDialogState();
}

class _CreateAnnouncementDialogState extends State<_CreateAnnouncementDialog> {
  bool _saving = false;
  bool _saveSucceeded = false;

  @override
  Widget build(BuildContext context) {
    return PopScope(
      canPop: !_saving,
      child: AlertDialog(
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
        title: Text(context.l10n.createAnnouncementDialog_title, textAlign: TextAlign.center),
        contentPadding: const EdgeInsets.only(left: 24, right: 24, top: 20, bottom: 32),
        content: const SizedBox(
          width: 500,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [Text('Hello')],
          ),
        ),
        actions: [
          OutlinedButton(onPressed: _saving ? null : () => context.pop(), child: Text(_saveSucceeded ? context.l10n.close : context.l10n.cancel)),
          if (!_saveSucceeded)
            FilledButton(
              onPressed: !_saveSucceeded && !_saving ? _createAnnouncement : null,
              child: Text(context.l10n.create),
            ),
        ],
      ),
    );
  }

  Future<void> _createAnnouncement() async {
    // if (_chosenDefaultTier == null) return;

    setState(() => _saving = true);

    // final maxNumberOfIdentities = _maxIdentitiesController.text.isNotEmpty ? int.parse(_maxIdentitiesController.text) : null;

    // final response = await GetIt.I.get<AdminApiClient>().clients.createClient(
    //       defaultTier: _chosenDefaultTier!,
    //       clientId: _clientIdController.text.isNotEmpty ? _clientIdController.text : null,
    //       clientSecret: _clientSecretController.text.isNotEmpty ? _clientSecretController.text : null,
    //       displayName: _displayNameController.text.isNotEmpty ? _displayNameController.text : null,
    //       maxIdentities: maxNumberOfIdentities,
    //     );

    setState(() => _saving = false);

    // if (response.hasError) return setState(() => _errorMessage = response.error.message);

    widget.onAnnouncementCreated();

    // _clientIdController.text = response.data.clientId;
    // _displayNameController.text = response.data.displayName;
    // _clientSecretController.text = response.data.clientSecret;

    setState(() => _saveSucceeded = true);
  }
}

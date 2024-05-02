import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:go_router/go_router.dart';

import '/core/core.dart';

Future<bool> showRemoveClientsDialog({
  required BuildContext context,
  required Set<String> selectedClients,
}) async {
  final removedClients = await showDialog<bool>(
    context: context,
    builder: (BuildContext context) => _RemoveClientsDialog(selectedClients: selectedClients),
  );

  return removedClients ?? false;
}

class _RemoveClientsDialog extends StatefulWidget {
  final Set<String> selectedClients;

  const _RemoveClientsDialog({required this.selectedClients});

  @override
  State<_RemoveClientsDialog> createState() => _RemoveClientsDialogState();
}

class _RemoveClientsDialogState extends State<_RemoveClientsDialog> {
  bool _deleting = false;
  bool _deletionSucceeded = false;

  @override
  Widget build(BuildContext context) {
    return PopScope(
      canPop: !_deleting,
      child: AlertDialog(
        title: Text(widget.selectedClients.length == 1 ? 'Delete Client' : 'Delete Clients', textAlign: TextAlign.center),
        content: switch ((_deleting, _deletionSucceeded)) {
          (_, true) => _RemoveClientsSucceeded(numberOfDeletedClients: widget.selectedClients.length),
          (true, _) => const _RemoveClientsLoading(),
          (_, _) => Text(
              'Are you sure you want to delete the selected ${widget.selectedClients.length > 1 ? 'clients' : 'client'}?',
              style: TextStyle(color: Theme.of(context).colorScheme.primary),
            ),
        },
        actions: <Widget>[
          OutlinedButton(
            onPressed: _deleting ? null : () => context.pop(),
            child: Text(_deletionSucceeded ? 'Close' : 'Cancel'),
          ),
          if (!_deletionSucceeded)
            TextButton(
              onPressed: _deleting ? null : _deleteSelectedClients,
              child: Text('Yes', style: TextStyle(color: Theme.of(context).colorScheme.error)),
            ),
        ],
      ),
    );
  }

  Future<void> _deleteSelectedClients() async {
    setState(() => _deleting = true);

    for (final clientId in widget.selectedClients) {
      // TODO: error handling
      // what if one fails after another one succeeded?
      await GetIt.I.get<AdminApiClient>().clients.deleteClient(clientId);
    }

    setState(() {
      _deleting = false;
      _deletionSucceeded = true;
    });
  }
}

class _RemoveClientsLoading extends StatelessWidget {
  const _RemoveClientsLoading();

  @override
  Widget build(BuildContext context) {
    return const Column(
      mainAxisSize: MainAxisSize.min,
      children: [
        Text('Removing clients...'),
        Gaps.h16,
        Padding(
          padding: EdgeInsets.all(16),
          child: Wrap(alignment: WrapAlignment.center, children: [CircularProgressIndicator()]),
        ),
      ],
    );
  }
}

class _RemoveClientsSucceeded extends StatelessWidget {
  final int numberOfDeletedClients;

  const _RemoveClientsSucceeded({required this.numberOfDeletedClients});

  @override
  Widget build(BuildContext context) {
    return Column(
      mainAxisSize: MainAxisSize.min,
      children: [
        const Icon(Icons.check_circle, color: Colors.green, size: 48),
        Gaps.h8,
        Text(
          'Successfully removed ${numberOfDeletedClients == 1 ? 'one client.' : '$numberOfDeletedClients clients.'}',
          style: TextStyle(color: Theme.of(context).colorScheme.primary),
        ),
      ],
    );
  }
}

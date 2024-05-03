import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:go_router/go_router.dart';
import 'package:logger/logger.dart';

import '/core/core.dart';

Future<void> showRemoveClientsDialog({
  required BuildContext context,
  required Set<String> selectedClients,
  required VoidCallback onClientsRemoved,
}) async {
  await showDialog<void>(
    context: context,
    builder: (BuildContext context) => _RemoveClientsDialog(selectedClients: selectedClients, onClientsRemoved: onClientsRemoved),
  );
}

class _RemoveClientsDialog extends StatefulWidget {
  final Set<String> selectedClients;
  final VoidCallback onClientsRemoved;

  const _RemoveClientsDialog({required this.selectedClients, required this.onClientsRemoved});

  @override
  State<_RemoveClientsDialog> createState() => _RemoveClientsDialogState();
}

class _RemoveClientsDialogState extends State<_RemoveClientsDialog> {
  bool _deleting = false;
  bool _deletionSucceeded = false;
  String? _deletionErrorMessage;
  final _deletedClients = <String>[];

  @override
  Widget build(BuildContext context) {
    return PopScope(
      canPop: !_deleting,
      child: AlertDialog(
        title: Text(widget.selectedClients.length == 1 ? 'Delete Client' : 'Delete Clients', textAlign: TextAlign.center),
        content: switch ((_deleting, _deletionSucceeded)) {
          (_, true) => _RemoveClientsSucceeded(numberOfDeletedClients: _deletedClients.length),
          (true, _) => const _RemoveClientsLoading(),
          (_, _) => Text(
              _deletionErrorMessage ?? 'Are you sure you want to delete the selected ${widget.selectedClients.length > 1 ? 'clients' : 'client'}?',
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

    try {
      for (final clientId in widget.selectedClients.toList()) {
        await GetIt.I.get<AdminApiClient>().clients.deleteClient(clientId);
        widget.onClientsRemoved();
        widget.selectedClients.remove(clientId);
        _deletedClients.add(clientId);
      }
    } catch (e) {
      GetIt.I.get<Logger>().e('An error occurred while deleting the client(s). $e');

      setState(() {
        _deleting = false;
        _deletionSucceeded = false;
        _deletionErrorMessage = 'An error occurred while deleting the client(s). Please try again.';
      });

      return;
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
          'Successfully deleted ${numberOfDeletedClients == 1 ? 'one client.' : '$numberOfDeletedClients clients.'}',
          style: TextStyle(color: Theme.of(context).colorScheme.primary),
        ),
      ],
    );
  }
}

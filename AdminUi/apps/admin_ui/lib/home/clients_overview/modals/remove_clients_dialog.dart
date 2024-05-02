import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:go_router/go_router.dart';

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

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: const Text('Confirm Deletion'),
      content: _deleting
          // TODO: make prettier / add text
          ? const CircularProgressIndicator()
          : Text(
              'Are you sure you want to delete the selected ${widget.selectedClients.length > 1 ? 'clients' : 'client'}?',
              style: TextStyle(color: Theme.of(context).colorScheme.primary),
            ),
      actions: <Widget>[
        OutlinedButton(
          onPressed: _deleting ? null : () => context.pop(),
          child: const Text('Cancel'),
        ),
        TextButton(
          onPressed: _deleting ? null : _deleteSelectedClients,
          child: Text('Yes', style: TextStyle(color: Theme.of(context).colorScheme.error)),
        ),
      ],
    );
  }

  Future<void> _deleteSelectedClients() async {
    setState(() => _deleting = true);

    for (final clientId in widget.selectedClients) {
      // TODO: error handling
      // what if one fails after another one succeeded?
      await GetIt.I.get<AdminApiClient>().clients.deleteClient(clientId);
    }

    if (mounted) {
      context.pop(true);
      final snackBar = SnackBar(
        content: Text(
          widget.selectedClients.length == 1 ? 'Successfully removed one client.' : 'Successfully removed ${widget.selectedClients.length} clients.',
        ),
      );
      ScaffoldMessenger.of(context).showSnackBar(snackBar);
    }
  }
}

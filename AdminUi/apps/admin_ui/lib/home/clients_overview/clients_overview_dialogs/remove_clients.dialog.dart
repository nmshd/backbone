import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';

class RemoveClientsDialog extends StatefulWidget {
  final Map<String, bool> selectedClients;
  final Future<void> Function() loadClients;
  final void Function(int numberOfRemovedClients) onSuccess;

  const RemoveClientsDialog({required this.selectedClients, required this.loadClients, required this.onSuccess, super.key});

  @override
  State<RemoveClientsDialog> createState() => _RemoveClientsDialogState();
}

class _RemoveClientsDialogState extends State<RemoveClientsDialog> {
  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: const Text('Confirm Deletion'),
      content: Text(
        'Are you sure you want to delete the selected client?',
        style: TextStyle(color: Theme.of(context).colorScheme.primary),
      ),
      actions: <Widget>[
        TextButton(
          onPressed: () {
            Navigator.of(context).pop();
          },
          child: const Text('Cancel'),
        ),
        TextButton(
          onPressed: () {
            _deleteSelectedClients();
            Navigator.of(context).pop();
          },
          child: Text('Yes', style: TextStyle(color: Theme.of(context).colorScheme.error)),
        ),
      ],
    );
  }

  Future<void> _deleteSelectedClients() async {
    final numberOfRemovedClients = widget.selectedClients.length;
    final deletionFutures = <Future<dynamic>>[];
    widget.selectedClients.forEach((clientId, isSelected) {
      if (isSelected) {
        deletionFutures.add(GetIt.I.get<AdminApiClient>().clients.deleteClient(clientId));
      }
    });

    await Future.wait(deletionFutures);

    await widget.loadClients();

    setState(() {
      widget.selectedClients.clear();
      widget.onSuccess(numberOfRemovedClients);
    });
  }
}

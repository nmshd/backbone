import 'package:admin_api_types/admin_api_types.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '/core/extensions.dart';

Future<void> showAllRecipientsDialog({required BuildContext context, required List<MessageRecipient> recipients}) => showDialog<void>(
  context: context,
  builder: (BuildContext context) => _AllRecipientsDialog(recipients: recipients),
);

class _AllRecipientsDialog extends StatefulWidget {
  final List<MessageRecipient> recipients;
  const _AllRecipientsDialog({required this.recipients});

  @override
  State<_AllRecipientsDialog> createState() => _AllRecipientsDialogState();
}

class _AllRecipientsDialogState extends State<_AllRecipientsDialog> {
  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: Text(context.l10n.recipientsDialog_listOfAllRecipients, textAlign: TextAlign.center),
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
      contentPadding: const EdgeInsets.only(left: 24, right: 24, top: 20, bottom: 32),
      content: Padding(
        padding: const EdgeInsets.all(8),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: widget.recipients
              .map(
                (recipient) => Padding(
                  padding: const EdgeInsets.all(4),
                  child: InkWell(
                    onTap: () {
                      context.push('/identities/${recipient.address}');
                      Navigator.of(context).pop();
                    },
                    child: Text(recipient.address),
                  ),
                ),
              )
              .toList(),
        ),
      ),
      actions: [OutlinedButton(onPressed: () => Navigator.of(context).pop(), child: Text(context.l10n.close))],
    );
  }
}

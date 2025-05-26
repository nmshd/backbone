import 'package:enmeshed_ui_kit/enmeshed_ui_kit.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '/core/core.dart';

class QueryDeletionProcessAuditLogs extends StatefulWidget {
  const QueryDeletionProcessAuditLogs({super.key});

  @override
  State<QueryDeletionProcessAuditLogs> createState() => _QueryDeletionProcessAuditLogsState();
}

class _QueryDeletionProcessAuditLogsState extends State<QueryDeletionProcessAuditLogs> {
  late TextEditingController textController;
  bool isButtonEnabled = false;

  @override
  void initState() {
    super.initState();
    textController = TextEditingController();

    textController.addListener(() => setState(() => isButtonEnabled = textController.text.trim().isNotEmpty));
  }

  @override
  void dispose() {
    textController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(context.l10n.deletionProcessAuditLogs_title, style: const TextStyle(fontSize: 24)),
            Gaps.h16,
            Row(
              children: [
                SizedBox(
                  width: 500,
                  child: Padding(
                    padding: const EdgeInsets.all(8),
                    child: TextFormField(
                      controller: textController,
                      decoration: InputDecoration(
                        hintText: context.l10n.deletionProcessAuditLogs_inputHint,
                        labelText: context.l10n.address,
                        alignLabelWithHint: true,
                        border: const OutlineInputBorder(),
                      ),
                    ),
                  ),
                ),
                Gaps.w8,
                ElevatedButton(
                  onPressed: isButtonEnabled
                      ? () {
                          final address = textController.text.trim();
                          textController.clear();
                          context.push('/identities/$address/deletion-process-audit-logs');
                        }
                      : null,
                  style: ElevatedButton.styleFrom(
                    backgroundColor: isButtonEnabled ? Theme.of(context).colorScheme.primary : Colors.grey,
                    shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
                    padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 24),
                  ),
                  child: Text(context.l10n.find, style: TextStyle(color: isButtonEnabled ? Theme.of(context).colorScheme.onPrimary : Colors.white)),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

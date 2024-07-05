import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '/core/core.dart';

class DeletionProcessAuditLogs extends StatefulWidget {
  const DeletionProcessAuditLogs({super.key});

  @override
  State<DeletionProcessAuditLogs> createState() => _DeletionProcessAuditLogsState();
}

class _DeletionProcessAuditLogsState extends State<DeletionProcessAuditLogs> {
  late TextEditingController textController;

  @override
  void initState() {
    super.initState();
    textController = TextEditingController();
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
            Text(
              context.l10n.deletionProcessAuditLogs_title,
              style: const TextStyle(fontSize: 24),
            ),
            Gaps.h16,
            Text(
              '${context.l10n.address}:',
              style: const TextStyle(
                fontSize: 16,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 8),
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
                        border: const OutlineInputBorder(),
                      ),
                    ),
                  ),
                ),
                Gaps.w8,
                ElevatedButton(
                  onPressed: () {
                    final address = textController.text.trim();
                    textController.clear();
                    context.push('/identities/$address/deletion-process-audit-logs');
                  },
                  style: ElevatedButton.styleFrom(
                    backgroundColor: Theme.of(context).colorScheme.primary,
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(10),
                    ),
                    padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 24),
                  ),
                  child: Text(
                    context.l10n.find,
                    style: TextStyle(color: Theme.of(context).colorScheme.onPrimary),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

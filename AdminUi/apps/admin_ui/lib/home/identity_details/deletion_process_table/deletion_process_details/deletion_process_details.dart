import 'dart:io';

import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:intl/intl.dart';

import '/core/core.dart';
import './deletion_process_auditlogs/deletion_process_auditlogs.dart';

class DeletionProcessDetails extends StatefulWidget {
  final String address;
  final String deletionProcessId;

  const DeletionProcessDetails({required this.address, required this.deletionProcessId, super.key});

  @override
  State<DeletionProcessDetails> createState() => _DeletionProcessDetailsState();
}

class _DeletionProcessDetailsState extends State<DeletionProcessDetails> {
  DeletionProcess? _deletionProcessesDetails;

  late final ScrollController _scrollController;

  @override
  void initState() {
    super.initState();

    _scrollController = ScrollController();

    _reloadDeletionProcess();
  }

  @override
  void dispose() {
    _scrollController.dispose();

    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    if (_deletionProcessesDetails == null) return const Center(child: CircularProgressIndicator());

    final deletionProcessDetails = _deletionProcessesDetails!;
    return Scrollbar(
      controller: _scrollController,
      child: SingleChildScrollView(
        controller: _scrollController,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            if (Platform.isMacOS || Platform.isWindows) const BackButton(),
            _DeletionProcessDetailsCard(
              address: widget.address,
              deletionProcessDetails: deletionProcessDetails,
              updateDeletionProcess: _reloadDeletionProcess,
            ),
            Gaps.h16,
            DeletionProcessAuditLogsTable(auditLogs: deletionProcessDetails.auditLogs ?? []),
          ],
        ),
      ),
    );
  }

  Future<void> _reloadDeletionProcess() async {
    final deletionProcessesDetails = await GetIt.I
        .get<AdminApiClient>()
        .identities
        .getIdentityDeletionProcessDetails(address: widget.address, deletionProcessId: widget.deletionProcessId);
    if (mounted) {
      setState(() {
        _deletionProcessesDetails = deletionProcessesDetails.data;

        // Log the entire _deletionProcessesDetails to inspect its structure
        print('_deletionProcessesDetails: $_deletionProcessesDetails');

        final auditLogs = _deletionProcessesDetails?.auditLogs;
        if (auditLogs != null) {
          for (var auditLog in auditLogs) {
            print('AuditLog ID: ${auditLog.id}');
            print('Created At: ${auditLog.createdAt}');
            print('Old Status: ${auditLog.oldStatus}');
            print('New Status: ${auditLog.newStatus}');
            print('Message Key: ${auditLog.messageKey}');
            print('Additional Data: ${auditLog.additionalData}');
          }
        } else {
          print('AuditLogs are null');
        }
      });
    }
  }
}

class _DeletionProcessDetailsCard extends StatelessWidget {
  final String address;
  final DeletionProcess deletionProcessDetails;
  final VoidCallback updateDeletionProcess;

  const _DeletionProcessDetailsCard({
    required this.address,
    required this.deletionProcessDetails,
    required this.updateDeletionProcess,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Expanded(
          child: Card(
            child: Padding(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(context.l10n.deletionProcessDetails_title, style: Theme.of(context).textTheme.headlineLarge),
                  Gaps.h32,
                  Wrap(
                    crossAxisAlignment: WrapCrossAlignment.center,
                    spacing: 8,
                    runSpacing: 8,
                    children: [
                      _DeletionProcessDetails(title: context.l10n.address, value: address),
                      _DeletionProcessDetails(title: context.l10n.deletionProcessDetails_deletionProcessID, value: deletionProcessDetails.id),
                      _DeletionProcessDetails(
                        title: context.l10n.createdAt,
                        value:
                            '${DateFormat.yMd(Localizations.localeOf(context).languageCode).format(deletionProcessDetails.createdAt)} ${DateFormat.Hms().format(deletionProcessDetails.createdAt)}',
                      ),
                      _DeletionProcessDetails(title: context.l10n.deletionProcessDetails_status, value: deletionProcessDetails.status),
                    ],
                  ),
                ],
              ),
            ),
          ),
        ),
      ],
    );
  }
}

class _DeletionProcessDetails extends StatelessWidget {
  final String title;
  final String value;

  const _DeletionProcessDetails({required this.title, required this.value});

  @override
  Widget build(BuildContext context) {
    return RawChip(
      label: Text.rich(
        TextSpan(
          children: [
            TextSpan(text: '$title ', style: Theme.of(context).textTheme.bodyLarge!.copyWith(fontWeight: FontWeight.bold)),
            TextSpan(text: value, style: Theme.of(context).textTheme.bodyLarge),
          ],
        ),
      ),
    );
  }
}

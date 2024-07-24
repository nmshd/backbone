import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/material.dart';

import 'messages_data_table/identity_messages_table.dart';

class IdentityMessagesOverview extends StatefulWidget {
  final String participant;
  final MessageType type;
  final String title;
  final String subtitle;
  final String emptyTableMessage;

  const IdentityMessagesOverview({
    required this.participant,
    required this.type,
    required this.title,
    required this.subtitle,
    required this.emptyTableMessage,
    super.key,
  });

  @override
  State<IdentityMessagesOverview> createState() => _IdentityMessagesOverviewState();
}

class _IdentityMessagesOverviewState extends State<IdentityMessagesOverview> {
  late IdentityMessagesTableSource _dataSource;

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();

    _dataSource = IdentityMessagesTableSource(
      participant: widget.participant,
      type: widget.type,
      locale: Localizations.localeOf(context),
    );
  }

  @override
  void dispose() {
    _dataSource.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return IdentityMessagesTable(
      dataSource: _dataSource,
      type: widget.type,
      title: widget.title,
      subtitle: widget.subtitle,
      emptyTableMessage: widget.emptyTableMessage,
    );
  }
}

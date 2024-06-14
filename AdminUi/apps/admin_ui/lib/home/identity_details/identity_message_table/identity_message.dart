import 'package:flutter/material.dart';

import '/core/core.dart';

class IdentityMessagesOverview extends StatefulWidget {
  final String address;
  final String type;
  const IdentityMessagesOverview({required this.address, required this.type, super.key});

  @override
  State<IdentityMessagesOverview> createState() => _IdentityMessagesOverviewState();
}

class _IdentityMessagesOverviewState extends State<IdentityMessagesOverview> {
  late IdentityMessagesTableSource _dataSource;

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();

    _dataSource = IdentityMessagesTableSource(
      address: widget.address,
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
    return IdentityMessagesTable(dataSource: _dataSource);
  }
}

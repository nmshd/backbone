import 'package:flutter/material.dart';

import 'identity_relationship_table.dart';

class IdentityRelationshipOverview extends StatefulWidget {
  final String address;
  final String title;
  final String subtitle;
  final String emptyTableMessage;

  const IdentityRelationshipOverview({
    required this.address,
    required this.title,
    required this.subtitle,
    required this.emptyTableMessage,
    super.key,
  });

  @override
  State<IdentityRelationshipOverview> createState() => _IdentityRelationshipOverviewState();
}

class _IdentityRelationshipOverviewState extends State<IdentityRelationshipOverview> {
  late IdentityRelationshipSource _dataSource;

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();

    _dataSource = IdentityRelationshipSource(
      address: widget.address,
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
    return IdentityRelationshipTable(
      dataSource: _dataSource,
      title: widget.title,
      subtitle: widget.subtitle,
      emptyTableMessage: widget.emptyTableMessage,
    );
  }
}

import 'package:flutter/material.dart';

import 'identity_relationship_table.dart';

class IdentityRelationship extends StatefulWidget {
  final String address;

  const IdentityRelationship({
    required this.address,
    super.key,
  });

  @override
  State<IdentityRelationship> createState() => _IdentityRelationshipState();
}

class _IdentityRelationshipState extends State<IdentityRelationship> {
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
    );
  }
}

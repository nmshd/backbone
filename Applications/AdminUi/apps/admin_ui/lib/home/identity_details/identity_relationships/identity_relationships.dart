import 'package:flutter/material.dart';

import 'identity_relationship_data_table_source.dart';
import 'identity_relationship_table.dart';

class IdentityRelationships extends StatefulWidget {
  final String address;

  const IdentityRelationships({required this.address, super.key});

  @override
  State<IdentityRelationships> createState() => _IdentityRelationshipsState();
}

class _IdentityRelationshipsState extends State<IdentityRelationships> {
  late IdentityRelationshipDataTableSource _dataSource;

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();

    _dataSource = IdentityRelationshipDataTableSource(address: widget.address, locale: Localizations.localeOf(context));
  }

  @override
  void dispose() {
    _dataSource.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return IdentityRelationshipTable(dataSource: _dataSource);
  }
}

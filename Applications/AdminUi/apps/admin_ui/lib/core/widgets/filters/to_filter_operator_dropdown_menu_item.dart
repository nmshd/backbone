import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/material.dart';

extension ToFilterOperatorDropdownMenuItem on List<FilterOperator> {
  List<DropdownMenuItem<FilterOperator>> toDropdownMenuItems() =>
      map((operator) => DropdownMenuItem<FilterOperator>(value: operator, child: Text(operator.userFriendlyOperator))).toList();
}

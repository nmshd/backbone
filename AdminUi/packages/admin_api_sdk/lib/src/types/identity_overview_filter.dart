enum FilterOperator {
  equal,
  notEqual,
  greaterThan,
  greaterThanOrEqual,
  lessThan,
  lessThanOrEqual,
}

class IdentityOverviewFilter {
  final String? address;
  final List<String>? tiers;
  final List<String>? clients;
  final FilterOperatorValue? createdAt;
  final FilterOperatorValue? lastLoginAt;
  final FilterOperatorValue? numberOfDevices;
  final FilterOperatorValue? datawalletVersion;
  final FilterOperatorValue? identityVersion;

  IdentityOverviewFilter({
    this.address,
    this.tiers,
    this.clients,
    this.createdAt,
    this.lastLoginAt,
    this.numberOfDevices,
    this.datawalletVersion,
    this.identityVersion,
  });
}

class FilterOperatorValue {
  final FilterOperator operator;
  final String value;

  FilterOperatorValue(this.operator, this.value);
}

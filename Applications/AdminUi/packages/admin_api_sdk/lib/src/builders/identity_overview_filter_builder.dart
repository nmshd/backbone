import '../utils/utils.dart';

class IdentityOverviewFilterBuilder {
  final IdentityOverviewFilter filter;

  String _filter = '';

  IdentityOverviewFilterBuilder(this.filter);

  String build() {
    if (filter.address != null) {
      _appendFilter("contains(address, '${filter.address}')");
    }

    if (filter.tiers != null) {
      final tiersFilter = filter.tiers!.map((tier) => "tier/Id eq '$tier'").join(' or ');
      _appendFilter('($tiersFilter)');
    }

    if (filter.clients != null) {
      final clientsFilter = filter.clients!.map((client) => "createdWithClient eq '$client'").join(' or ');
      _appendFilter('($clientsFilter)');
    }

    if (filter.createdAt != null) {
      _appendFilter('createdAt ${_getOperatorString(filter.createdAt!.operator)} ${filter.createdAt!.value.substring(0, 10)}');
    }

    if (filter.lastLoginAt != null) {
      _appendFilter('lastLoginAt ${_getOperatorString(filter.lastLoginAt!.operator)} ${filter.lastLoginAt!.value.substring(0, 10)}');
    }

    if (filter.numberOfDevices != null) {
      _appendFilter('numberOfDevices ${_getOperatorString(filter.numberOfDevices!.operator)} ${filter.numberOfDevices!.value}');
    }

    if (filter.datawalletVersion != null) {
      _appendFilter('datawalletVersion ${_getOperatorString(filter.datawalletVersion!.operator)} ${filter.datawalletVersion!.value}');
    }

    if (filter.identityVersion != null) {
      _appendFilter('identityVersion ${_getOperatorString(filter.identityVersion!.operator)} ${filter.identityVersion!.value}');
    }

    return _filter;
  }

  void _appendFilter(String filter) {
    if (_filter.isNotEmpty) {
      _filter += ' and ';
    }
    _filter += '($filter)';
  }

  String _getOperatorString(FilterOperator operator) => switch (operator) {
    .equal => 'eq',
    .notEqual => 'ne',
    .greaterThan => 'gt',
    .greaterThanOrEqual => 'ge',
    .lessThan => 'lt',
    .lessThanOrEqual => 'le',
  };
}

enum FilterOperator {
  equal('='),
  notEqual('!='),
  greaterThan('>'),
  greaterThanOrEqual('>='),
  lessThan('<'),
  lessThanOrEqual('<=')
  ;

  final String userFriendlyOperator;

  const FilterOperator(this.userFriendlyOperator);
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

  IdentityOverviewFilter copyWith({
    Optional<String?>? address,
    Optional<List<String>?>? tiers,
    Optional<List<String>?>? clients,
    Optional<FilterOperatorValue?>? createdAt,
    Optional<FilterOperatorValue?>? lastLoginAt,
    Optional<FilterOperatorValue?>? numberOfDevices,
    Optional<FilterOperatorValue?>? datawalletVersion,
    Optional<FilterOperatorValue?>? identityVersion,
  }) {
    return IdentityOverviewFilter(
      address: (address != null) ? address.value : this.address,
      tiers: (tiers != null) ? tiers.value : this.tiers,
      clients: (clients != null) ? clients.value : this.clients,
      createdAt: (createdAt != null) ? createdAt.value : this.createdAt,
      lastLoginAt: (lastLoginAt != null) ? lastLoginAt.value : this.lastLoginAt,
      numberOfDevices: (numberOfDevices != null) ? numberOfDevices.value : this.numberOfDevices,
      datawalletVersion: (datawalletVersion != null) ? datawalletVersion.value : this.datawalletVersion,
      identityVersion: (identityVersion != null) ? identityVersion.value : this.identityVersion,
    );
  }
}

class FilterOperatorValue {
  final FilterOperator operator;
  final String value;

  FilterOperatorValue(this.operator, this.value);
}

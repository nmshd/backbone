import 'package:json_annotation/json_annotation.dart';

import '../quotas/quota.dart';

part 'tier.g.dart';

@JsonSerializable()
class Tiers {
  final String id;
  final String name;
  final bool canBeUsedAsDefaultForClient;
  final bool canBeManuallyAssigned;
  final int numberOfIdentities;

  Tiers({
    required this.id,
    required this.name,
    required this.canBeUsedAsDefaultForClient,
    required this.canBeManuallyAssigned,
    required this.numberOfIdentities,
  });

  factory Tiers.fromJson(dynamic json) => _$TiersFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$TiersToJson(this);
}

@JsonSerializable()
class Tier {
  final String id;
  final String name;

  Tier({required this.id, required this.name});

  factory Tier.fromJson(dynamic json) => _$TierFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$TierToJson(this);
}

@JsonSerializable()
class TierDetails {
  final String id;
  final String name;
  final List<Quota> quotas;

  TierDetails({
    required this.id,
    required this.name,
    required this.quotas,
  });

  factory TierDetails.fromJson(dynamic json) => _$TierDetailsFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$TierDetailsToJson(this);
}

@JsonSerializable()
class TierOverview {
  final String id;
  final String name;
  final int numberOfIdentities;
  final bool canBeUsedAsDefaultForClient;
  final bool canBeManuallyAssigned;

  TierOverview({
    required this.id,
    required this.name,
    required this.numberOfIdentities,
    required this.canBeUsedAsDefaultForClient,
    required this.canBeManuallyAssigned,
  });

  factory TierOverview.fromJson(dynamic json) => _$TierOverviewFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$TierOverviewToJson(this);
}

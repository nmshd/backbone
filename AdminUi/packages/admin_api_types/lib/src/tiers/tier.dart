import 'package:json_annotation/json_annotation.dart';

import '../quotas/quota.dart';

part 'tier.g.dart';

@JsonSerializable()
class Tier {
  final String id;
  final String name;

  Tier({
    required this.id,
    required this.name,
  });

  factory Tier.fromJson(dynamic json) => _$TierFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$TierToJson(this);
}

@JsonSerializable()
class TierOverview {
  final String id;
  final String name;
  final List<Quota> quotas;

  TierOverview({
    required this.id,
    required this.name,
    required this.quotas,
  });

  factory TierOverview.fromJson(dynamic json) => _$TierOverviewFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$TierOverviewToJson(this);
}

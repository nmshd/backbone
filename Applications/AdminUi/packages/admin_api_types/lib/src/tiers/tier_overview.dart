import 'package:json_annotation/json_annotation.dart';

part 'tier_overview.g.dart';

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

import 'package:json_annotation/json_annotation.dart';

part 'tier_overview_response.g.dart';

@JsonSerializable()
class TierOverviewResponse {
  final String id;
  final String name;
  final int numberOfIdentities;
  final bool canBeUsedAsDefaultForClient;
  final bool canBeManuallyAssigned;

  TierOverviewResponse({
    required this.id,
    required this.name,
    required this.numberOfIdentities,
    required this.canBeUsedAsDefaultForClient,
    required this.canBeManuallyAssigned,
  });

  factory TierOverviewResponse.fromJson(dynamic json) => _$TierOverviewResponseFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$TierOverviewResponseToJson(this);
}

import 'package:json_annotation/json_annotation.dart';

import 'tier_quota_definition.dart';

part 'tier_details.g.dart';

@JsonSerializable()
class TierDetails {
  final String id;
  final String name;
  final List<TierQuotaDefinition> quotas;

  TierDetails({required this.id, required this.name, required this.quotas});

  factory TierDetails.fromJson(dynamic json) => _$TierDetailsFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$TierDetailsToJson(this);
}

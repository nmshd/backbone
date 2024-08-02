import 'package:admin_api_types/admin_api_types.dart';
import 'package:json_annotation/json_annotation.dart';

part 'tier_quota_definition.g.dart';

@JsonSerializable()
class TierQuotaDefinition {
  final String id;
  final MetricResponse metric;
  final int max;
  final String period;

  TierQuotaDefinition({
    required this.id,
    required this.metric,
    required this.max,
    required this.period,
  });

  factory TierQuotaDefinition.fromJson(dynamic json) => _$TierQuotaDefinitionFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$TierQuotaDefinitionToJson(this);
}

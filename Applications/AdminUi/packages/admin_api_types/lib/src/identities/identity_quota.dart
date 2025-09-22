import 'package:json_annotation/json_annotation.dart';

import '../metrics/metrics.dart';

part 'identity_quota.g.dart';

@JsonSerializable()
class IdentityQuota {
  final String id;
  final String source;
  final Metric metric;
  final int max;
  final int usage;
  final String period;

  IdentityQuota({required this.id, required this.source, required this.metric, required this.max, required this.usage, required this.period});

  factory IdentityQuota.fromJson(dynamic json) => _$IdentityQuotaFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$IdentityQuotaToJson(this);
}

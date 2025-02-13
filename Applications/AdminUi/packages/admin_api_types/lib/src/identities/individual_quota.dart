import 'package:json_annotation/json_annotation.dart';

import '../metrics/metrics.dart';

part 'individual_quota.g.dart';

@JsonSerializable()
class IndividualQuota {
  final String id;
  final Metric metric;
  final int max;
  final String period;

  IndividualQuota({required this.id, required this.metric, required this.max, required this.period});

  factory IndividualQuota.fromJson(dynamic json) => _$IndividualQuotaFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$IndividualQuotaToJson(this);
}

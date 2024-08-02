import 'package:json_annotation/json_annotation.dart';

part 'metric_response.g.dart';

@JsonSerializable()
class MetricResponse {
  final String key;
  final String displayName;

  MetricResponse({
    required this.key,
    required this.displayName,
  });

  factory MetricResponse.fromJson(dynamic json) => _$MetricResponseFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$MetricResponseToJson(this);
}

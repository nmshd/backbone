import 'package:json_annotation/json_annotation.dart';

part 'tier.g.dart';

@JsonSerializable()
class Tier {
  final String id;
  final String name;

  new({required this.id, required this.name});

  factory fromJson(dynamic json) => _$TierFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$TierToJson(this);
}

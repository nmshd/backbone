import 'package:json_annotation/json_annotation.dart';

part 'create_tier_response.g.dart';

@JsonSerializable()
class CreateTierResponse {
  final String id;
  final String name;

  CreateTierResponse({required this.id, required this.name});

  factory CreateTierResponse.fromJson(dynamic json) => _$CreateTierResponseFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$CreateTierResponseToJson(this);
}

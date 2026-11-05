import 'package:json_annotation/json_annotation.dart';

part 'update_client_response.g.dart';

@JsonSerializable()
class UpdateClientResponse {
  final String clientId;
  final String displayName;
  final String defaultTier;
  final DateTime createdAt;
  final int? maxIdentities;

  new({required this.clientId, required this.displayName, required this.defaultTier, required this.createdAt, required this.maxIdentities});

  factory fromJson(dynamic json) => _$UpdateClientResponseFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$UpdateClientResponseToJson(this);
}

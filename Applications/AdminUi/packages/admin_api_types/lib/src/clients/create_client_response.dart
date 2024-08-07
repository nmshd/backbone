import 'package:json_annotation/json_annotation.dart';

part 'create_client_response.g.dart';

@JsonSerializable()
class CreateClientResponse {
  final String clientId;
  final String displayName;
  final String clientSecret;
  final String defaultTier;
  final DateTime createdAt;
  final int? maxIdentities;

  CreateClientResponse({
    required this.clientId,
    required this.displayName,
    required this.clientSecret,
    required this.defaultTier,
    required this.createdAt,
    required this.maxIdentities,
  });

  factory CreateClientResponse.fromJson(dynamic json) => _$CreateClientResponseFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$CreateClientResponseToJson(this);
}

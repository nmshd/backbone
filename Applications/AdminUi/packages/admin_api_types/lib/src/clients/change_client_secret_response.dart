import 'package:json_annotation/json_annotation.dart';

part 'change_client_secret_response.g.dart';

@JsonSerializable()
class ChangeClientSecretResponse {
  final String clientId;
  final String displayName;
  final String clientSecret;
  final String defaultTier;
  final DateTime createdAt;
  final int? maxIdentities;

  new({
    required this.clientId,
    required this.displayName,
    required this.clientSecret,
    required this.defaultTier,
    required this.createdAt,
    required this.maxIdentities,
  });

  factory fromJson(dynamic json) => _$ChangeClientSecretResponseFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$ChangeClientSecretResponseToJson(this);
}

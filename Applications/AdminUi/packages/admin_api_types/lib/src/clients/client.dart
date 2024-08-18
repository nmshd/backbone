import 'package:json_annotation/json_annotation.dart';

part 'client.g.dart';

@JsonSerializable()
class Client {
  final String clientId;
  final String displayName;
  final String defaultTier;
  final DateTime createdAt;
  final int numberOfIdentities;
  final int? maxIdentities;

  Client({
    required this.clientId,
    required this.displayName,
    required this.defaultTier,
    required this.createdAt,
    required this.numberOfIdentities,
    this.maxIdentities,
  });

  factory Client.fromJson(dynamic json) => _$ClientFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$ClientToJson(this);
}

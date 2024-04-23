import 'package:json_annotation/json_annotation.dart';

part 'client.g.dart';

@JsonSerializable()
class Client {
  final String clientId;
  final String displayName;
  final String defaultTier;
  final String clientSecret;
  final String createdAt;
  final int? maxIdentities;
  final int? numberOfIdentities;

  Client({
    required this.clientId,
    required this.displayName,
    required this.defaultTier,
    required this.clientSecret,
    required this.createdAt,
    this.maxIdentities,
    this.numberOfIdentities,
  });

  factory Client.fromJson(dynamic json) => _$ClientFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$ClientToJson(this);
}

@JsonSerializable()
class Clients {
  final String clientId;
  final String displayName;
  final ClientDefaultTier defaultTier;
  final DateTime createdAt;
  final int? maxIdentities;
  final int? numberOfIdentities;

  Clients({
    required this.clientId,
    required this.displayName,
    required this.defaultTier,
    required this.createdAt,
    required this.maxIdentities,
    required this.numberOfIdentities,
  });

  factory Clients.fromJson(dynamic json) => _$ClientsFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$ClientsToJson(this);
}

@JsonSerializable()
class ClientDefaultTier {
  final String id;
  final String name;

  ClientDefaultTier({
    required this.id,
    required this.name,
  });

  factory ClientDefaultTier.fromJson(Map<String, dynamic> json) => _$ClientDefaultTierFromJson(json);
  Map<String, dynamic> toJson() => _$ClientDefaultTierToJson(this);
}

import 'package:json_annotation/json_annotation.dart';

import '../tiers/tier.dart';

part 'client_overview.g.dart';

@JsonSerializable()
class ClientOverview {
  final String clientId;
  final String displayName;
  final Tier defaultTier;
  final DateTime createdAt;
  final int? maxIdentities;
  final int numberOfIdentities;

  ClientOverview({
    required this.clientId,
    required this.displayName,
    required this.defaultTier,
    required this.createdAt,
    required this.maxIdentities,
    required this.numberOfIdentities,
  });

  factory ClientOverview.fromJson(dynamic json) => _$ClientOverviewFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$ClientOverviewToJson(this);
}

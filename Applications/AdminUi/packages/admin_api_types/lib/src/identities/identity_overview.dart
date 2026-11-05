import 'package:json_annotation/json_annotation.dart';

import '../tiers/tier.dart';

part 'identity_overview.g.dart';

@JsonSerializable()
class IdentityOverview {
  final String address;
  final DateTime createdAt;
  final String createdWithClient;
  final int identityVersion;
  final int numberOfDevices;
  final Tier tier;
  final DateTime? lastLoginAt;
  final int? datawalletVersion;

  new({
    required this.address,
    required this.createdAt,
    required this.createdWithClient,
    required this.identityVersion,
    required this.numberOfDevices,
    required this.tier,
    this.lastLoginAt,
    this.datawalletVersion,
  });

  factory fromJson(dynamic json) => _$IdentityOverviewFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$IdentityOverviewToJson(this);
}

import 'package:json_annotation/json_annotation.dart';

import 'identity_device.dart';
import 'identity_quota.dart';

part 'identity_response.g.dart';

@JsonSerializable()
class IdentityResponse {
  final String address;
  final String clientId;
  final String publicKey;
  final String tierId;
  final DateTime createdAt;
  final int identityVersion;
  final int numberOfDevices;
  final List<IdentityDevice>? devices;
  final List<IdentityQuota>? quotas;

  IdentityResponse({
    required this.address,
    required this.clientId,
    required this.publicKey,
    required this.tierId,
    required this.createdAt,
    required this.identityVersion,
    required this.numberOfDevices,
    this.devices,
    this.quotas,
  });

  factory IdentityResponse.fromJson(dynamic json) => _$IdentityResponseFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$IdentityResponseToJson(this);
}

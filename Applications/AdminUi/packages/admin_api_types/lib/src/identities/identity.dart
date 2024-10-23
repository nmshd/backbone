import 'package:json_annotation/json_annotation.dart';

import 'identity_device.dart';
import 'identity_quota.dart';

part 'identity.g.dart';

@JsonSerializable()
class Identity {
  final String address;
  final String clientId;
  final String publicKey;
  final String tierId;
  final DateTime createdAt;
  final int identityVersion;
  final int numberOfDevices;
  final List<IdentityDevice> devices;
  final List<IdentityQuota> quotas;

  Identity({
    required this.address,
    required this.clientId,
    required this.publicKey,
    required this.tierId,
    required this.createdAt,
    required this.identityVersion,
    required this.numberOfDevices,
    required this.devices,
    required this.quotas,
  });

  factory Identity.fromJson(dynamic json) => _$IdentityFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$IdentityToJson(this);
}

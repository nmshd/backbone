import 'package:json_annotation/json_annotation.dart';

part 'identity_device.g.dart';

@JsonSerializable()
class IdentityDevice {
  final String id;
  final String username;
  final DateTime createdAt;
  final String createdByDevice;
  final Map<String, dynamic>? lastLogin;
  final String communicationLanguage;

  IdentityDevice({
    required this.id,
    required this.username,
    required this.createdAt,
    required this.createdByDevice,
    required this.communicationLanguage,
    this.lastLogin,
  });

  factory IdentityDevice.fromJson(dynamic json) => _$IdentityDeviceFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$IdentityDeviceToJson(this);
}

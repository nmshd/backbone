import 'package:json_annotation/json_annotation.dart';

part 'identity_device.g.dart';

@JsonSerializable()
class IdentityDevice {
  final String id;
  final String username;
  final DateTime createdAt;
  final String createdByDevice;
  final LastLoginInformation? lastLogin;
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

@JsonSerializable()
class LastLoginInformation {
  final DateTime time;

  LastLoginInformation({required this.time});

  factory LastLoginInformation.fromJson(dynamic json) => _$LastLoginInformationFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$LastLoginInformationToJson(this);
}

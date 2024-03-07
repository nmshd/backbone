import 'package:admin_api_types/src/quotas/quota.dart';
import 'package:json_annotation/json_annotation.dart';

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
  final List<Device> devices;
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

@JsonSerializable()
class Device {
  final String id;
  final String username;
  final DateTime createdAt;
  final String createdByDevice;
  final LastLoginInformation lastLogin;

  Device({
    required this.id,
    required this.username,
    required this.createdAt,
    required this.createdByDevice,
    required this.lastLogin,
  });

  factory Device.fromJson(dynamic json) => _$DeviceFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$DeviceToJson(this);
}

@JsonSerializable()
class LastLoginInformation {
  final DateTime time;

  LastLoginInformation({
    required this.time,
  });

  factory LastLoginInformation.fromJson(dynamic json) => _$LastLoginInformationFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$LastLoginInformationToJson(this);
}

@JsonSerializable()
class IdentityQuota {
  final String id;
  final String source;
  final Metric metric;
  final int max;
  final int usage;
  final String period;

  IdentityQuota({
    required this.id,
    required this.source,
    required this.metric,
    required this.max,
    required this.usage,
    required this.period,
  });

  factory IdentityQuota.fromJson(dynamic json) => _$IdentityQuotaFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$IdentityQuotaToJson(this);
}

@JsonSerializable()
class IdentityOverviewFilter {
  final String? address;
  final List<String>? tiers;
  final List<String>? clients;
  final NumberFilter numberOfDevices;
  final DateFilter createdAt;
  final DateFilter lastLoginAt;
  final NumberFilter datawalletVersion;
  final NumberFilter identityVersion;

  IdentityOverviewFilter({
    required this.numberOfDevices,
    required this.createdAt,
    required this.lastLoginAt,
    required this.datawalletVersion,
    required this.identityVersion,
    this.address,
    this.tiers,
    this.clients,
  });

  factory IdentityOverviewFilter.fromJson(dynamic json) => _$IdentityOverviewFilterFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$IdentityOverviewFilterToJson(this);
}

@JsonSerializable()
class NumberFilter {
  final String? operator;
  final int? value;

  NumberFilter({
    this.operator,
    this.value,
  });

  factory NumberFilter.fromJson(dynamic json) => _$NumberFilterFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$NumberFilterToJson(this);
}

@JsonSerializable()
class DateFilter {
  final String? operator;
  final DateTime? value;

  DateFilter({
    this.operator,
    this.value,
  });

  factory DateFilter.fromJson(dynamic json) => _$DateFilterFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$DateFilterToJson(this);
}

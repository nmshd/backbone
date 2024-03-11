import 'package:admin_api_types/admin_api_types.dart';
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
  final List<Device>? devices;
  final List<IdentityQuota>? quotas;

  Identity({
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

  factory Identity.fromJson(dynamic json) => _$IdentityFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$IdentityToJson(this);
}

@JsonSerializable()
class Device {
  final String id;
  final String username;
  final DateTime createdAt;
  final String createdByDevice;
  final Map<String, dynamic>? lastLogin;

  Device({
    required this.id,
    required this.username,
    required this.createdAt,
    required this.createdByDevice,
    this.lastLogin,
  });

  factory Device.fromJson(dynamic json) => _$DeviceFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$DeviceToJson(this);
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
class IdentityOverview {
  @JsonKey(name: 'address')
  final String address;
  @JsonKey(name: 'createdAt')
  final DateTime createdAt;
  @JsonKey(name: 'lastLoginAt')
  final DateTime? lastLoginAt;
  @JsonKey(name: 'createdWithClient')
  final String createdWithClient;
  @JsonKey(name: 'datawalletVersion')
  final int? datawalletVersion;
  @JsonKey(name: 'identityVersion')
  final int identityVersion;
  @JsonKey(name: 'numberOfDevices')
  final int numberOfDevices;
  @JsonKey(name: 'tier')
  final Tier tier;

  IdentityOverview({
    required this.address,
    required this.createdAt,
    required this.createdWithClient,
    required this.identityVersion,
    required this.numberOfDevices,
    required this.tier,
    this.lastLoginAt,
    this.datawalletVersion,
  });

  factory IdentityOverview.fromJson(dynamic json) => _$IdentityOverviewFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$IdentityOverviewToJson(this);
}

@JsonSerializable()
class ODataResponse {
  @JsonKey(name: '@odata.context')
  final String odataContext;

  @JsonKey(name: '@odata.count')
  final int odataCount;

  @JsonKey(name: 'value')
  final List<IdentityOverview> identities;

  ODataResponse({
    required this.odataContext,
    required this.odataCount,
    required this.identities,
  });

  factory ODataResponse.fromJson(dynamic json) => _$ODataResponseFromJson(json as Map<String, dynamic>);

  Map<String, dynamic> toJson() => _$ODataResponseToJson(this);
}

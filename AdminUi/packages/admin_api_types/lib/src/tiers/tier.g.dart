// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'tier.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Tier _$TierFromJson(Map<String, dynamic> json) => Tier(
      id: json['id'] as String,
      name: json['name'] as String,
    );

Map<String, dynamic> _$TierToJson(Tier instance) => <String, dynamic>{
      'id': instance.id,
      'name': instance.name,
    };

TierDetails _$TierDetailsFromJson(Map<String, dynamic> json) => TierDetails(
      id: json['id'] as String,
      name: json['name'] as String,
      quotas: (json['quotas'] as List<dynamic>).map(Quota.fromJson).toList(),
    );

Map<String, dynamic> _$TierDetailsToJson(TierDetails instance) =>
    <String, dynamic>{
      'id': instance.id,
      'name': instance.name,
      'quotas': instance.quotas,
    };

TierOverview _$TierOverviewFromJson(Map<String, dynamic> json) => TierOverview(
      id: json['id'] as String,
      name: json['name'] as String,
      numberOfIdentities: json['numberOfIdentities'] as int,
      canBeUsedAsDefaultForClient: json['canBeUsedAsDefaultForClient'] as bool,
      canBeManuallyAssigned: json['canBeManuallyAssigned'] as bool,
    );

Map<String, dynamic> _$TierOverviewToJson(TierOverview instance) =>
    <String, dynamic>{
      'id': instance.id,
      'name': instance.name,
      'numberOfIdentities': instance.numberOfIdentities,
      'canBeUsedAsDefaultForClient': instance.canBeUsedAsDefaultForClient,
      'canBeManuallyAssigned': instance.canBeManuallyAssigned,
    };

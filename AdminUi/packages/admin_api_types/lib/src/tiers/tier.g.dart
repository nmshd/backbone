// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'tier.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Tiers _$TiersFromJson(Map<String, dynamic> json) => Tiers(
      id: json['id'] as String,
      name: json['name'] as String,
      canBeUsedAsDefaultForClient: json['canBeUsedAsDefaultForClient'] as bool,
      canBeManuallyAssigned: json['canBeManuallyAssigned'] as bool,
      numberOfIdentities: json['numberOfIdentities'] as int,
    );

Map<String, dynamic> _$TiersToJson(Tiers instance) => <String, dynamic>{
      'id': instance.id,
      'name': instance.name,
      'canBeUsedAsDefaultForClient': instance.canBeUsedAsDefaultForClient,
      'canBeManuallyAssigned': instance.canBeManuallyAssigned,
      'numberOfIdentities': instance.numberOfIdentities,
    };

Tier _$TierFromJson(Map<String, dynamic> json) => Tier(
      id: json['id'] as String,
      name: json['name'] as String,
    );

Map<String, dynamic> _$TierToJson(Tier instance) => <String, dynamic>{
      'id': instance.id,
      'name': instance.name,
    };

TierOverview _$TierOverviewFromJson(Map<String, dynamic> json) => TierOverview(
      id: json['id'] as String,
      name: json['name'] as String,
      quotas: (json['quotas'] as List<dynamic>).map(Quota.fromJson).toList(),
    );

Map<String, dynamic> _$TierOverviewToJson(TierOverview instance) => <String, dynamic>{
      'id': instance.id,
      'name': instance.name,
      'quotas': instance.quotas,
    };

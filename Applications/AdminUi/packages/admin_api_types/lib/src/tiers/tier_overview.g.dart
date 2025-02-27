// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'tier_overview.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

TierOverview _$TierOverviewFromJson(Map<String, dynamic> json) => TierOverview(
  id: json['id'] as String,
  name: json['name'] as String,
  numberOfIdentities: (json['numberOfIdentities'] as num).toInt(),
  canBeUsedAsDefaultForClient: json['canBeUsedAsDefaultForClient'] as bool,
  canBeManuallyAssigned: json['canBeManuallyAssigned'] as bool,
);

Map<String, dynamic> _$TierOverviewToJson(TierOverview instance) => <String, dynamic>{
  'id': instance.id,
  'name': instance.name,
  'numberOfIdentities': instance.numberOfIdentities,
  'canBeUsedAsDefaultForClient': instance.canBeUsedAsDefaultForClient,
  'canBeManuallyAssigned': instance.canBeManuallyAssigned,
};

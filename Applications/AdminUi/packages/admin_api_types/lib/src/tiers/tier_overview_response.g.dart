// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'tier_overview_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

TierOverviewResponse _$TierOverviewResponseFromJson(Map<String, dynamic> json) => TierOverviewResponse(
      id: json['id'] as String,
      name: json['name'] as String,
      numberOfIdentities: (json['numberOfIdentities'] as num).toInt(),
      canBeUsedAsDefaultForClient: json['canBeUsedAsDefaultForClient'] as bool,
      canBeManuallyAssigned: json['canBeManuallyAssigned'] as bool,
    );

Map<String, dynamic> _$TierOverviewResponseToJson(TierOverviewResponse instance) => <String, dynamic>{
      'id': instance.id,
      'name': instance.name,
      'numberOfIdentities': instance.numberOfIdentities,
      'canBeUsedAsDefaultForClient': instance.canBeUsedAsDefaultForClient,
      'canBeManuallyAssigned': instance.canBeManuallyAssigned,
    };

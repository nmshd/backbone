// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'relationship.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Relationship _$RelationshipFromJson(Map<String, dynamic> json) => Relationship(
  peer: json['peer'] as String,
  requestedBy: json['requestedBy'] as String,
  templateId: json['templateId'] as String?,
  status: json['status'] as String,
  creationDate: DateTime.parse(json['creationDate'] as String),
  answeredAt: json['answeredAt'] == null ? null : DateTime.parse(json['answeredAt'] as String),
  createdByDevice: json['createdByDevice'] as String,
  answeredByDevice: json['answeredByDevice'] as String?,
);

Map<String, dynamic> _$RelationshipToJson(Relationship instance) => <String, dynamic>{
  'peer': instance.peer,
  'requestedBy': instance.requestedBy,
  'templateId': instance.templateId,
  'status': instance.status,
  'creationDate': instance.creationDate.toIso8601String(),
  'answeredAt': instance.answeredAt?.toIso8601String(),
  'createdByDevice': instance.createdByDevice,
  'answeredByDevice': instance.answeredByDevice,
};

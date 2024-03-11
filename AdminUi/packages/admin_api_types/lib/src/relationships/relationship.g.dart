// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'relationship.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PagedHttpResponseEnvelope _$PagedHttpResponseEnvelopeFromJson(
        Map<String, dynamic> json) =>
    PagedHttpResponseEnvelope(
      result: (json['result'] as List<dynamic>)
          .map((e) => Relationship.fromJson(e as Map<String, dynamic>))
          .toList(),
      pagination:
          PaginationData.fromJson(json['pagination'] as Map<String, dynamic>),
    );

Map<String, dynamic> _$PagedHttpResponseEnvelopeToJson(
        PagedHttpResponseEnvelope instance) =>
    <String, dynamic>{
      'result': instance.result,
      'pagination': instance.pagination,
    };

PaginationData _$PaginationDataFromJson(Map<String, dynamic> json) =>
    PaginationData(
      pageNumber: json['pageNumber'] as int?,
      pageSize: json['pageSize'] as int?,
      totalPages: json['totalPages'] as int?,
      totalRecords: json['totalRecords'] as int?,
    );

Map<String, dynamic> _$PaginationDataToJson(PaginationData instance) =>
    <String, dynamic>{
      'pageNumber': instance.pageNumber,
      'pageSize': instance.pageSize,
      'totalPages': instance.totalPages,
      'totalRecords': instance.totalRecords,
    };

Relationship _$RelationshipFromJson(Map<String, dynamic> json) => Relationship(
      peer: json['peer'] as String,
      requestedBy: json['requestedBy'] as String,
      templateId: json['templateId'] as String,
      status: json['status'] as String,
      creationDate: DateTime.parse(json['creationDate'] as String),
      answeredAt: DateTime.parse(json['answeredAt'] as String),
      createdByDevice: json['createdByDevice'] as String,
      answeredByDevice: json['answeredByDevice'] as String,
    );

Map<String, dynamic> _$RelationshipToJson(Relationship instance) =>
    <String, dynamic>{
      'peer': instance.peer,
      'requestedBy': instance.requestedBy,
      'templateId': instance.templateId,
      'status': instance.status,
      'creationDate': instance.creationDate.toIso8601String(),
      'answeredAt': instance.answeredAt.toIso8601String(),
      'createdByDevice': instance.createdByDevice,
      'answeredByDevice': instance.answeredByDevice,
    };

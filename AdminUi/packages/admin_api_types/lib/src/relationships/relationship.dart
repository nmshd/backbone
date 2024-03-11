import 'package:json_annotation/json_annotation.dart';

part 'relationship.g.dart';

@JsonSerializable()
class PagedHttpResponseEnvelope {
  final List<Relationship> result;
  final PaginationData pagination;

  PagedHttpResponseEnvelope({
    required this.result,
    required this.pagination,
  });

  factory PagedHttpResponseEnvelope.fromJson(dynamic json) => _$PagedHttpResponseEnvelopeFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$PagedHttpResponseEnvelopeToJson(this);
}

@JsonSerializable()
class PaginationData {
  final int? pageNumber;
  final int? pageSize;
  final int? totalPages;
  final int? totalRecords;

  PaginationData({
    this.pageNumber,
    this.pageSize,
    this.totalPages,
    this.totalRecords,
  });

  factory PaginationData.fromJson(dynamic json) => _$PaginationDataFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$PaginationDataToJson(this);
}

@JsonSerializable()
class Relationship {
  final String peer;
  final String requestedBy;
  final String templateId;
  final String status;
  final DateTime creationDate;
  final DateTime answeredAt;
  final String createdByDevice;
  final String answeredByDevice;

  Relationship({
    required this.peer,
    required this.requestedBy,
    required this.templateId,
    required this.status,
    required this.creationDate,
    required this.answeredAt,
    required this.createdByDevice,
    required this.answeredByDevice,
  });

  factory Relationship.fromJson(dynamic json) => _$RelationshipFromJson(json as Map<String, dynamic>);
  Map<String, dynamic> toJson() => _$RelationshipToJson(this);
}

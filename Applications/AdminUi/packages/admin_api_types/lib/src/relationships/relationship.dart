import 'package:json_annotation/json_annotation.dart';

part 'relationship.g.dart';

@JsonSerializable()
class Relationship {
  final String peer;
  final String requestedBy;
  final String? templateId;
  final String status;
  final DateTime creationDate;
  final DateTime? answeredAt;
  final String createdByDevice;
  final String? answeredByDevice;

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

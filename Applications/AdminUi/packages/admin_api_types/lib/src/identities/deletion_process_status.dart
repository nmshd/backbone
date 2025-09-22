import 'package:json_annotation/json_annotation.dart';

enum DeletionProcessStatus {
  @JsonValue('Active')
  active,

  @JsonValue('Cancelled')
  cancelled,

  @JsonValue('Deleting')
  deleting,
}

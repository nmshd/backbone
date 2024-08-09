import 'package:json_annotation/json_annotation.dart';

enum DeletionProcessStatus {
  @JsonValue('WaitingForApproval')
  waitingForApproval,

  @JsonValue('Approved')
  approved,

  @JsonValue('Cancelled')
  cancelled,

  @JsonValue('Rejected')
  rejected,

  @JsonValue('Deleting')
  deleting,
}

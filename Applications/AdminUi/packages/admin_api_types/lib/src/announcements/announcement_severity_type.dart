enum AnnouncementSeverity {
  low,
  medium,
  high
  ;

  String get name => switch (this) {
    .low => 'Low',
    .medium => 'Medium',
    .high => 'High',
  };
}

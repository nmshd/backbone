enum AnnouncementSeverityType {
  low,
  medium,
  high;

  String get name =>
      switch (this) { AnnouncementSeverityType.low => 'Low', AnnouncementSeverityType.medium => 'Medium', AnnouncementSeverityType.high => 'High' };
}

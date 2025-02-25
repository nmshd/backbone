enum AnnouncementSeverity {
  low,
  medium,
  high;

  String get name =>
      switch (this) { AnnouncementSeverity.low => 'Low', AnnouncementSeverity.medium => 'Medium', AnnouncementSeverity.high => 'High' };
}

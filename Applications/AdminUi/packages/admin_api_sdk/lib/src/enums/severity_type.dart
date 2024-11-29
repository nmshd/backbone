enum SeverityType {
  low,
  medium,
  high;

  String get name => switch (this) { SeverityType.low => 'Low', SeverityType.medium => 'Medium', SeverityType.high => 'High' };
}

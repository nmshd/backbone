enum OptionalLanguageType {
  german,
  portuguese,
  italian;

  String get name => switch (this) {
        OptionalLanguageType.german => 'German',
        OptionalLanguageType.portuguese => 'Portuguese',
        OptionalLanguageType.italian => 'Italian',
      };
}

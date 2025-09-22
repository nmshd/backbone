class ApiError {
  final String id;
  final String docs;
  final String time;
  final String code;
  final String message;

  ApiError({required this.id, required this.docs, required this.time, required this.code, required this.message});

  factory ApiError.fromJson(Map<String, dynamic> json) {
    return ApiError(
      id: json['id'] as String,
      docs: json['docs'] as String,
      time: json['time'] as String,
      code: json['code'] as String,
      message: json['message'] as String,
    );
  }

  @override
  String toString() => 'ApiError{id: $id, docs: $docs, time: $time, code: $code, message: $message}';
}

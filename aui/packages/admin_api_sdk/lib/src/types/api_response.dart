import 'api_error.dart';

class ApiResponse<T> {
  final T? _data;
  final ApiError? _error;

  T get data {
    if (_data == null) {
      throw Exception('${error.code}: ${error.message}');
    }

    return _data;
  }

  ApiError get error {
    if (_error == null) {
      throw Exception('No error');
    }

    return _error;
  }

  ApiResponse._(this._data, this._error);

  factory ApiResponse.success(T data) {
    return ApiResponse._(data, null);
  }

  factory ApiResponse.fromError(ApiError error) {
    return ApiResponse._(
      null,
      error,
    );
  }

  bool get hasError => _error != null;
  bool get hasData => _data != null;
}

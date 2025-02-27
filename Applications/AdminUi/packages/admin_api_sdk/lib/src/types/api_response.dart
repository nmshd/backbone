import 'api_error.dart';

class ApiResponse<T> {
  final T? _data;
  final ApiError? _error;
  final Pagination? _pagination;

  bool get isPaged => _pagination != null;
  Pagination get pagination {
    if (!isPaged) throw Exception('No pagination, check isPaged first');
    return _pagination!;
  }

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

  ApiResponse._(this._data, this._error, this._pagination);

  factory ApiResponse.success(T data, [Pagination? pagination]) => ApiResponse._(data, null, pagination);
  factory ApiResponse.fromError(ApiError error) => ApiResponse._(null, error, null);

  bool get hasError => _error != null;
  bool get hasData => _data != null;
}

class Pagination {
  final int pageNumber;
  final int pageSize;
  final int totalPages;
  final int totalRecords;

  Pagination({required this.pageNumber, required this.pageSize, required this.totalPages, required this.totalRecords});

  factory Pagination.fromJson(Map<String, dynamic> json) {
    return Pagination(
      pageNumber: (json['pageNumber'] as num).toInt(),
      pageSize: (json['pageSize'] as num).toInt(),
      totalPages: (json['totalPages'] as num).toInt(),
      totalRecords: (json['totalRecords'] as num).toInt(),
    );
  }

  @override
  String toString() {
    return 'Pagination{pageNumber: $pageNumber, pageSize: $pageSize, totalPages: $totalPages, totalRecords: $totalRecords}';
  }
}

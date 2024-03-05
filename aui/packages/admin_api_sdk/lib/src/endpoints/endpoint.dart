import 'package:dio/dio.dart';
import 'package:meta/meta.dart';

import '../types/types.dart';

abstract class Endpoint {
  final Dio _dio;

  Endpoint(this._dio);

  @protected
  Future<T> getPlain<T>(String path) async {
    final response = await _dio.get<T>(path);
    if (response.data == null) {
      throw Exception('Invalid response type');
    }

    return response.data!;
  }

  @protected
  Future<ApiResponse<T>> get<T>(String path, {required T Function(dynamic) transformer, Map<String, dynamic>? query}) async {
    final response = await _dio.get<Map<String, dynamic>>(
      path,
      queryParameters: query,
      options: Options(headers: {'Accept': 'application/json'}),
    );
    return _makeResult(response, transformer);
  }

  @protected
  Future<ApiResponse<T>> post<T>(
    String path, {
    required T Function(dynamic) transformer,
    Object? data,
    int? expectedStatus,
    Map<String, dynamic>? params,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(path, data: data, queryParameters: params);
    return _makeResult(response, transformer, expectedStatus: expectedStatus);
  }

  @protected
  Future<ApiResponse<T>> put<T>(String path, {required T Function(dynamic) transformer, Object? data}) async {
    final response = await _dio.put<Map<String, dynamic>>(path, data: data);
    return _makeResult(response, transformer);
  }

  @protected
  Future<ApiResponse<T>> patch<T>(String path, {required T Function(dynamic) transformer, Object? data}) async {
    final response = await _dio.patch<Map<String, dynamic>>(path, data: data);
    return _makeResult(response, transformer);
  }

  @protected
  Future<ApiResponse<T>> delete<T>(
    String path, {
    required T Function(dynamic) transformer,
    required int expectedStatus,
    bool allowEmptyResponse = false,
  }) async {
    final response = await _dio.delete<Map<String, dynamic>>(path);
    return _makeResult(response, transformer, expectedStatus: expectedStatus, allowEmptyResponse: allowEmptyResponse);
  }

  ApiResponse<T> _makeResult<T>(
    Response<Map<String, dynamic>> httpResponse,
    T Function(dynamic) transformer, {
    int? expectedStatus,
    bool allowEmptyResponse = false,
  }) {
    expectedStatus ??= switch (httpResponse.requestOptions.method.toUpperCase()) { 'POST' => 201, _ => 200 };

    final payload = httpResponse.data;

    if (httpResponse.statusCode != expectedStatus) {
      if (payload == null) {
        throw Exception('Invalid response type');
      }

      final errorPayload = payload['error'] as Map<String, dynamic>;
      return ApiResponse.fromError(ApiError.fromJson(errorPayload));
    }

    if (payload == null) {
      if (allowEmptyResponse) {
        return ApiResponse.success(transformer(null));
      }

      throw Exception('Invalid response type');
    }
    return ApiResponse.success(transformer(payload['result']));
  }
}

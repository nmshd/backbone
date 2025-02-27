import 'package:admin_api_sdk/src/endpoints/announcements_endpoint.dart';
import 'package:admin_api_sdk/src/endpoints/metrics_endpoint.dart';
import 'package:dio/dio.dart';

import 'endpoints/endpoints.dart';

class AdminApiClient {
  late final Dio _dio;

  late final ClientsEndpoint clients;
  late final TiersEndpoint tiers;
  late final QuotasEndpoint quotas;
  late final IdentitiesEndpoint identities;
  late final RelationshipsEndpoint relationships;
  late final MessagesEndpoint messages;
  late final MetricsEndpoint metrics;
  late final AnnouncementsEndpoint announcements;

  AdminApiClient._(String baseUrl, String apiKey) {
    final dio = Dio(BaseOptions(baseUrl: baseUrl, headers: {'X-API-KEY': apiKey}, validateStatus: (_) => true));
    _dio = dio;

    clients = ClientsEndpoint(dio);
    tiers = TiersEndpoint(dio);
    quotas = QuotasEndpoint(dio);
    identities = IdentitiesEndpoint(dio);
    relationships = RelationshipsEndpoint(dio);
    messages = MessagesEndpoint(dio);
    metrics = MetricsEndpoint(dio);
    announcements = AnnouncementsEndpoint(dio);
  }

  static Future<AdminApiClient> create({required String baseUrl, required String apiKey}) async {
    final client = AdminApiClient._(baseUrl, apiKey);
    await client._setupXsrf();
    return client;
  }

  Future<void> _setupXsrf() async {
    final xsrf = await _dio.get<String>('/api/v1/xsrf');
    final xsrfToken = xsrf.data!;
    final xsrfCookie = xsrf.headers.value('Set-Cookie');

    _dio.options.headers['X-XSRF-TOKEN'] = xsrfToken;
    _dio.options.headers['Cookie'] = xsrfCookie;
  }

  static Future<bool> validateApiKey({required String baseUrl, required String apiKey}) async {
    final isValidResponse = await Dio(
      BaseOptions(baseUrl: baseUrl, validateStatus: (status) => status == 200),
    ).post<Map<String, dynamic>>('/api/v1/validateApiKey', data: {'apiKey': apiKey});

    final isValid = isValidResponse.data!['isValid'] as bool;
    return isValid;
  }
}

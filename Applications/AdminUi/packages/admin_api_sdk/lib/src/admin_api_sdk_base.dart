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
    await AdminApiClient._setupXsrf(client._dio);
    return client;
  }

  static Future<void> _setupXsrf(Dio dio) async {
    final xsrf = await dio.get<String>('/api/v2/xsrf');
    final xsrfToken = xsrf.data!;
    final xsrfCookie = xsrf.headers.value('Set-Cookie');

    dio.options.headers['X-XSRF-TOKEN'] = xsrfToken;
    dio.options.headers['Cookie'] = xsrfCookie;
  }

  static Future<bool> validateApiKey({required String baseUrl, required String apiKey}) async {
    final dio = Dio(BaseOptions(baseUrl: baseUrl, validateStatus: (status) => status == 200));

    await AdminApiClient._setupXsrf(dio);

    final isValidResponse = await dio.post<Map<String, dynamic>>('/api/v2/validateApiKey', data: {'apiKey': apiKey});

    final isValid = isValidResponse.data!['isValid'] as bool;
    return isValid;
  }
}

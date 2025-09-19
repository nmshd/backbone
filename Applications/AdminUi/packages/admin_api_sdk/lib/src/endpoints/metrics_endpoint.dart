import 'package:admin_api_types/admin_api_types.dart';

import '../types/types.dart';
import 'endpoint.dart';

class MetricsEndpoint extends Endpoint {
  MetricsEndpoint(super._dio);

  Future<ApiResponse<List<Metric>>> getMetrics() => get('/api/v2/Metrics', transformer: (e) => (e as List).map(Metric.fromJson).toList());
}

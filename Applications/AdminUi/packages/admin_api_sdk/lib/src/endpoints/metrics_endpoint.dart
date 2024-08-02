import 'package:admin_api_types/admin_api_types.dart';

import '../types/types.dart';
import 'endpoint.dart';

class MetricsEndpoint extends Endpoint {
  MetricsEndpoint(super._dio);

  Future<ApiResponse<List<MetricResponse>>> getMetrics() => get(
        '/api/v1/Metrics',
        transformer: (e) => (e as List).map(MetricResponse.fromJson).toList(),
      );
}

export interface RouteMetrics {
    [route: string]: {
        [metricType: string]: {
            totalRequests: number;
            totalTime: number;
            statuses: {
                [statusCode: number]: number;
            };
        };
    };
}

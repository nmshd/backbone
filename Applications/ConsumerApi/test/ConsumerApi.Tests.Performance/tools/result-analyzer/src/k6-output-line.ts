export interface k6OutputLine {
    metric_name: string;
    timestamp: string;
    metric_value: string;
    check: string;
    error: string;
    error_code: string;
    expected_response: string;
    group: string;
    method: string;
    name: string;
    proto: string;
    scenario: string;
    service: string;
    status: string;
    subproto: string;
    tls_version: string;
    url: string;
    extra_tags: string;
    metadata: string;
}

export interface HttpErrorResponseWrapper {
    error: HttpErrorResponse;
}

interface HttpErrorResponse {
    error: Error;
}

interface Error {
    id: string;
    code: string;
    message: string;
    time: string;
    data: any;
}

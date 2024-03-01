import { HttpErrorResponse } from "@angular/common/http";

export interface HttpErrorResponseWrapper extends HttpErrorResponse {
    error: {
        error: {
            id: string;
            code: string;
            message: string;
            time: string;
            data: any;
        };
    };
}

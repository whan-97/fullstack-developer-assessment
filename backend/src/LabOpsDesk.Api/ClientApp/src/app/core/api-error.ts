import { HttpErrorResponse } from '@angular/common/http';
import { ApiError } from './models';

export function readApiError(error: unknown): ApiError {
  if (error instanceof HttpErrorResponse) {
    const body = error.error as ApiError | string | null;
    if (body && typeof body === 'object' && 'message' in body) {
      return body;
    }

    return {
      code: `http_${error.status}`,
      message: error.message || `Request failed with status ${error.status}`,
    };
  }

  return {
    code: 'unknown',
    message: 'Unexpected client error',
  };
}

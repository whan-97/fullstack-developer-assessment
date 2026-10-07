import { HttpErrorResponse } from '@angular/common/http';
import { ApiError } from './models';

const DEFAULT_MESSAGES: Record<number, string> = {
  400: 'The request was invalid. Please check your input.',
  409: 'The request conflicts with the current state.',
  500: 'The server encountered an error. Please try again later.',
};

export function readApiError(error: unknown): ApiError {
  if (error instanceof HttpErrorResponse) {
    const { status, error: body, message } = error;

    if (body && typeof body === 'object' && typeof body.message === 'string') {
      return body as ApiError;
    }

    return {
      code: `http_${status}`,
      message: DEFAULT_MESSAGES[status] ?? (typeof body === 'string' ? body : message || `Request failed with status ${status}`),
    };
  }

  return { code: 'unknown', message: 'Unexpected client error' };
}

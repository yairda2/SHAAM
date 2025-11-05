import { Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';

/**
 * Centralized error handling service
 * Converts API errors to user-friendly messages
 */
@Injectable({
  providedIn: 'root'
})
export class ErrorHandlerService {
  constructor() {}

  /**
   * Handles HTTP errors and returns user-friendly error messages
   * Discriminates between client-side and server-side errors
   */
  handleError(error: HttpErrorResponse): string {
    let errorMessage = 'An unexpected error occurred';

    if (error.error instanceof ErrorEvent) {
      // Client-side or network error
      errorMessage = `Network error: ${error.error.message}`;
      console.error('Client-side error:', error.error.message);
    } else {
      // Server-side error
      if (error.status === 0) {
        // Connection refused or network timeout
        errorMessage = 'Unable to connect to the server. Please check your connection.';
      } else if (error.status === 400) {
        // Bad request - validation errors
        errorMessage = error.error?.message || 'Invalid request. Please check your input.';
      } else if (error.status === 404) {
        // Not found
        errorMessage = error.error?.message || 'The requested resource was not found.';
      } else if (error.status === 500) {
        // Internal server error
        errorMessage = error.error?.message || 'A server error occurred. Please try again later.';
      } else {
        // Other HTTP errors
        errorMessage = error.error?.message || `Error: ${error.statusText}`;
      }

      console.error(
        `Server error: ${error.status}\n` +
        `Message: ${error.message}\n` +
        `Status Text: ${error.statusText}`
      );
    }

    return errorMessage;
  }

  /**
   * Extracts validation errors from API response
   * Returns array of error messages for display
   */
  extractValidationErrors(error: HttpErrorResponse): string[] {
    if (error.error?.errors && Array.isArray(error.error.errors)) {
      return error.error.errors;
    }

    if (error.error?.message) {
      return [error.error.message];
    }

    return ['Validation failed. Please check your input.'];
  }

  /**
   * Logs errors for debugging purposes
   * Can be extended to send errors to remote logging service
   */
  logError(error: any, context?: string): void {
    const timestamp = new Date().toISOString();
    const contextInfo = context ? `[${context}]` : '';

    console.error(`${timestamp} ${contextInfo} Error:`, error);

    // In production, could send to remote logging service
    // Example: this.loggingService.logError(error, context);
  }
}

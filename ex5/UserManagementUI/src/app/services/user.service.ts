import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, map, retry, tap } from 'rxjs/operators';
import { User, CreateUserRequest, UpdateUserRequest, ApiResponse } from '../models/user.model';
import { environment } from '../../environments/environment';
import { ErrorHandlerService } from './error-handler.service';

/**
 * Service for managing user data and API communication
 * Implements observable pattern for reactive data flow
 * Provides caching and state management for user data
 */
@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = `${environment.apiUrl}/users`;

  // BehaviorSubject for shared user state across components
  private usersSubject = new BehaviorSubject<User[]>([]);

  // Public observable for components to subscribe to
  public users$ = this.usersSubject.asObservable();

  constructor(
    private http: HttpClient,
    private errorHandler: ErrorHandlerService
  ) {
    // Initialize by loading users on service creation
    this.loadUsers();
  }

  /**
   * Loads all users from the API and updates the shared state
   * Automatically called on service initialization
   */
  private loadUsers(): void {
    this.getUsers().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.usersSubject.next(response.data);
        }
      },
      error: (error) => {
        this.errorHandler.logError(error, 'UserService.loadUsers');
      }
    });
  }

  /**
   * Retrieves all users from the API
   * Implements retry logic for failed requests
   */
  getUsers(): Observable<ApiResponse<User[]>> {
    return this.http.get<ApiResponse<User[]>>(this.apiUrl).pipe(
      retry(2), // Retry failed requests twice
      catchError(this.handleError.bind(this))
    );
  }

  /**
   * Retrieves a specific user by ID
   */
  getUserById(id: number): Observable<ApiResponse<User>> {
    return this.http.get<ApiResponse<User>>(`${this.apiUrl}/${id}`).pipe(
      catchError(this.handleError.bind(this))
    );
  }

  /**
   * Creates a new user
   * Updates the shared state on success
   */
  createUser(user: CreateUserRequest): Observable<ApiResponse<User>> {
    return this.http.post<ApiResponse<User>>(this.apiUrl, user).pipe(
      tap((response) => {
        // Update shared state on successful creation
        if (response.success && response.data) {
          const currentUsers = this.usersSubject.value;
          this.usersSubject.next([...currentUsers, response.data]);
        }
      }),
      catchError(this.handleError.bind(this))
    );
  }

  /**
   * Updates an existing user
   * Updates the shared state on success
   */
  updateUser(id: number, user: UpdateUserRequest): Observable<ApiResponse<User>> {
    return this.http.put<ApiResponse<User>>(`${this.apiUrl}/${id}`, user).pipe(
      tap((response) => {
        // Update shared state on successful update
        if (response.success && response.data) {
          const currentUsers = this.usersSubject.value;
          const updatedUsers = currentUsers.map(u =>
            u.id === id ? response.data! : u
          );
          this.usersSubject.next(updatedUsers);
        }
      }),
      catchError(this.handleError.bind(this))
    );
  }

  /**
   * Deletes a user by ID
   * Updates the shared state on success
   */
  deleteUser(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.apiUrl}/${id}`).pipe(
      tap((response) => {
        // Update shared state on successful deletion
        if (response.success) {
          const currentUsers = this.usersSubject.value;
          this.usersSubject.next(currentUsers.filter(u => u.id !== id));
        }
      }),
      catchError(this.handleError.bind(this))
    );
  }

  /**
   * Searches for users matching a search term
   * Case-insensitive search across name, email, and company fields
   */
  searchUsers(searchTerm: string): Observable<ApiResponse<User[]>> {
    const url = `${this.apiUrl}/search?searchTerm=${encodeURIComponent(searchTerm)}`;
    return this.http.get<ApiResponse<User[]>>(url).pipe(
      catchError(this.handleError.bind(this))
    );
  }

  /**
   * Refreshes the user list from the API
   * Useful after operations that may have changed the data
   */
  refreshUsers(): void {
    this.loadUsers();
  }

  /**
   * Handles HTTP errors and returns user-friendly error messages
   * Delegates to ErrorHandlerService for centralized error handling
   */
  private handleError(error: HttpErrorResponse): Observable<never> {
    const errorMessage = this.errorHandler.handleError(error);
    return throwError(() => new Error(errorMessage));
  }
}

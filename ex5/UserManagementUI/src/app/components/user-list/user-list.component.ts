import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { User } from '../../models/user.model';
import { UserService } from '../../services/user.service';
import { UserFormComponent } from '../user-form/user-form.component';
import { UserDetailComponent } from '../user-detail/user-detail.component';

/**
 * Smart component managing user list display and operations
 * Handles user selection, search, and CRUD operations
 * Implements proper subscription management to prevent memory leaks
 */
@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [CommonModule, FormsModule, UserFormComponent, UserDetailComponent],
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss']
})
export class UserListComponent implements OnInit, OnDestroy {
  users: User[] = [];
  filteredUsers: User[] = [];
  selectedUser: User | null = null;
  searchTerm: string = '';
  isLoading: boolean = false;
  errorMessage: string = '';
  successMessage: string = '';
  showForm: boolean = false;
  isEditMode: boolean = false;

  // Subject for managing subscriptions to prevent memory leaks
  private destroy$ = new Subject<void>();

  // Subject for debouncing search input
  private searchSubject = new Subject<string>();

  constructor(private userService: UserService) {}

  /**
   * Component initialization lifecycle hook
   * Sets up subscriptions and loads initial data
   */
  ngOnInit(): void {
    this.loadUsers();
    this.setupSearchDebounce();
  }

  /**
   * Component cleanup lifecycle hook
   * Unsubscribes from all observables to prevent memory leaks
   */
  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Loads all users from the service
   * Subscribes to the users observable for reactive updates
   */
  private loadUsers(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.userService.users$
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (users) => {
          this.users = users;
          this.filteredUsers = users;
          this.isLoading = false;
        },
        error: (error) => {
          this.errorMessage = error.message;
          this.isLoading = false;
        }
      });
  }

  /**
   * Sets up debounced search functionality
   * Waits 300ms after user stops typing before executing search
   */
  private setupSearchDebounce(): void {
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe(searchTerm => {
      this.performSearch(searchTerm);
    });
  }

  /**
   * Triggers search when user types in search box
   * Debounced to avoid excessive API calls
   */
  onSearchChange(searchTerm: string): void {
    this.searchSubject.next(searchTerm);
  }

  /**
   * Performs the actual search operation
   * Filters users locally or calls API based on implementation
   */
  private performSearch(searchTerm: string): void {
    if (!searchTerm.trim()) {
      this.filteredUsers = this.users;
      return;
    }

    const lowerSearchTerm = searchTerm.toLowerCase();
    this.filteredUsers = this.users.filter(user =>
      user.name.toLowerCase().includes(lowerSearchTerm) ||
      user.email.toLowerCase().includes(lowerSearchTerm) ||
      (user.company && user.company.toLowerCase().includes(lowerSearchTerm))
    );
  }

  /**
   * Handles user selection for viewing details
   */
  onSelectUser(user: User): void {
    this.selectedUser = user;
    this.showForm = false;
  }

  /**
   * Opens form for creating a new user
   */
  onAddUser(): void {
    this.selectedUser = null;
    this.isEditMode = false;
    this.showForm = true;
    this.clearMessages();
  }

  /**
   * Opens form for editing an existing user
   */
  onEditUser(user: User): void {
    this.selectedUser = user;
    this.isEditMode = true;
    this.showForm = true;
    this.clearMessages();
  }

  /**
   * Handles user deletion with confirmation
   * Improved error handling displays specific error messages from API response
   */
  onDeleteUser(user: User): void {
    if (confirm(`Are you sure you want to delete ${user.name}?`)) {
      this.isLoading = true;
      this.errorMessage = '';

      this.userService.deleteUser(user.id)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (response) => {
            // Handle successful response
            if (response.success) {
              this.successMessage = `User ${user.name} deleted successfully`;
              this.selectedUser = null;
              this.isLoading = false;
              this.clearMessagesAfterDelay();
            } else if (response.errors && response.errors.length > 0) {
              // Display first error from errors array for clear user feedback
              this.errorMessage = response.errors[0];
              this.isLoading = false;
            } else {
              // Fallback error message if no specific errors provided
              this.errorMessage = response.message || 'Failed to delete user';
              this.isLoading = false;
            }
          },
          error: (error) => {
            // Network or unexpected errors with user-friendly message
            this.errorMessage = error.message || 'Failed to delete user. Please try again.';
            this.isLoading = false;
          }
        });
    }
  }

  /**
   * Handles successful form submission (create or update)
   */
  onFormSubmit(success: boolean): void {
    if (success) {
      this.showForm = false;
      this.selectedUser = null;
      this.successMessage = this.isEditMode ? 'User updated successfully' : 'User created successfully';
      this.clearMessagesAfterDelay();
    }
  }

  /**
   * Handles form cancellation
   */
  onFormCancel(): void {
    this.showForm = false;
    this.selectedUser = null;
    this.clearMessages();
  }

  /**
   * Clears all success and error messages
   */
  private clearMessages(): void {
    this.successMessage = '';
    this.errorMessage = '';
  }

  /**
   * Clears messages after 5 seconds
   */
  private clearMessagesAfterDelay(): void {
    setTimeout(() => {
      this.clearMessages();
    }, 5000);
  }

  /**
   * Tracks users in ngFor for performance optimization
   */
  trackByUserId(index: number, user: User): number {
    return user.id;
  }
}

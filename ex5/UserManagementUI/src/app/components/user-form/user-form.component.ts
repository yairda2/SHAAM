import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { User, CreateUserRequest, UpdateUserRequest } from '../../models/user.model';
import { UserService } from '../../services/user.service';

/**
 * Form component for creating and editing users
 * Implements reactive forms with validation
 * Emits events for parent component communication
 */
@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './user-form.component.html',
  styleUrls: ['./user-form.component.scss']
})
export class UserFormComponent implements OnInit {
  @Input() user: User | null = null;
  @Input() isEditMode: boolean = false;
  @Output() formSubmit = new EventEmitter<boolean>();
  @Output() formCancel = new EventEmitter<void>();

  userForm!: FormGroup;
  isSubmitting: boolean = false;
  errorMessage: string = '';

  constructor(
    private fb: FormBuilder,
    private userService: UserService
  ) {}

  /**
   * Component initialization lifecycle hook
   * Sets up form with validation rules and populates for edit mode
   */
  ngOnInit(): void {
    this.initializeForm();

    // Populate form with user data for edit mode
    if (this.isEditMode && this.user) {
      this.userForm.patchValue({
        name: this.user.name,
        email: this.user.email,
        phone: this.user.phone || '',
        website: this.user.website || '',
        company: this.user.company || ''
      });
    }
  }

  /**
   * Initializes the form with validation rules
   * Name and email are required, other fields are optional
   */
  private initializeForm(): void {
    this.userForm = this.fb.group({
      name: ['', [
        Validators.required,
        Validators.minLength(2),
        Validators.maxLength(100)
      ]],
      email: ['', [
        Validators.required,
        Validators.email
      ]],
      phone: ['', [
        Validators.maxLength(20)
      ]],
      website: ['', [
        Validators.maxLength(255)
      ]],
      company: ['', [
        Validators.maxLength(255)
      ]]
    });
  }

  /**
   * Handles form submission
   * Validates form and calls appropriate service method
   */
  onSubmit(): void {
    // Mark all fields as touched to display validation errors
    this.markFormGroupTouched(this.userForm);

    if (this.userForm.invalid) {
      this.errorMessage = 'Please correct the errors in the form';
      return;
    }

    const formValue = this.userForm.value;

    // Check for duplicate email in existing users
    // This provides client-side validation before making the API call
    // Case-insensitive matching ensures emails like 'test@email.com' and 'TEST@email.com' are treated as duplicates
    // Backend will also validate to ensure data integrity
    let users: any[] = [];
    this.userService.users$.subscribe(u => users = u).unsubscribe();

    const emailLower = formValue.email.toLowerCase();
    const duplicateUser = users.find(u => {
      // When editing, exclude the current user from duplicate check to allow keeping the same email
      const isDifferentUser = !this.isEditMode || u.id !== this.user?.id;
      return isDifferentUser && u.email.toLowerCase() === emailLower;
    });

    if (duplicateUser) {
      this.errorMessage = 'This email already exists';
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';

    if (this.isEditMode && this.user) {
      this.updateUser(formValue);
    } else {
      this.createUser(formValue);
    }
  }

  /**
   * Creates a new user via the service
   * Improved error handling displays specific error messages from API response
   */
  private createUser(userData: CreateUserRequest): void {
    this.userService.createUser(userData).subscribe({
      next: (response) => {
        // Handle successful response
        if (response.success) {
          this.isSubmitting = false;
          this.formSubmit.emit(true);
        } else if (response.errors && response.errors.length > 0) {
          // Display first error from errors array for user-friendly feedback
          this.errorMessage = response.errors[0];
          this.isSubmitting = false;
        } else {
          // Fallback error message if no specific errors provided
          this.errorMessage = response.message || 'Failed to create user';
          this.isSubmitting = false;
        }
      },
      error: (error) => {
        // Network or unexpected errors
        this.errorMessage = error.message || 'Failed to create user. Please try again.';
        this.isSubmitting = false;
      }
    });
  }

  /**
   * Updates an existing user via the service
   * Improved error handling displays specific error messages from API response
   */
  private updateUser(userData: UpdateUserRequest): void {
    if (!this.user) return;

    this.userService.updateUser(this.user.id, userData).subscribe({
      next: (response) => {
        // Handle successful response
        if (response.success) {
          this.isSubmitting = false;
          this.formSubmit.emit(true);
        } else if (response.errors && response.errors.length > 0) {
          // Display first error from errors array for user-friendly feedback
          this.errorMessage = response.errors[0];
          this.isSubmitting = false;
        } else {
          // Fallback error message if no specific errors provided
          this.errorMessage = response.message || 'Failed to update user';
          this.isSubmitting = false;
        }
      },
      error: (error) => {
        // Network or unexpected errors
        this.errorMessage = error.message || 'Failed to update user. Please try again.';
        this.isSubmitting = false;
      }
    });
  }

  /**
   * Handles form cancellation
   */
  onCancel(): void {
    this.formCancel.emit();
  }

  /**
   * Marks all form controls as touched to display validation errors
   * Used when form is submitted with invalid data
   */
  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      control?.markAsTouched();

      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }

  /**
   * Checks if a field has errors and should display error message
   * Only shows errors for touched or dirty fields
   */
  hasError(fieldName: string, errorType?: string): boolean {
    const field = this.userForm.get(fieldName);
    if (!field) return false;

    const hasError = errorType
      ? field.hasError(errorType)
      : field.invalid;

    return hasError && (field.touched || field.dirty);
  }

  /**
   * Gets error message for a specific field
   */
  getErrorMessage(fieldName: string): string {
    const field = this.userForm.get(fieldName);
    if (!field || !field.errors) return '';

    if (field.hasError('required')) {
      return `${this.getFieldLabel(fieldName)} is required`;
    }

    if (field.hasError('email')) {
      return 'Please enter a valid email address';
    }

    if (field.hasError('minlength')) {
      const minLength = field.errors['minlength'].requiredLength;
      return `${this.getFieldLabel(fieldName)} must be at least ${minLength} characters`;
    }

    if (field.hasError('maxlength')) {
      const maxLength = field.errors['maxlength'].requiredLength;
      return `${this.getFieldLabel(fieldName)} cannot exceed ${maxLength} characters`;
    }

    return 'Invalid input';
  }

  /**
   * Gets display label for field name
   */
  private getFieldLabel(fieldName: string): string {
    const labels: { [key: string]: string } = {
      name: 'Name',
      email: 'Email',
      phone: 'Phone',
      website: 'Website',
      company: 'Company'
    };
    return labels[fieldName] || fieldName;
  }
}

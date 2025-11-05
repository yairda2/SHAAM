/**
 * User interface representing the complete user entity
 */
export interface User {
  id: number;
  name: string;
  email: string;
  phone?: string;
  website?: string;
  company?: string;
  createdAt: Date;
  updatedAt: Date;
}

/**
 * CreateUserRequest interface for creating new users
 */
export interface CreateUserRequest {
  name: string;
  email: string;
  phone?: string;
  website?: string;
  company?: string;
}

/**
 * UpdateUserRequest interface for updating existing users
 */
export interface UpdateUserRequest {
  name: string;
  email: string;
  phone?: string;
  website?: string;
  company?: string;
}

/**
 * Generic API response wrapper for consistent response handling
 */
export interface ApiResponse<T> {
  success: boolean;
  data: T | null;
  message: string;
  errors?: string[];
}

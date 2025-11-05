# User Management Application

A production-ready full-stack User Management application built with .NET 8+ backend and Angular 18+ frontend.

## Overview

This application provides a complete user management system with CRUD operations, implementing SOLID principles, design patterns, and modern development practices. The application loads initial user data from the JSONPlaceholder API and provides a beautiful Apple-inspired UI.

## Features

- **Complete CRUD Operations**: Create, Read, Update, and Delete users
- **Search Functionality**: Search users by name, email, or company
- **Real-time Validation**: Client and server-side validation
- **Responsive Design**: Apple-inspired UI that works on all devices
- **Error Handling**: Comprehensive error handling throughout the application
- **Initial Data Seeding**: Automatically loads 10 users from JSONPlaceholder API on first startup

## Tech Stack

### Backend (.NET 8+)
- ASP.NET Core 8 REST API
- Entity Framework Core with In-Memory Database
- AutoMapper for object mapping
- FluentValidation for input validation
- Dependency Injection
- CORS configuration for cross-origin requests

### Frontend (Angular 18+)
- Angular 18+ with Standalone Components
- Reactive Forms with validation
- RxJS for reactive programming
- SCSS for styling
- TypeScript with strict mode

## Architecture & Design Patterns

### Backend Patterns
- **Repository Pattern**: Data access abstraction
- **Service Layer Pattern**: Business logic separation
- **Dependency Injection**: Loose coupling between components
- **Factory Pattern**: Object creation
- **Singleton Services**: Scoped lifetime management
- **Middleware Pattern**: Global exception handling

### Frontend Patterns
- **Component-Based Architecture**: Reusable components
- **Service Injection**: Centralized API communication
- **Observable Pattern**: Reactive data flow with RxJS
- **Smart/Dumb Components**: Separation of concerns

## Project Structure

```
ex5/
├── UserManagementAPI/          # Backend .NET API
│   ├── Controllers/            # API endpoints
│   ├── Models/                 # Data models and DTOs
│   ├── Services/               # Business logic
│   ├── Data/                   # Database context
│   ├── Validators/             # Input validation
│   ├── Middleware/             # Exception handling
│   ├── Mappers/                # AutoMapper profiles
│   └── Common/                 # Shared utilities
│
└── UserManagementUI/           # Frontend Angular app
    └── src/
        ├── app/
        │   ├── components/     # Angular components
        │   ├── services/       # API services
        │   └── models/         # TypeScript interfaces
        └── environments/       # Environment configs
```

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Node.js 18+ and npm
- Visual Studio 2022 or VS Code

### Backend Setup

1. Navigate to the backend directory:
   ```bash
   cd UserManagementAPI
   ```

2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

3. Run the application:
   ```bash
   dotnet run
   ```

   The API will start at `https://localhost:5001`

### Frontend Setup

1. Navigate to the frontend directory:
   ```bash
   cd UserManagementUI
   ```

2. Install npm packages:
   ```bash
   npm install
   ```

3. Start the development server:
   ```bash
   npm start
   ```

   The application will open at `http://localhost:4200`

## API Endpoints

All endpoints return data wrapped in an `ApiResponse<T>` object with the following structure:

```json
{
  "success": boolean,
  "data": T,
  "message": string,
  "errors": string[]
}
```

### Available Endpoints

- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `GET /api/users/search?searchTerm={term}` - Search users
- `POST /api/users` - Create new user
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user


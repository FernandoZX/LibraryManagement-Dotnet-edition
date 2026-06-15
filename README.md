# Library Management System

A web application to manage a library's book inventory and member borrowings, built with **C# / ASP.NET Core** following a **Hexagonal (Ports & Adapters) architecture** on top of **SQL Server**.

This is the backend solution for the *Library Management System* technical challenge, including a full **xUnit** test suite (unit + integration) as the equivalent of the requested spec testing.

---

## Tech Stack

| Concern            | Technology |
|--------------------|------------|
| Language / Runtime | C# 14, .NET 10 (LTS) |
| Web framework      | ASP.NET Core (Controllers) |
| Persistence        | Entity Framework Core 10, SQL Server |
| Authentication     | JWT Bearer (custom, BCrypt password hashing) |
| Validation         | FluentValidation |
| API documentation  | Built-in OpenAPI + Scalar UI |
| Unit testing       | xUnit, Moq, Shouldly |
| Integration testing| xUnit, WebApplicationFactory, Testcontainers (SQL Server), Respawn |

---

## Architecture

The solution uses **Hexagonal Architecture**. Dependencies always point inward toward the domain; the domain itself depends on nothing.

```
            ┌──────────────────────────────────────────────┐
            │                     Api                       │
            │   (Controllers, JWT, middleware, DI root)     │
            └───────────────┬───────────────┬──────────────┘
                            │               │
                            ▼               ▼
            ┌──────────────────────┐   ┌──────────────────────┐
            │     Application       │   │    Infrastructure     │
            │  (use cases, ports,   │◄──│  (EF Core, repos,     │
            │   DTOs, validators)   │   │   JWT, BCrypt, seed)  │
            └───────────┬──────────┘   └──────────────────────┘
                        │
                        ▼
            ┌──────────────────────┐
            │        Domain         │
            │  (entities, business  │
            │   rules, exceptions)  │
            └──────────────────────┘
```

### Projects

| Project | Responsibility |
|---------|----------------|
| `LibraryManagement.Domain` | Entities (`Book`, `User`, `Borrowing`) with their business rules and invariants. No external dependencies. |
| `LibraryManagement.Application` | Use cases (services), output ports (repository / security interfaces), DTOs, validators, and the `Result` pattern. |
| `LibraryManagement.Infrastructure` | Adapters that implement the ports: EF Core `DbContext`, repositories, BCrypt hasher, JWT generator, database seeder. |
| `LibraryManagement.Api` | Composition root: controllers, JWT configuration, exception-handling middleware, dependency injection wiring. |

### Key design decisions

- **Rich domain model.** Business rules live inside the entities (e.g. `Book.Borrow()` / `Book.Return()`), not in the services. Properties have private setters, so invariants can only change through methods that enforce them.
- **Result pattern over exceptions for expected flows.** Domain rule violations throw `DomainException`; expected business outcomes (duplicate email, book not found, conflict) are modeled as `Result` with a typed `ErrorType`, mapped to HTTP status codes in a single place.
- **Ports & adapters.** The Application layer defines interfaces (ports); Infrastructure provides the implementations (adapters). Swapping the database or hashing library only touches Infrastructure.
- **Custom JWT instead of ASP.NET Core Identity.** Keeps the domain clean and avoids coupling to Identity's own `DbContext`.
- **Deterministic time via `TimeProvider`.** The domain receives the current time as a parameter and the services inject `TimeProvider`, making due-date and overdue logic fully testable.
- **Defense in depth.** The "a member cannot borrow the same book twice" rule is enforced both in the service and by a unique filtered index in the database.
- **CQRS-lite for the dashboard.** Read-only dashboard queries use a dedicated query port, separate from the write-side domain repositories.
- **No user enumeration.** Login returns the same error for an unknown email and a wrong password.

---

## Features

| Requirement | Implementation |
|-------------|----------------|
| Register / Login / Logout | JWT-based auth. Logout is client-side (token disposal). Two roles: `Librarian`, `Member`. |
| Only Librarians manage books | `[Authorize(Roles = "Librarian")]` on create / update / delete. |
| Book management | Add, edit, delete books (title, author, genre, ISBN, total copies). |
| Search | Search books by title, author, and/or genre (combinable filters). |
| Borrowing | Members borrow available books; cannot borrow the same book twice while it's active. |
| Due dates | Borrowing tracks `BorrowedAt` and `DueAt` (2 weeks later). |
| Returning | Librarians mark a borrowing as returned; the copy returns to inventory. |
| Librarian dashboard | Total books, total borrowed, books due today, members with overdue books. |
| Member dashboard | Own borrowings, due dates, and overdue count. |
| RESTful API | CRUD endpoints with proper status codes and responses. |
| Spec testing | xUnit suite at three levels (domain, application, integration). |

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local instance, LocalDB, or Docker)
- [Docker](https://www.docker.com/) — required only to run the integration tests
- EF Core tools: `dotnet tool install --global dotnet-ef`

### 1. Configure the connection string

Update `ConnectionStrings:DefaultConnection` in `LibraryManagement.Api/appsettings.json` to point to your SQL Server instance:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=LibraryDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Secret": "CHANGE-THIS-super-secret-key-at-least-32-chars!!",
    "Issuer": "LibraryManagement",
    "Audience": "LibraryManagementClients",
    "ExpiryMinutes": 60
  }
}
```

> The JWT secret must be at least 32 characters (256 bits). In a real deployment, store it in User Secrets or environment variables rather than `appsettings.json`.

### 2. Apply the database migrations

```bash
dotnet ef database update --project LibraryManagement.Infrastructure --startup-project LibraryManagement.Api
```

### 3. Run the API

```bash
dotnet run --project LibraryManagement.Api
```

On startup (Development environment), the application automatically applies pending migrations and seeds demo data. Open the Scalar UI to explore and test the API:

```
https://localhost:<port>/scalar
```

(The exact port is printed in the console and configured in `Properties/launchSettings.json`.)

---

## Demo Credentials

The seeder creates the following users (password `Password123!` for both):

| Role      | Email                    | Password       |
|-----------|--------------------------|----------------|
| Librarian | `librarian@library.com`  | `Password123!` |
| Member    | `member@library.com`     | `Password123!` |

It also seeds a few sample books so the dashboard and search return data immediately.

---

## API Endpoints

All endpoints (except register/login) require a `Authorization: Bearer <token>` header.

### Auth

| Method | Route                | Auth      | Description |
|--------|----------------------|-----------|-------------|
| POST   | `/api/auth/register` | Anonymous | Register a new user and receive a JWT. |
| POST   | `/api/auth/login`    | Anonymous | Log in and receive a JWT. |

### Books

| Method | Route               | Auth          | Description |
|--------|---------------------|---------------|-------------|
| GET    | `/api/books`        | Authenticated | Search by `?title=&author=&genre=` (all optional). |
| GET    | `/api/books/{id}`   | Authenticated | Get a single book. |
| POST   | `/api/books`        | Librarian     | Create a book (201 Created). |
| PUT    | `/api/books/{id}`   | Librarian     | Update a book. |
| DELETE | `/api/books/{id}`   | Librarian     | Delete a book (204 No Content). |

### Borrowings

| Method | Route                          | Auth      | Description |
|--------|--------------------------------|-----------|-------------|
| POST   | `/api/borrowings`              | Member    | Borrow a book by `{ "bookId": "..." }`. |
| PUT    | `/api/borrowings/{id}/return`  | Librarian | Mark a borrowing as returned. |

### Dashboard

| Method | Route                       | Auth      | Description |
|--------|-----------------------------|-----------|-------------|
| GET    | `/api/dashboard/librarian`  | Librarian | Totals, books due today, members with overdue books. |
| GET    | `/api/dashboard/member`     | Member    | Own borrowings with due dates and overdue count. |

### Status code conventions

| Status | Meaning |
|--------|---------|
| 200 OK / 201 Created / 204 No Content | Success |
| 400 Bad Request  | Validation error |
| 401 Unauthorized | Missing/invalid token or bad credentials |
| 403 Forbidden    | Authenticated but wrong role |
| 404 Not Found    | Resource does not exist |
| 409 Conflict     | Business rule violation (duplicate ISBN, no copies, already borrowed, etc.) |

---

## Testing

The test suite mirrors the architecture and forms a testing pyramid:

| Project | Type | What it covers |
|---------|------|----------------|
| `LibraryManagement.Domain.Tests` | Unit | Business rules and invariants (borrow/return logic, due-date calculation, overdue detection). Fast, no dependencies. |
| `LibraryManagement.Application.Tests` | Unit | Use cases with mocked ports (Moq) and a `FakeTimeProvider` for deterministic time. |
| `LibraryManagement.Api.IntegrationTests` | Integration | Full HTTP pipeline against a real SQL Server (Testcontainers), covering auth, role authorization, persistence, and status codes. |

Run everything:

```bash
dotnet test
```

Run a single layer:

```bash
dotnet test LibraryManagement.Domain.Tests
dotnet test LibraryManagement.Application.Tests
dotnet test LibraryManagement.Api.IntegrationTests
```

> The integration tests require **Docker** to be running. The first run is slower because Testcontainers pulls the SQL Server image and waits for the engine to be ready; subsequent runs use the cached image. Each test runs against a clean database (reset with Respawn between tests).

---

## Possible Improvements

- Reject deletion of a book that has active borrowings with a clear 409 instead of relying on the FK restriction.
- Restrict self-registration to the `Member` role; create `Librarian` accounts via an admin flow or seed only.
- Add refresh tokens and token revocation.
- Add pagination to the book search endpoint.
- Add a frontend (React + Vite + Tailwind) consuming the API.

# BookStore API

This is a RESTful API developed using professional .NET architectural principles. The project includes basic CRUD (Create, Read, Update, Delete) operations to manage books, authors, and users, along with secure authentication features.

**Test the live API here:** [**https://alper-bookstoreapi-hsg5guh5cfc3geag.westeurope-01.azurewebsites.net/swagger/index.html**](https://alper-bookstoreapi-hsg5guh5cfc3geag.westeurope-01.azurewebsites.net/swagger/index.html)

---

## Key Features

- **Layered Architecture:** A testable and maintainable architecture built with Interfaces, Generic Services, and Dependency Injection.
- **Security:** Endpoints are protected with a JWT (JSON Web Token) based authentication and authorization system (Register, Login).
- **Relational Data Management:** Manages relationships between books and authors and provides relational endpoints like `GET /api/authors/{id}/books`.
- **Smart Data Creation:** The `POST /api/books` endpoint automatically creates a new author if one doesn't exist when a book is created with a new author's name.
- **Duplicate Data Prevention:** Prevents the same book from being added again for the same author.
- **Paging:** List endpoints (e.g., `GET /api/books`) feature paging to efficiently manage large data sets.

---

## Technologies Used

- **Backend:** ASP.NET Core 8
- **Database:** Azure Cosmos DB for MongoDB
- **Security:** JWT (JSON Web Token)
- **Testing:** xUnit, Moq
- **Hosting:** Microsoft Azure App Service

---

## How to Test

1.  **Register:** Create a new user with the `POST /api/auth/register` endpoint.
2.  **Login:** Log in with the `POST /api/auth/login` endpoint and copy the returned `token`.
3.  **Authorize:** Click the 'Authorize' button at the top right of the Swagger page. In the value field, paste *only* the token string (without the `Bearer` prefix).
4.  **Use the API:** You can now test the locked endpoints like `GET /api/books`.

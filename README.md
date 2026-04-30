# 📈 RasyonetTracker - Financial Data REST API


## 🚀 Project Overview & Architectural Decisions

This project is a modern **.NET 8 Web API** solution developed for the Rasyonet Software Engineering Internship technical assessment. It fetches real-time stock market data and manages a local stock portfolio using SQLite.

As highlighted in the assessment guidelines, since there is no single "correct" solution, my primary focus was on **clean architecture, maintainability, and clear decision-making.** To elevate the application beyond the basic requirements and align it with industry standards, I made the following architectural decisions:

* **Separation of Concerns:** I strictly separated the business logic (`Services`) from the HTTP routing (`Controllers`). Controller classes are kept as thin as possible.
* **Repository Pattern & Testability:** I abstracted the data access layer (`IStockRepository`). This decouples the core business logic from the Entity Framework dependency, making the codebase highly testable with **xUnit and Moq**.
* **Frictionless Setup (Docker):** Knowing that reviewers need to evaluate this easily, I containerized the application. You can spin up the entire application environment with a single command without needing a local .NET SDK or database setup.

## 📡 Data Source & Integration

I chose the **Finnhub API** among the suggested public financial APIs.

**Why Finnhub?**
* **Rate Limit Advantage:** Finnhub's free tier offers *60 requests per minute*, which provided a much more flexible development and testing environment compared to Alpha Vantage (25 requests/day).
* **Fit for Purpose:** The endpoints provided by Finnhub were perfectly straightforward and sufficient for the "real-time quote tracking" scenario.

**Integration & Security:**
The external API integration is isolated within the `StockService` using `HttpClient` to keep the architecture clean. Following security best practices, the API Key is **never hardcoded** in the source code. Instead, it is configured to be read from configurations (`appsettings.json` or Environment Variables).

## ✅ Requirements Breakdown

All mandatory, hidden, and bonus requirements specified in the interview document were carefully analyzed and implemented as follows:

### 🔴 Must Have
* **.NET 6+:** The project is built with the current LTS version, **.NET 8**.
* **External API Integration:** Real-time price data is fetched using the **Finnhub API**.
* **Analytical / Aggregation:** Dedicated endpoints were created to calculate both the **Total Value** and the **Average Value** of the portfolio based on the real-time prices of the stocks in the database.
* **Database:** **SQLite** (with Entity Framework Core) was chosen for its zero-configuration, platform-independent nature. `Stock` is used as the core entity.
* **Clean RESTful API:** 5 clean endpoints were created, covering basic CRUD and aggregation operations.
* **OOP Principles:** Object-Oriented principles like Interfaces, Encapsulation, and Dependency Injection are strictly followed throughout the project.
* **Design Pattern:** The **Repository Pattern** is used to abstract the data access layer. As requested, inline comments are included in the relevant classes explaining why this pattern was chosen. Unnecessary pattern forcing was avoided.
* **Swagger:** Fully active and functional in the Development environment.
* **README.md:** This document covers all the requested information (purpose, choices, instructions).
* **Compiles and Runs:** The project compiles and runs without any errors.

### 🟡 Evaluated During Code Review
* **Error Handling:** Raw exceptions are not exposed to the client. They are wrapped in try-catch blocks returning meaningful HTTP status codes (404, 400, 201, 200).
* **Code Quality:** Clean Code naming conventions are followed, and no dead code is left.
* **Project Structure:** The project is logically separated into folders: `Controllers`, `Services`, `Repositories`, `Models`, and `DTOs`.
* **Separation of Concerns:** Business logic does not live in Controllers; it is delegated to the `StockService`.
* **Git History:** Meaningful, incremental Git commits were made using standard prefixes (feat:, test:, chore:) rather than a single massive commit.

### 🟢 Bonus
* **⭐ Unit Tests:** Focusing on "quality over quantity," the core business logic (`StockService`) is tested using **xUnit** and **Moq**.
* **🐳 Docker Support:** The application is containerized with a `Dockerfile` and `.dockerignore`. It can be run with a single Docker command without any SDK requirements.
* **Why No Frontend?:** Given the "Backend Development" focus of the position and the time constraints, I decided that focusing on backend architecture (Repository Pattern), reliability (xUnit Tests), and deployment ease (Docker) would be a more professional approach than writing a basic UI.
* **Why No Additional Design Patterns?:** I heeded the warning: *"Avoid forcing patterns unnecessarily."* The Repository Pattern abstracts the architecture sufficiently for a project of this scale. Patterns like Factory or CQRS would be "over-engineering," so they were consciously omitted.

## 🐳 Running with Docker (Frictionless Setup)

To make the evaluation process as seamless as possible, this project is fully containerized. You **do not need to install the .NET SDK or SQLite** on your local machine to test this application. Docker is all you need.

Follow these simple steps to spin up the project in an isolated environment in seconds:

### 1. Build the Docker Image
Open your terminal in the root directory of the project (where the `Dockerfile` is located) and run this command:
```bash
docker build -t rasyonettracker-api .
```

### 2. Initializing (Run) the Container
``` bash
docker run -d -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Development --name myapi rationalettracker-api
```


### 3. Accessing the API

Once the container has successfully launched, open your browser to test the application and go to the following address:

**http://localhost:8080/swagger**
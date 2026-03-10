
# Banking Microservices Application

## Overview

This project demonstrates a **.NET microservices architecture** using:

* **ASP.NET Core Web APIs**
* **Ocelot API Gateway**
* **Steeltoe Service Discovery**
* **Netflix Eureka Server**
* **Refit for service-to-service communication**
* **Global Exception Handling Middleware**

The system contains three main services:

| Service                     | Purpose                             |
| --------------------------- | ----------------------------------- |
| **Banking.Gateway**         | Entry point for all client requests |
| **Banking.CustomerService** | Manages customer data               |
| **Banking.AccountService**  | Manages bank accounts               |

Services communicate using **service discovery via Eureka**, avoiding hardcoded URLs.

---

# Architecture

```
Client / Postman
        │
        ▼
Banking.Gateway (Ocelot API Gateway)
        │
        ▼
Service Discovery (Eureka Server)
        │
 ┌───────────────┴───────────────┐
 ▼                               ▼
CustomerService             AccountService
```

### Communication Flow

1. Client calls the **Gateway**.
2. Gateway forwards the request using **Ocelot routing**.
3. Services communicate using **Refit clients**.
4. Service addresses are resolved through **Eureka Service Discovery**.

---

# Project Structure

```
BankingApplication
│
├── Banking.Gateway
│       ├── Program.cs
│       └── ocelot.json
│
├── Banking.CustomerService
│       ├── Controllers
│       ├── Repositories
│       ├── Clients
│       └── Program.cs
│
├── Banking.AccountService
│       ├── Controllers
│       ├── Repositories
│       ├── Clients
│       └── Program.cs
│
└── Banking.Common
        └── Middleware
                └── GlobalExceptionHandlerMiddleware.cs
```

---

# Technologies Used

* **.NET 8 / .NET 9**
* **ASP.NET Core Web API**
* **Steeltoe**
* **Netflix Eureka**
* **Ocelot API Gateway**
* **Refit HTTP Client**
* **Docker (for Eureka)**

---

# Prerequisites

Ensure the following tools are installed:

* .NET SDK 8+
* Docker
* Visual Studio / VS Code
* Postman (for API testing)

---

# Running the Application

The application can be executed using **two methods**:

1. **Run Locally using .NET**
2. **Run using Docker Compose**

---

# Method 1 — Run Locally (.NET)

## Step 1 — Start Eureka Server

Run Eureka using Docker:

```bash
docker run -d -p 8761:8761 --name eureka-server steeltoeoss/eureka-server

## Step 1 — Start Eureka Server

Run Eureka using Docker:

```bash
docker run -d -p 8761:8761 --name eureka-server steeltoeoss/eureka-server
```

Verify Eureka is running:

```
http://localhost:8761
```

You should see the **Eureka Dashboard**.

---

## Step 2 — Run Customer Service

Open a terminal and run:

```bash
dotnet run --project Banking.CustomerService
```

The service will:

* Register itself with Eureka
* Start the API server

Example running port:

```
http://localhost:5208
```

---

## Step 3 — Run Account Service

Open another terminal:

```bash
dotnet run --project Banking.AccountService
```

Example running port:

```
http://localhost:5102
```

---

## Step 4 — Run API Gateway

Open another terminal:

```bash
dotnet run --project Banking.Gateway
```

Example port:

```
http://localhost:5029
```

---

# Verify Service Registration

Open Eureka UI:

```
http://localhost:8761
```
# Method 2 — Run using Docker Compose

## Step 1 — Build the Containers

Build the Containers

```bash
docker compose build
```
## Step 2 — Start the System

Run the Containers

```bash
docker compose up -d
```

## Step 3 — Verify Running Containers

Check running containers:

```bash
docker ps
```
## Step 4 — Verify Eureka Registration

Open the Eureka dashboard:

```bash
http://localhost:8761
```

You should see:

```
ACCOUNT-SERVICE
CUSTOMER-SERVICE
BANKING.GATEWAY
```

All services should show **Status = UP**.

---

# API Endpoints

All requests should be sent to the **Gateway**.

### Base Gateway URL

```
http://localhost:{gateway-port}
```

Example:

```
http://localhost:5029
```

---

## Customer APIs

### Get Customers

```
GET /customers
```

Example:

```
http://localhost:5029/customers
```

---

### Create Customer

```
POST /customers
```

Body:

```json
{
  "name": "John Doe",
  "email": "john@example.com"
}
```

---

## Account APIs

### Get Accounts

```
GET /accounts
```

Example:

```
http://localhost:5029/accounts
```

---

### Create Account

```
POST /accounts
```

Body:

```json
{
  "customerId": 1,
  "balance": 1000
}
```

---

# Service-to-Service Communication

Services call each other using **Refit clients**.

Example:

```
AccountService → CustomerService
CustomerService → AccountService
```

Instead of using fixed URLs, they call using **service names**:

```
http://CUSTOMER-SERVICE
http://ACCOUNT-SERVICE
```

Eureka resolves the actual service location.

---

# Error Handling

The project uses a shared middleware:

```
GlobalExceptionHandlerMiddleware
```

Features:

* Centralized exception handling
* Standard JSON error responses
* Prevents internal stack traces from leaking to clients

Example error response:

```json
{
  "message": "An internal server error occurred",
  "type": "HttpRequestException"
}
```

---

# Useful Commands

Stop Eureka:

```bash
docker stop eureka-server
```

Remove Eureka container:

```bash
docker rm eureka-server
```

---



Verify Eureka is running:

```
http://localhost:8761
```

You should see the **Eureka Dashboard**.

---

## Step 2 — Run Customer Service

Open a terminal and run:

```bash
dotnet run --project Banking.CustomerService
```

The service will:

* Register itself with Eureka
* Start the API server

Example running port:

```
http://localhost:5208
```

---

## Step 3 — Run Account Service

Open another terminal:

```bash
dotnet run --project Banking.AccountService
```

Example running port:

```
http://localhost:5102
```

---

## Step 4 — Run API Gateway

Open another terminal:

```bash
dotnet run --project Banking.Gateway
```

Example port:

```
http://localhost:5029
```

---

# Verify Service Registration

Open Eureka UI:

```
http://localhost:8761
```

You should see:

```
ACCOUNT-SERVICE
CUSTOMER-SERVICE
BANKING.GATEWAY
```

All services should show **Status = UP**.

---

# API Endpoints

All requests should be sent to the **Gateway**.

### Base Gateway URL

```
http://localhost:{gateway-port}
```

Example:

```
http://localhost:5029
```

---

## Customer APIs

### Get Customers

```
GET /customers
```

Example:

```
http://localhost:5029/customers
```

---

### Create Customer

```
POST /customers
```

Body:

```json
{
  "name": "John Doe",
  "email": "john@example.com"
}
```

---

## Account APIs

### Get Accounts

```
GET /accounts
```

Example:

```
http://localhost:5029/accounts
```

---

### Create Account

```
POST /accounts
```

Body:

```json
{
  "customerId": 1,
  "balance": 1000
}
```

---

# Service-to-Service Communication

Services call each other using **Refit clients**.

Example:

```
AccountService → CustomerService
CustomerService → AccountService
```

Instead of using fixed URLs, they call using **service names**:

```
http://CUSTOMER-SERVICE
http://ACCOUNT-SERVICE
```

Eureka resolves the actual service location.

---

# Error Handling

The project uses a shared middleware:

```
GlobalExceptionHandlerMiddleware
```

Features:

* Centralized exception handling
* Standard JSON error responses
* Prevents internal stack traces from leaking to clients

Example error response:

```json
{
  "message": "An internal server error occurred",
  "type": "HttpRequestException"
}
```

---

# Useful Commands

Stop Eureka:

```bash
docker stop eureka-server
```

Remove Eureka container:

```bash
docker rm eureka-server
```

---


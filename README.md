# Banking Microservices Application

## Overview

This project is a **Microservices-based Banking Application** built using **.NET 9**. It demonstrates a simplified banking system implemented using modern microservices architecture patterns including:

* API Gateway
* Service Discovery
* Inter-service communication
* Centralized exception handling
* Containerization using Docker

The application consists of two core microservices:

* **Customer Service** – manages customer data
* **Account Service** – manages account transactions

These services communicate through REST APIs and are discovered dynamically using **Eureka Service Registry**.

---

# Architecture

```
Client (Postman / Browser)
        │
        ▼
API Gateway (Ocelot)
        │
        ▼
Eureka Service Registry
   │              │
   ▼              ▼
CustomerService  AccountService
```

### Components

| Component        | Description                                    |
| ---------------- | ---------------------------------------------- |
| API Gateway      | Routes external requests to internal services  |
| Eureka Server    | Service discovery registry                     |
| Customer Service | Manages customers                              |
| Account Service  | Manages accounts                               |
| Banking.Common   | Shared DTOs, models, exceptions and middleware |

---

# Technologies Used

* .NET 9
* ASP.NET Core Web API
* Ocelot API Gateway
* Steeltoe Eureka Discovery Client
* Refit (HTTP client for inter-service communication)
* Docker & Docker Compose

---

# Project Structure

```
BankingApplication
│
├── Banking.Common
│   ├── Models
│   ├── DTOs
│   ├── Exceptions
│   └── Middleware
│
├── Banking.CustomerService
│   ├── Controllers
│   ├── Repositories
│   ├── Clients
│   └── Program.cs
│
├── Banking.AccountService
│   ├── Controllers
│   ├── Repositories
│   ├── Clients
│   └── Program.cs
│
├── Banking.Gateway
│   ├── ocelot.json
│   └── Program.cs
│
└── docker-compose.yml
```

---

# Features Implemented

## Customer Service APIs

| Method | Endpoint              | Description                          |
| ------ | --------------------- | ------------------------------------ |
| POST   | `/api/customers`      | Add new customer                     |
| GET    | `/api/customers`      | Get all customers                    |
| GET    | `/api/customers/{id}` | Get customer details                 |
| PUT    | `/api/customers/{id}` | Update customer                      |
| DELETE | `/api/customers/{id}` | Delete customer and related accounts |

---

## Account Service APIs

| Method | Endpoint                            | Description         |
| ------ | ----------------------------------- | ------------------- |
| POST   | `/api/accounts`                     | Create account      |
| POST   | `/api/accounts/{id}/add-money`      | Deposit money       |
| POST   | `/api/accounts/{id}/withdraw-money` | Withdraw money      |
| GET    | `/api/accounts/{id}`                | Get account details |
| DELETE | `/api/accounts/{id}`                | Delete account      |

Account service validates customer existence by calling **Customer Service**.

---

# Cross Cutting Concerns

The following cross-cutting concerns are implemented:

* Global Exception Handling Middleware
* Service Discovery using Eureka
* API Gateway Routing
* Inter-service communication using Refit
* Containerized deployment using Docker

---

# Running the Application

## Prerequisites

Install the following:

* .NET 9 SDK
* Docker
* Docker Compose
* Postman (for testing)

---

# Option 1 – Run using Docker (Recommended)

### Step 1 – Navigate to the project root

```
cd BankingApplication
```

### Step 2 – Build Docker images

```
docker compose build
```

### Step 3 – Start containers

```
docker compose up
```

### Step 4 – Verify running containers

```
docker ps
```

Expected containers:

```
gateway
customer-service
account-service
eureka
```

---

# Accessing Services

### Eureka Dashboard

```
http://localhost:8761
```

You should see:

```
CUSTOMER-SERVICE
ACCOUNT-SERVICE
```

---

### API Gateway

```
http://localhost:5000
```

All external requests should go through the gateway.

---

# Testing APIs using Postman

## Create Customer

```
POST http://localhost:5000/customers
```

Body

```
{
  "name": "Bhargav",
  "email": "Bhargav@yahoo.com",
  "phone": "1234567890"
}
```

---

## Get Customers

```
GET http://localhost:5000/customers
```

---

## Create Account

```
POST http://localhost:5000/accounts
```

Body

```
{
  "customerId": 1,
  "balance": 10000
}
```

---

## Deposit Money

```
POST http://localhost:5000/accounts/1/add-money
```

Body

```
{
  "amount": 2000,
  "customerId": 1
}
```

---

## Withdraw Money

```
POST http://localhost:5000/accounts/1/withdraw-money
```

Body

```
{
  "amount": 100,
  "customerId": 1
}
```

---

## Delete Customer (Cascade Delete)

```
DELETE http://localhost:5000/customers/1
```

Deleting a customer automatically removes all related accounts.

---

# Running Without Docker (Local Development)

Start services individually.

### Start Customer Service

```
dotnet run --project Banking.CustomerService
```

Runs on:

```
http://localhost:5001
```

---

### Start Account Service

```
dotnet run --project Banking.AccountService
```

Runs on:

```
http://localhost:5002
```

---

### Start API Gateway

```
dotnet run --project Banking.Gateway
```

Runs on:

```
http://localhost:5000
```

---

# Assumptions

* In-memory repositories are used instead of databases.
* Data will reset when services restart.
* Authentication and authorization are not implemented as part of Assignment.

---

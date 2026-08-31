# Smart Business Manager

## Project Overview

Smart Business Manager is an enterprise-grade mobile and desktop application designed to streamline business operations, financial tracking, client management, and AI-driven insights. Built to address the complex needs of modern businesses, it provides a centralized hub to monitor performance, manage authentications, and optimize operational efficiency. This application delivers high business value by reducing manual overhead, improving decision-making through AI integrations, and ensuring a scalable foundation for future growth.

## Tech Stack

This project is built using the **.NET MAUI (Multi-platform App UI)** framework, providing a native cross-platform experience across iOS, Android, macOS, and Windows from a single C# codebase.

*   **Framework:** .NET MAUI
*   **Language:** C# 10+ / XAML
*   **Architecture:** Clean Architecture / Feature-Driven Design (Vertical Slicing)
*   **State Management & Data Binding:** MVVM (Model-View-ViewModel)

## Architecture

To ensure zero technical debt, high maintainability, and scalability, the application strictly adheres to **Clean Architecture** principles combined with **Vertical Slice Architecture**. The codebase is organized to separate concerns, making it highly testable and robust for large engineering teams.

*   **Presentation Layer (`Features/`):** Organized by feature (e.g., `Authentication`, `Dashboard`, `Finance`, `Clients`, `AI`). Each feature module encapsulates its own UI (XAML views) and ViewModels, ensuring that modifications to one domain do not introduce regressions in others.
*   **Domain Layer (`Core/Models/`):** Contains the core enterprise business rules, entities, and abstractions. It has no dependencies on external frameworks, ensuring the business logic remains pure and isolated.
*   **Data & Infrastructure Layer (`Core/Services/` & `Core/Helpers/`):** Responsible for external communications, API clients, local persistence, and utility helpers. This layer implements the interfaces defined by the Domain layer, following the Dependency Inversion Principle.

## Key Features

*   **Secure Authentication:** Robust user authentication and session management.
*   **Interactive Dashboard:** Real-time metrics and high-level overview of business health.
*   **Finance Tracking:** Comprehensive financial management, reporting, and tracking.
*   **Client Management:** Centralized CRM capabilities for managing clients and interactions.
*   **AI Integration:** Embedded AI features for advanced analytics and automated assistance.
*   **Cross-Platform Delivery:** True native performance across mobile (iOS/Android) and desktop platforms.

## Getting Started

### Prerequisites

*   [.NET 8.0 SDK](https://dotnet.microsoft.com/download) (or later)
*   [Visual Studio 2022](https://visualstudio.microsoft.com/) (with .NET MAUI workload installed) or Visual Studio Code with the .NET MAUI extension.

### Installation & Run

1.  **Clone the repository:**
    ```bash
    git clone <repository_url>
    ```
2.  **Navigate to the project directory:**
    ```bash
    cd SmartBusinessManager
    ```
3.  **Restore dependencies:**
    ```bash
    dotnet restore
    ```
4.  **Run the application:**
    Open `SmartBusinessManager.sln` in Visual Studio and click **Start Debugging (F5)**, or use the .NET CLI:
    ```bash
    dotnet build -t:Run -f net8.0-android # Replace with your target platform
    ```

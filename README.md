# Healthcare Quality & Management System (HQMS)

## Overview
HQMS is an independent project developed to enhance practical skills in modern **.NET 8 architecture**, **Microservices**, and **Azure Cloud Services**.  
The system simulates a **hospital management solution** with features like patient registration, appointment management, doctor queues, and real-time notifications.

---

## Key Features
- Patient Registration & Appointment Management
- Doctor & Hospital Queue Management (Event-Driven using SignalR)
- Payment Module Integration
- Role-based Access (Admin, Doctor, Patient)
- Real-time Dashboard Updates
- RESTful APIs for internal and external integrations

---

## Technologies Used
- **Backend:** C#, .NET 8, ASP.NET Core, Entity Framework Core
- **Frontend:** React.js (Bootstrap UI)
- **Database:** SQL Server
- **Cloud Services:** Azure Service Bus, Azure Blob Storage, Azure Functions, Key Vault
- **Messaging / Real-Time:** SignalR
- **CI/CD:** Azure DevOps (YAML pipelines)
- **Testing:** xUnit

---

## Architecture
- Clean Architecture / Domain-Driven Design
- Microservices with Event Publisher & Subscriber pattern
- Outbox pattern for reliable event delivery
- Separate layers: Domain, Application, Infrastructure, WebAPI, Shared

---

## How to Run
1. Clone the repository:
2. Restore NuGet packages:

dotnet restore


3. Update the appsettings.json with your SQL Server and Azure configurations

4. Run the backend API:

dotnet run --project WebAPI/HQMS.WebAPI

5. Run the frontend React app:

cd Frontend
npm install
npm start

Notes

🚀 Work in Progress: This project is currently under development. While some features may not be fully implemented yet, the repository demonstrates my expertise in .NET 8, Microservices, Azure Cloud Services, Event-Driven Architecture, and modern architecture patterns.

Sensitive production code is excluded for security.

Contact

Naveen Kumar
Email: navinjaiswal05@gmail.com
LinkedIn: https://www.linkedin.com/in/naveen-kumar-729a6653/


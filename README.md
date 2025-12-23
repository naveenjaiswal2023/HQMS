## Overview 🚀
**HQMS** is a production-grade **microservices-based hospital management system** built with **.NET 8**, **Azure Cloud Services**, and modern architecture patterns. This independent project demonstrates expertise in scalable enterprise solutions for patient care, queue management, and real-time operations.

**Key Business Value:**
- Patient registration & appointment scheduling
- Real-time doctor queue management (Event-Driven)
- Secure payment processing
- Role-based access control (Admin/Doctor/Patient)

## 🏗️ Architecture
Domain-Driven Design | Clean Architecture | CQRS | Event-Driven
┌─────────────────┐ ┌──────────────────┐
│ API Gateway │◄──►│ Azure Service Bus │
└─────────┬───────┘ └──────────────────┘
│
┌──────────▼──────────┐
│ Microservices │
│ - Authentication │
│ - Patient Records │
│ - Appointments │
│ - Queue Management │
│ - Payments │
│ - Notifications │
└──────────┬──────────┘
│
┌──────────▼──────────┐
│ Redis Cache │──► Serilog (Central Logging)
└─────────────────────┘

text

## ✨ Key Features
| Feature | Technology | Status |
|---------|------------|--------|
| Patient Registration & EMR | EF Core, .NET 8 | ✅ Complete |
| Real-time Queue Management | SignalR + Azure Service Bus | ✅ Complete |
| Appointment Scheduling | CQRS + MediatR | ✅ Complete |
| Role-based Authentication | JWT + Refresh Tokens | ✅ Complete |
| Payment Integration | Stripe/PayPal APIs | 🔄 In Progress |
| Real-time Dashboard | React + SignalR | ✅ Complete |
| CI/CD Pipelines | Azure DevOps YAML | ✅ Complete |

## 🛠️ Tech Stack
Backend: .NET 8, ASP.NET Core, EF Core, MediatR, AutoMapper
Frontend: React 18, TypeScript, Bootstrap 5, SignalR Client
Database: PostgreSQL, SQL Server
Cloud: Azure Service Bus, Redis, Blob Storage, App Services
DevOps: Azure DevOps, Docker, GitHub Actions
Testing: xUnit, Moq, FluentAssertions
Monitoring: Serilog, Application Insights

## 🚀 Quick Start
Clone & Restore
git clone https://github.com/naveenjaiswal2023/HQMS.git
cd HQMS
dotnet restore

Update appsettings.json (Azure/SQL connection strings)
Run API Gateway
dotnet run --project src/ApiGateway/HQMS.ApiGateway

Run Frontend (separate terminal)
cd src/Frontend
npm install
npm start

## 📈 Demo & Screenshots
![Dashboard Preview](https://via.placeholder.com/1200x600/1e3a8a/ffffff?text=HQMS+Dashboard) *[Live Demo Coming Soon]*

## 📋 Roadmap
- [x] Microservices scaffolding & API Gateway
- [x] Event-Driven Queue Management
- [x] JWT Authentication with Refresh Tokens
- [ ] Payment Gateway Integration
- [ ] Mobile App (React Native)
- [ ] ML-based Patient Triage

## 🤝 Contact
**Naveen Jaiswal**  
[✉️ navinjaiswal05@gmail.com](mailto:navinjaiswal05@gmail.com) | [🔗 LinkedIn](https://www.linkedin.com/in/naveen-kumar-729a6653/) | [🌐 Portfolio](https://naveenjaiswal.dev)

**⭐ Star this repo if you find it helpful!**

*🔒 Sensitive production configurations excluded for security. Work in active development.*
Key Improvements Made:
Visual Hierarchy: Emojis, badges, tables, and diagrams improve scannability
​

Conciseness: Reduced fluff while retaining technical depth

Recruiter-Friendly: Clear tech stack, status table, quick start guide

GitHub Optimized: Badges, screenshots placeholder, contribution call-to-action

Professional Polish: Consistent formatting, proper code blocks, roadmap

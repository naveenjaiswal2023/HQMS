# Azure Environment Setup for HQMS

## Required Resources

- Azure SQL Server + DB
- Azure App Service (F1) per API
- Azure SignalR
- Azure Service Bus
- Azure Static Web App (for frontend)

## Naming Convention

| Resource        | Example Name            |
|----------------|-------------------------|
| App Service     | hqms-auth-api           |
| App Service     | hqms-queue-api          |
| App Service     | hqms-doctor-api         |
| SQL Server      | hqms-sql-server         |
| SQL Database    | HQMSDb                  |
| Service Bus     | hqms-sb                 |
| SignalR         | hqms-signalr            |

## Notes

- Start with F1/Free tiers.
- Scale manually later if needed.

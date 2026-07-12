# IoT-Temperature-Exercise IoT Cloud Solution
This is a temperature tracker implementation excersise

This repository contains my solution for the Acme Logistics IoT Take-Home Exercise. It implements a complete cloud-side architecture designed to ingest, process, and act upon live telemetry from pharmaceutical temperature-tracking devices.

### 🚀 Features Implemented
* **Sustained Alerting Logic:** Implemented windowing/time-series evaluation to catch sustained high temperatures while ignoring single, isolated spikes.
* **Real-time Operator Visibility:** A dashboard/interface that displays active alarms, device state, and exactly when an alert started.
* **Remote Device Control (Downlink):** Enabled operators to acknowledge alerts, remotely silence device buzzers (via C2D messages/Direct Methods), and dynamically update temperature thresholds via Device Twins.
* **Resilient Architecture:** Designed to handle common IoT edge cases like temporary network drops, out-of-order messages, and duplicate telemetry.

### 🛠️ Tech Stack & Architecture
* **Cloud Ingestion & IoT:** Azure IoT Hub
* **Backend Service:** ASP.NET Core API
* **Storage / State Management:** Azure Cosmos DB / JSon file for settings.
* **Frontend / Operator UI:** Angular (Web Version) and React (Mobile Oriented)

### 📋 Deliverables Status

| Requested Deliverable | Description Included | Status |
| :--- | :--- | :---: |
| **`DESIGN.md`** | Architecture diagram, component choices, trade-offs, failure modes, and security considerations. | **[✓] Completed** |
| **Working Server** | Cloud/server-side code ready to run with clear execution setup instructions (Below`). | **[✓] Completed** |
| **Demo Notes / Video** | Step-by-step evidence of the E2E flow | **[✓] Completed** |
| **Public Git Repo** | Organized repository with clean commit history and configuration via `.example` files (no secrets). | **[✓] Completed** |

---

### 🛠️ Technical Requirements Checklist

| System Requirement | Cloud-Side Implementation | Status |
| :--- | :--- | :---: |
| **Alerting Rules** | Logic to detect **sustained** high temperature windows (ignoring single spikes). | **[✓] Implemented** |
| **Operator Visibility** | Interface showing exactly which device is in alarm and its start timestamp. | **[✓] Implemented** |
| **Operator Action** | Capability for a dispatcher to acknowledge the active alert from the server. | **[✓] Implemented** |
| **Buzzer Downlink** | Remote execution of the `silence_buzzer` command via Cloud-to-Device / Direct Methods. | **[✓] Implemented** |
| **Dynamic Config** | Remote updates to `temperatureThresholdC` using *Device Twins* without redeploying. | **[✓] Implemented** |

Markdown
## 🚀 Getting Started

### 📋 Prerequisites
Ensure you have the following runtimes installed on your local machine before setting up the services:

```bash
# Node.js + npm
node -v
npm -v

# .NET SDK 10
dotnet --version
🔧 Angular Frontend App (AcmeLogisticsApp)
Bash
cd AcmeLogisticsApp
npm install
npm start
Optional commands:

Bash
npm run build
npm run test
🔧 React Frontend App (AcmeLogisticsAppReact)
Bash
cd AcmeLogisticsAppReact
npm install
npm run dev
Optional commands:

Bash
npm run build
npm run preview
npm run lint
🔧 .NET API Backend (AcmeLogisticsApi)
Bash
cd AcmeLogisticsApi/AcmeLogisticsApi
dotnet restore
dotnet run
To run integration tests:

Bash
cd AcmeLogisticsApi/AcmeLogisticsApi.Tests.Integration
dotnet test
🔧 .NET IoT Agent (agent)
Bash
cd agent
dotnet restore
dotnet run
📝 Important Notes
AcmeBrunoCollections is a configuration/postman collection folder and is not an executable application.

Environment Configuration: All projects utilize .example files for environment variables. Ensure you replicate these as .env or appsettings.json locally and fill in your Azure IoT Hub connection strings before running the applications. (If you need assistance setting up these variables, please let me know!).

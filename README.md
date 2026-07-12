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


# Local Setup Guide

Follow these steps to run the entire ecosystem locally.

---

## 📋 1. Prerequisites

Before starting, ensure you have the following environments installed on your machine:

# Node.js & npm (For Frontend Apps)
node -v
npm -v

# .NET SDK (For Backend API & IoT Agent)
dotnet --version

---

## 🛠️ 2. Running the Frontend Applications

Choose either the Angular or React version of the operator interface to run:

### Option A: Angular App (AcmeLogisticsApp)
cd AcmeLogisticsApp
npm install
npm start

*Optional Production & Testing Commands:* npm run build or npm run test

### Option B: React App (AcmeLogisticsAppReact)
cd AcmeLogisticsAppReact
npm install
npm run dev

*Optional Development Commands:* npm run build, npm run preview, or npm run lint

---

## ⚙️ 3. Running the Backend Services

### Step 1: Start the .NET Core API (AcmeLogisticsApi)
cd AcmeLogisticsApi/AcmeLogisticsApi
dotnet restore
dotnet run

### Step 2: Run Integration Tests (Optional)
cd AcmeLogisticsApi/AcmeLogisticsApi.Tests.Integration
dotnet test

---

## 📟 4. Running the IoT Device Agent

To simulate live tracker data sending telemetry to your backend, run the C# agent:

#### Connect and send telemetry every 30 seconds (Ctrl+C to stop)
dotnet run --project agent -- connect --device-id shipment-001

#### Custom interval
dotnet run --project agent -- connect --device-id shipment-001 --interval-seconds 15

#### Send a single telemetry message
dotnet run --project agent -- send --device-id shipment-001 --temperature 8.4

#### Simulate a high-temperature event (several readings)
dotnet run --project agent -- simulate-alarm --device-id shipment-001 --cycles 5

#### Show local device state
dotnet run --project agent -- status

#### Run for full option details.
dotnet run --project agent -- --help

---

## 📝 Key Notes & Configuration

* AcmeBrunoCollections: This directory contains API testing collections (such as Bruno or Postman) and is not an executable application.
* Environment Variables: All applications rely on template configuration files (e.g., .env.example or appsettings.example.json). Make sure to create your local copies without the .example extension and populate them with your Azure IoT Hub connection strings before spinning up the services.

## 2. General Architecture

### Block Diagram

The system follows a standard event-driven IoT architecture for real-time ingestion, processing, and remote management:

[IoT Device / Agent] ---> [Azure IoT Hub] ---> [.NET Backend Server] ---> [Azure Blob Storage]
                                                      |
                                                      v
                                             [Frontend Interface]

You can view the detailed architecture diagram below:


<img width="892" height="316" alt="image" src="https://github.com/user-attachments/assets/288891da-fe9a-41eb-a3b1-6a15f191992b" />


---

## 3. Step-by-Step Implementation

### Telemetry Ingestion
* **Ingestion Service:** Implemented `TelemetryIngestService` utilizing the Azure `EventProcessorClient` to safely read and partition real-time messages streaming from Azure IoT Hub.
* **Storage Containers:** 
  * `checkpoints`: Manages partition offsets to prevent data loss or duplicate processing during service restarts.
  * `telemetry`: Persists raw JSON telemetry payloads for historical auditing.

### Sustained Alerting Rules
* **Temperature Threshold:** Set to a baseline evaluation of 8°C.
* **Sustained Evaluation Lemic:** Implemented a consecutive readings counter per device window. Alerts are only triggered when the threshold is violated continuously over a period, ignoring single isolated spikes.
* **Alert Persistence:** Once an alert state is validated, a structured JSON file named `alert-*.json` is automatically generated and written to the blob storage state container.

### Web API Controller
Exposes the core operational endpoints for the frontend applications:
* `GET /api/telemetry/messages` - Retrieves the latest historical telemetry payloads.
* `GET /api/telemetry/alerts` - Fetches all currently active or unacknowledged alarms.
* `PUT /api/telemetry/alerts/{id}/acknowledge` - Marks an active alarm as acknowledged/resolved by an operator.

### Downlink & Configuration Management
Handles remote interaction back to the hardware layer:
* `POST /api/device/{id}/command` - Dispatches Cloud-to-Device (C2D) messages or direct methods to execute immediate hardware actions.
  * *Example Payload:* `{ "command": "silence_buzzer", "correlationId": "abc-123" }`
* `PUT /api/device/{id}/config` - Dynamically updates the Device Twin's desired properties without code redeployment.
  * *Example Payload:* `{ "temperatureThresholdC": 6.0 }`

---

## 4. Integration Testing

The system reliability is verified through the `AcmeLogisticsApi.Tests.Integration` test suite built using **xUnit**.

* **In-Memory Testing:** Employs `WebApplicationFactory` to spin up a fully functional instance of the API in-memory, simulating real network requests and dependency routing.
* **Automated Scenarios Covered:**
  1. Simulating sustained high-temperature cycles to verify alert generation.
  2. Executing operator acknowledgement on an active alarm.
  3. Dispatching the `silence_buzzer` downlink command.
  4. Confirming hardware state response in subsequent telemetry frames.

### Test Example:

[Fact]
public async Task OperatorCanAcknowledgeAlert()
{
    // Act
    var response = await _client.PutAsync("/api/telemetry/alerts/alert-12345/acknowledge", null);
    
    // Assert
    response.IsSuccessStatusCode.Should().BeTrue();
}

---

## 5. Expected Results

When executing the entire ecosystem end-to-end, the following behaviors are validated:
* **Storage Updates:** Alerts are accurately written as JSON blobs under the storage containers upon rule criteria matching.
* **Operator Flow:** Dispatchers can seamlessly view, track, and acknowledge live triggers from either frontend application.
* **Edge Feedback Loop:** Downlink instructions are successfully intercepted by the C# device agent, which reacts by modifying its local loop state (silencing its buzzer or adjusting its internal tracking threshold).

---

## 6. Conclusions

* **End-to-End Core Competency:** The solution successfully delivers a complete IoT cloud loop, closing the gap between raw hardware telemetry and business operational dashboards.
* **Azure Integration:** Demonstrated robust, production-ready integration patterns utilizing native Azure IoT Hub primitives, distributed event consumers, and scalable object storage.
* **Test-Driven Security:** The inclusion of automated integration tests ensures the system's alerting and command pathways remain highly stable and maintainable under future updates.

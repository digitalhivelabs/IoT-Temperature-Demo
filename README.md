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

#Summary: 
# Workspace Architecture Overview — IoT Temperature Demo

Comprehensive system layout detailing the interaction, structure, and data flow across the backend API, frontend applications, and the local hardware simulator.

---

## 1. Backend API (AcmeLogisticsApi)

### Core Structure
* **Project File:** `AcmeLogisticsApi.csproj`
* **Entry Point:** `Program.cs`
* **Controllers:**
  * `DeviceController.cs`: Handles device twin connectivity, configurations, and remote command routing.
  * `TelemetryController.cs`: Manages ingestion stream metrics, logs, and operator alert states.
* **Services:**
  * `DeviceService`: Orchestrates digital twin state synchronizations.
  * `TelemetryService`: Houses calculations for thresholds, window caching, and alert files.
  * `TelemetryIngestService`: Background workers dedicated to handling message processing.
* **IoT Infrastructure Components:**
  * `ITwinRepository`, `RegistryTwinRepository`: Interfaces managing direct communication to backend identity registry stores.
  * `ServiceClient`: Native Azure SDK handler used to dispatch Cloud-to-Device (C2D) commands down to edge nodes.

### Data Flow & Endpoints
1. `TelemetryIngestService` is initialized within `Program.cs` and listens continuously to data streaming from Azure Event Hubs.
2. Upon processing new telemetry frames, it evaluates alerting rules and persists generated outputs to storage.
3. **Telemetry Endpoints (`TelemetryController`):**
  * `GET /api/telemetry/messages` - Fetches historical tracking rows.
  * `GET /api/telemetry/alerts` - Returns currently active alarms.
  * `GET /api/telemetry/summary` - Provides aggregate telemetry health scores.
  * `PUT /api/telemetry/alerts/{blobName}/acknowledge` - Settles an open alert.
4. **Device Endpoints (`DeviceController`):**
  * `POST /api/device/{deviceId}/command` - Directs low-level actions down to the target hardware.
  * `PUT /api/device/{deviceId}/config` - Pushes twin adjustments to target profiles.
  * `GET /api/device/{deviceId}/status` - Reports operational registry statuses.

### IoC Container & Environment Rules
* `Program.cs` registers dependencies as singletons for continuous application lifecycle tracking:
  * `AddSingleton<ITwinRepository, RegistryTwinRepository>()`
  * `AddSingleton<DeviceService>()`
  * `AddSingleton<TelemetryService>()`
  * `AddSingleton<TelemetryIngestService>(...)`
* **CORS Settings:** Configured to accept secure requests coming from localhost dev environments: `http://localhost:4200` (Angular) and `http://localhost:5173` (React).

---

## 2. Angular Frontend (AcmeLogisticsApp)

### Core Structure
* **Workspace Framework:** Angular Single Page Application.
* **Key Visual Components:**
  * `src/app/components/device-card/` - Card rendering device summary blocks.
  * `src/app/components/device-details/` - Time-series metrics and logs presentation views.
  * `src/app/components/alert-card/` - Operator alerts triggers with acknowledgement controls.
  * `src/app/components/settings/` - Modal view handling control overrides.
* **Data Integration Service:** `src/app/services/acme-logistics-service.ts`

### Service Consumption Layer
The `AcmeLogisticsService` wraps HTTP client pipelines to hook into backend endpoints:
* `getDeviceStatus(deviceId)`
* `getLatestTelemetryMessages(maxResults)`
* `getActiveAlerts(maxResults)`
* `getTelemetrySummary()`
* `sendCommand(deviceId, command)`
* `updateConfig(deviceId, desiredProps)`
* `acknowledgeAlert(blobName)`

### Component State Binding
* `device-card` consumes `getTelemetrySummary()` payload for fast overview reporting.
* `device-details` maps real-time lists extracted from historical messaging buckets.
* `alert-card` maps active triggers, processing style classes depending on whether an alert is flagged or acknowledged.
* `settings` binds data streams dynamically from status checks, enabling remote twin updates and immediate buzzer-silence command submissions.

### UI Implementation Details
* Clean, non-intrusive settings modal.
* Configured polling hooks refreshing critical data intervals across active components.
* Strict style mapping driven by `ngClass` to flash dynamic visual cues relative to alarm severity levels.
* Built utilizing standard structured elements (`CommonModule` integrations using `NgIf`, `NgForOf`, and `AsyncPipe` operators).

---

## 3. React Frontend (AcmeLogisticsAppReact)

### Core Structure
* **Workspace Framework:** React SPA driven by Vite compiler.
* **Key Functional Components:**
  * `src/components/AlertCard.tsx` - Displays triggered active alarm frames.
  * `src/components/DeviceDetails.tsx` - Focus dashboard covering specialized charts and tabular records.
  * `src/components/TemperatureCard.tsx` - Hero element visualizing current read-out metrics.
  * `src/components/Header.tsx` - Navigation framework and service status indicators.
* **API Service Layer:** `src/api/telemetryApi.ts`

### Consumed Endpoints
* `getSummary()`
* `getLatestMessages(maxResults)`
* `getActiveAlerts(maxResults)`

### Data Lifecycle Architecture
* **`App.tsx` Root Handler:**
  * Pulls `getSummary()` to populate global telemetry stats in `TemperatureCard`.
  * Pulls `getActiveAlerts()` to populate lists feeding downstream `AlertCard` blocks.
  * Evaluates layout contexts to handle explicit panel switches toward detailed module sub-views.
* **`DeviceDetails.tsx` Target View:**
  * Triggers explicit backend pulls fetching the latest telemetry messages.
  * Renders real-time telemetry inputs on historical grids instead of serving hardcoded mock components.

### UI Styling Design
* `TemperatureCard` shows the last available climate temperature and includes direct, clickable interaction paths.
* `AlertCard` handles precise UI variations matching active severity states, adapting templates to match critical versus resolved tags.

---

## 4. Local IoT Device Agent (agent)

### Core Structure
* **Project File:** `ColdChain.Agent.csproj` (.NET Core Application Console Engine)
* **Key Logic Modules:**
  * `Program.cs` - Initializes application cycles, bootstrapping environmental settings.
  * `DeviceClientHost.cs` - Manages long-lived connections, handling packet transport states securely.
  * `DeviceState.cs` - Holds tracking metrics, tracking dynamic status criteria.
  * `DownlinkHandler.cs` - Intercepts inbound streams, parsing cloud requests into actionable edge changes.
  * `TelemetryModels.cs` - Validates tracking contracts against outbound serialization structures.

### Functional Roles
* Simulates physical transit pharmaceutical payload tracking behaviors over local environments.
* Interacts directly over Azure IoT Hub utilizing a secure AMQP/MQTT `DeviceClient` pipeline.
* Transmits periodic telemetry packets containing device telemetry fields.
* Audits local environment status, keeping the physical sound alarm updated depending on `TemperatureThresholdC` metrics.
* Receives, processes, and reflects changes triggered by C2D directives like `activate_buzzer` and `silence_buzzer`.

### Core Logic Snippets
* **`DeviceState.EvaluateBuzzer()`**: Compares `LastTemperatureC` records against retrieved `TemperatureThresholdC` settings to flags alarm responses.
* **`DeviceClientHost.RunTelemetryLoopAsync()`**: Manages the main hardware timer, refreshing ambient readings, evaluating conditions, and dispatching JSON payloads up to the cloud gateway.
* **`DownlinkHandler.HandleCloudToDeviceMessageAsync()`**: Evaluates incoming cloud commands and mutates device memory settings appropriately.

---

## 5. End-to-End Operational Lifecycle

[ Local Device Agent ]
    | (Simulates transit environment metrics & tracks local baseline limits)
    |--- [Uplink Telemetry] ---> [ Azure IoT Hub ]
                                      |
                                      | (Event processor picks up raw message payloads)
                                      v
                             [ AcmeLogisticsApi ]
                                      |
                                      |---> Processed via Telemetry & Device Services
                                      |     (Validates limits, tracks state, and logs history)
                                      |
                                      v
                             [ REST Controllers ] <--- [ Angular Frontend ] (Port 4200)
                                    |     |       <--- [ React Frontend ]   (Port 5173)
                                    |     |
                                    |     +---> Reports Summaries, Alarms & Historical Trends
                                    v
                       [ Direct Methods / Twins ]
                                    |
                                    +--- [Downlink Commands] ---> [ Local Device Agent ]
                                                                       (Silences local buzzer &
                                                                        updates configurations)

---

## 6. Key Source File Reference Matrix

For fast troubleshooting, prioritize tracking operations inside these baseline paths:

* **Backend Services Layer:**
  * `Program.cs` - Check dependency container lifecycles and middleware route definitions.
  * `DeviceController.cs` - Trace incoming twin edits and downlink action command processing.
  * `TelemetryController.cs` - Inspect real-time data feeds and operator acknowledgment tasks.
* **Angular Workspace Structure:**
  * `acme-logistics-service.ts` - Inspect layout configurations wrapping endpoint communication pipelines.
  * `alert-card` - Review visual template states evaluating tracking alarms.
  * `settings` - Modify structures feeding operator interactive modal dialog boxes.
* **React Workspace Structure:**
  * `telemetryApi.ts` - Adjust queries parsing remote json structures.
  * `App.tsx` - Manage orchestration states and navigation logic.
  * `DeviceDetails.tsx` - Customize graphs and history logs.
* **Hardware Agent Simulator Core:**
  * `DeviceState.cs` - Adjust evaluation conditions changing buzzer flags.
  * `DeviceClientHost.cs` - Manage outbound intervals and network connections.
  * `DownlinkHandler.cs` - Track custom string actions intercepted via the cloud loop.

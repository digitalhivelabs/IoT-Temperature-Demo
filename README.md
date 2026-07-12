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

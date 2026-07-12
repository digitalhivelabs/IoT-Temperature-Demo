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
dotnet run --project agent -- status
---

## 📝 Key Notes & Configuration

* AcmeBrunoCollections: This directory contains API testing collections (such as Bruno or Postman) and is not an executable application.
* Environment Variables: All applications rely on template configuration files (e.g., .env.example or appsettings.example.json). Make sure to create your local copies without the .example extension and populate them with your Azure IoT Hub connection strings before spinning up the services.



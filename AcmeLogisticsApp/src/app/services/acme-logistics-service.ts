import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface DeviceStatus {
  deviceId: string;
  desired: Record<string, unknown>;
  reported: Record<string, unknown>;
  tags: Record<string, unknown>;
  etag: string;
}

export interface TelemetryMessage {
  deviceId: string;
  temperatureC: number;
  humidityPct: number;
  buzzerActive: boolean;
  sequenceNumber: number;
  timestamp: string;
}

export interface AlertMessage {
  deviceId: string;
  temperatureC: number;
  humidityPct: number;
  buzzerActive: boolean;
  sequenceNumber: number;
  timestamp: string;
  temperatureThreshold: number;
  message: string;
  acknowledged: boolean;
  blobName: string;
}

export interface TelemetrySummary {
  lastTemperature: number | null;
  readsCount: number;
  alertsCount: number;
}

@Injectable({
  providedIn: 'root',
})
export class AcmeLogisticsService {
  private readonly apiBase = 'http://localhost:5001/api';

  constructor(private readonly http: HttpClient) {}

  sendCommand(deviceId: string, command: unknown): Observable<unknown> {
    return this.http.post(`${this.apiBase}/device/${deviceId}/command`, command);
  }

  updateConfig(deviceId: string, desiredProps: Record<string, unknown>): Observable<unknown> {
    return this.http.put(`${this.apiBase}/device/${deviceId}/config`, desiredProps);
  }

  getDeviceStatus(deviceId: string): Observable<DeviceStatus> {
    return this.http.get<DeviceStatus>(`${this.apiBase}/device/${deviceId}/status`);
  }

  getLatestTelemetryMessages(maxResults = 10): Observable<TelemetryMessage[]> {
    return this.http.get<TelemetryMessage[]>(`${this.apiBase}/telemetry/messages?maxResults=${maxResults}`);
  }

  getActiveAlerts(maxResults = 10): Observable<AlertMessage[]> {
    return this.http.get<AlertMessage[]>(`${this.apiBase}/telemetry/alerts?maxResults=${maxResults}`);
  }

  acknowledgeAlert(blobName: string): Observable<AlertMessage> {
    return this.http.put<AlertMessage>(`${this.apiBase}/telemetry/alerts/${blobName}/acknowledge`, {});
  }

  getTelemetrySummary(): Observable<TelemetrySummary> {
    return this.http.get<TelemetrySummary>(`${this.apiBase}/telemetry/summary`);
  }
}

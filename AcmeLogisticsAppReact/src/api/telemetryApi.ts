const API_BASE = 'http://localhost:5001/api';

export interface TelemetrySummary {
  lastTemperature: number | null;
  readsCount: number;
  alertsCount: number;
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
  blobName: string | null;
}

export async function getSummary(): Promise<TelemetrySummary> {
  const res = await fetch(`${API_BASE}/telemetry/summary`);
  if (!res.ok) throw new Error('Failed to fetch summary');
  return res.json();
}

export async function getLatestMessages(maxResults = 10): Promise<TelemetryMessage[]> {
  const res = await fetch(`${API_BASE}/telemetry/messages?maxResults=${maxResults}`);
  if (!res.ok) throw new Error('Failed to fetch messages');
  return res.json();
}

export async function getActiveAlerts(maxResults = 10): Promise<AlertMessage[]> {
  const res = await fetch(`${API_BASE}/telemetry/alerts?maxResults=${maxResults}`);
  if (!res.ok) throw new Error('Failed to fetch alerts');
  return res.json();
}

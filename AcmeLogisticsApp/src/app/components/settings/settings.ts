import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AcmeLogisticsService, DeviceStatus } from '../../services/acme-logistics-service';

@Component({
  standalone: true,
  selector: 'app-settings',
  imports: [CommonModule, FormsModule],
  templateUrl: './settings.html',
  styleUrl: './settings.css',
})
export class Settings implements OnInit {
  readonly deviceId = 'temperature-pharma-001';

  status = signal<DeviceStatus | null>(null);
  loading = signal(false);
  saving = signal(false);
  commanding = signal(false);
  successMsg = signal<string | null>(null);
  errorMsg = signal<string | null>(null);

  thresholdInput = 8;

  constructor(private readonly svc: AcmeLogisticsService) {}

  ngOnInit(): void {
    this.loadStatus();
  }

  loadStatus(): void {
    this.loading.set(true);
    this.clearMessages();
    this.svc.getDeviceStatus(this.deviceId).subscribe({
      next: s => {
        this.status.set(s);
        const t = s.desired['temperatureThresholdC'];
        if (t != null) this.thresholdInput = Number(t);
        this.loading.set(false);
      },
      error: () => {
        this.errorMsg.set('No se pudo obtener el estado del dispositivo.');
        this.loading.set(false);
      },
    });
  }

  saveThreshold(): void {
    this.saving.set(true);
    this.clearMessages();
    this.svc.updateConfig(this.deviceId, { temperatureThresholdC: this.thresholdInput }).subscribe({
      next: () => {
        this.successMsg.set('Umbral actualizado correctamente.');
        this.saving.set(false);
        this.loadStatus();
      },
      error: () => {
        this.errorMsg.set('Error al actualizar la configuración.');
        this.saving.set(false);
      },
    });
  }

  sendCommand(command: string): void {
    this.commanding.set(true);
    this.clearMessages();
    this.svc.sendCommand(this.deviceId, { command }).subscribe({
      next: () => {
        this.successMsg.set(`Comando "${command}" enviado.`);
        this.commanding.set(false);
      },
      error: () => {
        this.errorMsg.set('Error al enviar el comando.');
        this.commanding.set(false);
      },
    });
  }

  private clearMessages(): void {
    this.successMsg.set(null);
    this.errorMsg.set(null);
  }

  twinEntries(obj: Record<string, unknown>): { key: string; value: string }[] {
    return Object.entries(obj)
      .filter(([k]) => !k.startsWith('$'))
      .map(([k, v]) => ({ key: k, value: v != null ? String(v) : '—' }));
  }
}

import { Component, inject, signal } from '@angular/core';
import { NgIf } from '@angular/common';
import { DeviceDetails } from '../../components/device-details/device-details';
import { AcmeLogisticsService, TelemetrySummary } from '../../services/acme-logistics-service';

@Component({
  standalone: true,
  selector: 'app-device-card',
  imports: [NgIf, DeviceDetails],
  templateUrl: './device-card.html',
  styleUrl: './device-card.css',
})
export class DeviceCard {
  private readonly acmeLogisticsService = inject(AcmeLogisticsService);

  protected readonly deviceDetailsOpen = signal(false);
  protected readonly telemetrySummary = signal<TelemetrySummary | null>(null);

  constructor() {
    this.loadTelemetrySummary();
  }

  protected openDeviceDetails(): void {
    this.deviceDetailsOpen.set(true);
  }

  protected closeDeviceDetails(): void {
    this.deviceDetailsOpen.set(false);
  }

  private loadTelemetrySummary(): void {
    this.acmeLogisticsService.getTelemetrySummary().subscribe({
      next: (summary) => this.telemetrySummary.set(summary),
      error: () => this.telemetrySummary.set(null),
    });
  }
}

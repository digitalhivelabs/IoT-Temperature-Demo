import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { AcmeLogisticsService, TelemetryMessage } from '../../services/acme-logistics-service';

@Component({
  standalone: true,
  selector: 'app-device-details',
  imports: [CommonModule],
  templateUrl: './device-details.html',
  styleUrl: './device-details.css',
})
export class DeviceDetails {
  private readonly acmeLogisticsService = inject(AcmeLogisticsService);

  protected readonly telemetryMessages$ = this.acmeLogisticsService.getLatestTelemetryMessages();
}

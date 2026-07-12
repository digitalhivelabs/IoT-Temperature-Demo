import { Component, signal } from '@angular/core';
import { NgIf } from '@angular/common';
import { DeviceDetails } from '../../components/device-details/device-details';

@Component({
  standalone: true,
  selector: 'app-device-card',
  imports: [NgIf, DeviceDetails],
  templateUrl: './device-card.html',
  styleUrl: './device-card.css',
})
export class DeviceCard {
  protected readonly deviceDetailsOpen = signal(false);

  protected openDeviceDetails(): void {
    this.deviceDetailsOpen.set(true);
  }

  protected closeDeviceDetails(): void {
    this.deviceDetailsOpen.set(false);
  }
}

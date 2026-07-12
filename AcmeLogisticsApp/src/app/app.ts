import { Component, signal } from '@angular/core';
import { NgIf } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { DeviceCard } from './components/device-card/device-card';
import { Sidebar } from './layout/sidebar/sidebar';
import { Settings } from './component/settings/settings';
import { Navbar } from './layout/navbar/navbar';

@Component({
  standalone: true,
  selector: 'app-root',
  imports: [RouterOutlet, NgIf, DeviceCard, Sidebar, Settings, Navbar],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('AcmeLogisticsApp');
  protected readonly settingsOpen = signal(false);

  protected openSettingsModal(): void {
    this.settingsOpen.set(true);
  }

  protected closeSettingsModal(): void {
    this.settingsOpen.set(false);
  }
}

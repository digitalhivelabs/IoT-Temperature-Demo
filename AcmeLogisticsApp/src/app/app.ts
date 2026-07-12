import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { AsyncPipe, NgForOf, NgIf } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { BehaviorSubject, interval, Subscription, switchMap } from 'rxjs';
import { DeviceCard } from './components/device-card/device-card';
import { Sidebar } from './layout/sidebar/sidebar';
import { Settings } from './components/settings/settings';
import { Navbar } from './layout/navbar/navbar';
import { AlertCard } from './components/alert-card/alert-card';
import { AcmeLogisticsService, AlertMessage } from './services/acme-logistics-service';

@Component({
  standalone: true,
  selector: 'app-root',
  imports: [RouterOutlet, NgIf, NgForOf, AsyncPipe, DeviceCard, Sidebar, Settings, Navbar, AlertCard],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit, OnDestroy {
  protected readonly title = signal('AcmeLogisticsApp');
  protected readonly settingsOpen = signal(false);
  protected readonly alerts$ = new BehaviorSubject<AlertMessage[]>([]);

  private readonly POLL_INTERVAL_MS = 15_000;
  private pollSub?: Subscription;

  constructor(private readonly svc: AcmeLogisticsService) {}

  ngOnInit(): void {
    this.loadAlerts();
    this.pollSub = interval(this.POLL_INTERVAL_MS)
      .pipe(switchMap(() => this.svc.getActiveAlerts()))
      .subscribe(alerts => this.alerts$.next(alerts));
  }

  ngOnDestroy(): void {
    this.pollSub?.unsubscribe();
  }

  protected loadAlerts(): void {
    this.svc.getActiveAlerts().subscribe(alerts => this.alerts$.next(alerts));
  }

  protected onAcknowledge(blobName: string): void {
    this.svc.acknowledgeAlert(blobName).subscribe(() => this.loadAlerts());
  }

  protected openSettingsModal(): void {
    this.settingsOpen.set(true);
  }

  protected closeSettingsModal(): void {
    this.settingsOpen.set(false);
  }

  protected trackByBlob(_: number, alert: AlertMessage): string {
    return alert.blobName ?? alert.timestamp;
  }
}

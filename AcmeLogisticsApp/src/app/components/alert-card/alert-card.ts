import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AlertMessage } from '../../services/acme-logistics-service';

@Component({
  standalone: true,
  selector: 'app-alert-card',
  imports: [CommonModule],
  templateUrl: './alert-card.html',
  styleUrl: './alert-card.css',
})
export class AlertCard {
  @Input() alert!: AlertMessage;
  @Output() acknowledge = new EventEmitter<string>();

  onAcknowledge(): void {
    if (this.alert?.blobName) {
      this.acknowledge.emit(this.alert.blobName);
    }
  }
}

import {Component} from '@angular/core';

@Component({
  selector: 'app-settings',
  standalone: true,
  template: `
    <div class="page">
      <div class="page-header">
        <h1 class="page-title">Einstellungen</h1>
        <p class="page-subtitle">Systemkonfiguration.</p>
      </div>
      <div class="placeholder-card">
        <p>Einstellungen werden in einer zukünftigen Version implementiert.</p>
      </div>
    </div>
  `,
  styles: [':host { display: block; }']
})
export class SettingsComponent {
}



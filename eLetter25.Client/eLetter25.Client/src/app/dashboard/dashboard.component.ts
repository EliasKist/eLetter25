import { Component } from '@angular/core';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  template: `
    <div class="page">
      <div class="page-header">
        <h1 class="page-title">Dashboard</h1>
        <p class="page-subtitle">Willkommen bei eLetter25.</p>
      </div>
      <div class="placeholder-card">
        <p>Hier wird zukünftig eine Übersicht der neuesten Briefe und Aktivitäten angezeigt.</p>
      </div>
    </div>
  `,
  styles: [':host { display: block; }']
})
export class DashboardComponent {}



import { Component } from '@angular/core';

@Component({
  selector: 'app-letters-list',
  standalone: true,
  template: `
    <div class="page">
      <div class="page-header">
        <h1 class="page-title">Briefübersicht</h1>
        <p class="page-subtitle">Alle erfassten Briefe.</p>
      </div>
      <div class="placeholder-card">
        <p>Die Briefübersicht wird in einer zukünftigen Version implementiert.</p>
      </div>
    </div>
  `,
  styles: [':host { display: block; }']
})
export class LettersListComponent {}



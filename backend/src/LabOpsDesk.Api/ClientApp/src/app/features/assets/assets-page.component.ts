import { Component } from '@angular/core';

@Component({
  selector: 'app-assets-page',
  standalone: true,
  template: `
    <section class="page">
      <header class="toolbar">
        <h1>Assets</h1>
      </header>
    </section>
  `,
  styles: [
    `
      .page {
        padding: 1.5rem;
      }
      .toolbar {
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: 1rem;
      }
    `,
  ],
})
export class AssetsPageComponent {}

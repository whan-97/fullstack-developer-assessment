import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  standalone: true,
  template: `
    <section class="page">
      <h1>LabOps Desk</h1>
      <p>Track lab assets and spare parts for sequencing operations.</p>
    </section>
  `,
  styles: [
    `
      .page {
        padding: 1.5rem;
      }
    `,
  ],
})
export class HomeComponent {}

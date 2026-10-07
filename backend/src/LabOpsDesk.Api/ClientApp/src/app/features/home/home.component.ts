import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AssetApiService } from '../../core/asset-api.service';
import { PartApiService } from '../../core/part-api.service';
import { Asset, Part } from '../../core/models';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <section class="page">
      <header class="toolbar">
        <div>
          <h1>LabOps Desk</h1>
          <p class="subtitle">Track lab assets and spare parts for sequencing operations</p>
        </div>
      </header>

      <div class="stats-grid">
        <div class="stat-card">
          <span class="stat-label">Total Assets</span>
          <span class="stat-value">{{ isLoading ? '—' : totalAssets }}</span>
        </div>
        <div class="stat-card">
          <span class="stat-label">Assets In Use</span>
          <span class="stat-value text-info">{{ isLoading ? '—' : assetsInUse }}</span>
        </div>
        <div class="stat-card">
          <span class="stat-label">Total Parts</span>
          <span class="stat-value">{{ isLoading ? '—' : totalParts }}</span>
        </div>
        <div class="stat-card" [class.stat-alert]="lowStockParts > 0">
          <span class="stat-label">Low Stock Warnings</span>
          <span class="stat-value" [class.text-danger]="lowStockParts > 0">
            {{ isLoading ? '—' : lowStockParts }}
          </span>
        </div>
      </div>

      <div class="card-grid">
        <div class="dashboard-card">
          <div class="card-header">
            <h2>Assets Management</h2>
            <span class="badge badge-subtle">JSON Storage</span>
          </div>
          <p class="card-body">
            Monitor lab equipment, update lifecycle statuses, and process checkouts and check-ins with assigned personnel.
          </p>
          <a routerLink="/assets" class="btn btn-primary">Manage Assets &rarr;</a>
        </div>

        <div class="dashboard-card">
          <div class="card-header">
            <h2>Parts & Reagents</h2>
            <span class="badge badge-subtle">CSV Storage</span>
          </div>
          <p class="card-body">
            Track stock levels, monitor reorder thresholds, adjust inventory quantities, and identify low stock items.
          </p>
          <a routerLink="/parts" class="btn btn-primary">Manage Inventory &rarr;</a>
        </div>
      </div>
    </section>
  `,
  styles: [`
    .page { padding: 2rem 1.5rem; max-width: 1200px; margin: 0 auto; color: #111827; }
    .toolbar { margin-bottom: 1.5rem; }
    h1 { font-size: 1.5rem; font-weight: 600; margin: 0; }
    h2 { font-size: 1.125rem; font-weight: 600; margin: 0; }
    .subtitle { color: #6b7280; font-size: 0.875rem; margin-top: 0.25rem; }

    .stats-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(180px, 1fr)); gap: 0.85rem; margin-bottom: 1.5rem; }
    .stat-card { background: #fff; border: 1px solid #e5e7eb; border-radius: 6px; padding: 1rem; display: flex; flex-direction: column; }
    .stat-card.stat-alert { background: #fef2f2; border-color: #fee2e2; }
    .stat-label { font-size: 0.8rem; font-weight: 500; color: #4b5563; margin-bottom: 0.35rem; }
    .stat-value { font-size: 1.5rem; font-weight: 700; color: #111827; }
    .text-info { color: #1e40af; }
    .text-danger { color: #dc2626; }

    .card-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(300px, 1fr)); gap: 1rem; }
    .dashboard-card { background: #fff; border: 1px solid #e5e7eb; border-radius: 6px; padding: 1.25rem; display: flex; flex-direction: column; justify-content: space-between; }
    .card-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 0.5rem; }
    .card-body { color: #4b5563; font-size: 0.875rem; line-height: 1.4; margin: 0 0 1.25rem 0; }

    .badge { padding: 0.15rem 0.5rem; border-radius: 4px; font-size: 0.75rem; font-weight: 500; border: 1px solid transparent; }
    .badge-subtle { background: #f3f4f6; color: #4b5563; border-color: #e5e7eb; }

    .btn { padding: 0.45rem 0.85rem; border-radius: 5px; font-weight: 500; text-decoration: none; font-size: 0.85rem; display: inline-block; width: fit-content; }
    .btn-primary { background: #1f2937; color: #fff; border: 1px solid transparent; }
  `]
})
export class HomeComponent implements OnInit {
  private assetService = inject(AssetApiService);
  private partService = inject(PartApiService);

  isLoading = true;
  totalAssets = 0;
  assetsInUse = 0;
  totalParts = 0;
  lowStockParts = 0;

  ngOnInit(): void {
    this.loadSummaryMetrics();
  }

  private loadSummaryMetrics(): void {
    this.isLoading = true;

    this.assetService.list().subscribe({
      next: (assets: Asset[]) => {
        this.totalAssets = assets.length;
        this.assetsInUse = assets.filter(a => a.status === 'InUse').length;
      },
      error: () => {
        this.totalAssets = 0;
        this.assetsInUse = 0;
      }
    });

    this.partService.list().subscribe({
      next: (parts: Part[]) => {
        this.totalParts = parts.length;
        this.lowStockParts = parts.filter(p => p.isLowStock).length;
        this.isLoading = false;
      },
      error: () => {
        this.totalParts = 0;
        this.lowStockParts = 0;
        this.isLoading = false;
      }
    });
  }
}

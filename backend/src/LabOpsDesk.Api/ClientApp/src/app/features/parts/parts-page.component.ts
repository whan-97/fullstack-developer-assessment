import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PartApiService } from '../../core/part-api.service';
import { Part, CreatePartRequest, UpdatePartRequest } from '../../core/models';
import { readApiError } from '../../core/api-error';

export interface PartFormData {
  sku: string;
  name: string;
  category: string;
  unitOfMeasure: string;
  quantityOnHand: number;
  reorderThreshold: number;
  location: string;
}

@Component({
  selector: 'app-parts-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <section class="page">
      <header class="toolbar">
        <div>
          <h1>Parts & Reagents</h1>
          <p class="subtitle">Monitor inventory levels, stock reorders, and location distribution</p>
        </div>
        <button class="btn btn-primary" (click)="openForm()">+ Add Part</button>
      </header>

      <div *ngIf="errorMessage" class="alert alert-error">
        <span>{{ errorMessage }}</span>
        <button class="close-btn" (click)="errorMessage = null">&times;</button>
      </div>
      <div *ngIf="successMessage" class="alert alert-success">
        <span>{{ successMessage }}</span>
        <button class="close-btn" (click)="successMessage = null">&times;</button>
      </div>

      <div *ngIf="isLoading" class="center-state"><div class="spinner"></div><p>Loading inventory...</p></div>
      <div *ngIf="!isLoading && !parts.length" class="center-state">
        <p>No parts found in inventory.</p>
        <button class="btn btn-secondary" (click)="openForm()">Create First Part</button>
      </div>

      <div *ngIf="!isLoading && parts.length > 0" class="table-container">
        <table class="data-table">
          <thead>
            <tr>
              <th>SKU</th><th>Name</th><th>Category</th><th>Location</th>
              <th>Quantity On Hand</th><th>Reorder Threshold</th><th class="text-right">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let part of parts" [class.low-stock]="part.isLowStock">
              <td><code>{{ part.sku }}</code></td>
              <td class="font-medium">{{ part.name }}</td>
              <td>{{ part.category }}</td>
              <td>{{ part.location }}</td>
              <td>
                <div class="qty-ctrl">
                  <button class="btn-qty" (click)="adjustQuantity(part, -1)">-</button>
                  <span class="qty-label" [class.text-danger]="part.isLowStock">
                    {{ part.quantityOnHand }} <span class="unit">{{ part.unitOfMeasure }}</span>
                  </span>
                  <button class="btn-qty" (click)="adjustQuantity(part, 1)">+</button>
                </div>
              </td>
              <td>
                {{ part.reorderThreshold }}
                <span *ngIf="part.isLowStock" class="badge badge-warning">Low Stock</span>
              </td>
              <td class="actions-cell">
                <button class="btn btn-sm btn-ghost" (click)="openForm(part)">Edit</button>
                <button class="btn btn-sm btn-ghost-danger" (click)="openDeleteModal(part)">Delete</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <div *ngIf="showForm" class="modal-backdrop">
        <div class="modal-card">
          <h2>{{ activePart ? 'Edit Part' : 'Create Part' }}</h2>

          <div *ngIf="modalErrorMessage" class="alert alert-error">
            <span>{{ modalErrorMessage }}</span>
            <button class="close-btn" (click)="modalErrorMessage = null">&times;</button>
          </div>

          <form (ngSubmit)="savePart()">
            <div class="form-group" *ngIf="!activePart">
              <label>SKU *</label>
              <input type="text" class="form-control" [(ngModel)]="form.sku" name="sku" placeholder="e.g. PRT-500" />
            </div>
            <div class="form-group">
              <label>Name *</label>
              <input type="text" class="form-control" [(ngModel)]="form.name" name="name" placeholder="e.g. Flow Cell Cartridge" />
            </div>
            <div class="form-row">
              <div class="form-group">
                <label>Category *</label>
                <input type="text" class="form-control" [(ngModel)]="form.category" name="category" placeholder="e.g. Consumables" />
              </div>
              <div class="form-group">
                <label>Unit *</label>
                <input type="text" class="form-control" [(ngModel)]="form.unitOfMeasure" name="unitOfMeasure" placeholder="e.g. Box, Units" />
              </div>
            </div>
            <div class="form-row">
              <div class="form-group">
                <label>Quantity *</label>
                <input type="number" class="form-control" [(ngModel)]="form.quantityOnHand" name="quantityOnHand" />
              </div>
              <div class="form-group">
                <label>Reorder Level *</label>
                <input type="number" class="form-control" [(ngModel)]="form.reorderThreshold" name="reorderThreshold" />
              </div>
            </div>
            <div class="form-group">
              <label>Location *</label>
              <input type="text" class="form-control" [(ngModel)]="form.location" name="location" placeholder="e.g. Shelf B2" />
            </div>
            <div class="modal-actions">
              <button type="button" class="btn btn-secondary" (click)="closeModals()">Cancel</button>
              <button type="submit" class="btn btn-primary" [disabled]="isSubmitting">
                {{ isSubmitting ? 'Saving...' : 'Save Part' }}
              </button>
            </div>
          </form>
        </div>
      </div>

      <div *ngIf="showDelete" class="modal-backdrop">
        <div class="modal-card">
          <h2>Delete Part</h2>
          <p class="subtitle">Are you sure you want to delete <code>{{ activePart?.sku }}</code> ({{ activePart?.name }})?</p>

          <div *ngIf="modalErrorMessage" class="alert alert-error">
            <span>{{ modalErrorMessage }}</span>
            <button class="close-btn" (click)="modalErrorMessage = null">&times;</button>
          </div>

          <div class="modal-actions">
            <button type="button" class="btn btn-secondary" (click)="closeModals()">Cancel</button>
            <button type="button" class="btn btn-danger" [disabled]="isSubmitting" (click)="deletePart()">
              {{ isSubmitting ? 'Deleting...' : 'Delete' }}
            </button>
          </div>
        </div>
      </div>
    </section>
  `,
  styles: [`
    .page { padding: 2rem 1.5rem; max-width: 1400px; margin: 0 auto; color: #111827; }
    .toolbar { display: flex; align-items: center; justify-content: space-between; margin-bottom: 1.5rem; }
    h1 { font-size: 1.5rem; font-weight: 600; margin: 0; }
    h2 { font-size: 1.125rem; font-weight: 600; margin: 0 0 0.5rem; }
    .subtitle { color: #6b7280; font-size: 0.875rem; margin-top: 0.25rem; }
    code { font-family: monospace; font-size: 0.85em; background: #f3f4f6; padding: 0.15rem 0.35rem; border-radius: 4px; }

    .alert { padding: 0.75rem 1rem; border-radius: 6px; margin-bottom: 1.25rem; display: flex; justify-content: space-between; font-size: 0.875rem; }
    .alert-error { background: #fef2f2; color: #991b1b; border: 1px solid #fee2e2; }
    .alert-success { background: #f0fdf4; color: #166534; border: 1px solid #dcfce7; }
    .close-btn { background: none; border: none; cursor: pointer; color: inherit; opacity: 0.7; }

    .table-container { background: #fff; border: 1px solid #e5e7eb; border-radius: 6px; overflow-x: auto; }
    .data-table { width: 100%; border-collapse: collapse; text-align: left; font-size: 0.875rem; }
    .data-table th { background: #f9fafb; font-weight: 500; color: #4b5563; padding: 0.65rem 1rem; border-bottom: 1px solid #e5e7eb; }
    .data-table td { padding: 0.75rem 1rem; border-bottom: 1px solid #f3f4f6; color: #374151; vertical-align: middle; }
    .low-stock { background-color: #fff1f2; }

    .qty-ctrl { display: flex; align-items: center; gap: 0.4rem; }
    .btn-qty { width: 22px; height: 22px; border: 1px solid #d1d5db; background: #fff; border-radius: 4px; cursor: pointer; }
    .qty-label { min-width: 65px; text-align: center; font-weight: 500; }
    .unit { font-size: 0.75rem; color: #6b7280; font-weight: 400; }

    .badge-warning { background: #fffbeb; color: #92400e; border: 1px solid #fef3c7; padding: 0.15rem 0.5rem; border-radius: 4px; font-size: 0.75rem; margin-left: 0.5rem; }
    .text-danger { color: #dc2626; font-weight: 600; }
    .actions-cell { display: flex; gap: 0.35rem; justify-content: flex-end; }

    .btn { padding: 0.45rem 0.85rem; border-radius: 5px; font-weight: 500; cursor: pointer; border: 1px solid transparent; font-size: 0.85rem; }
    .btn-sm { padding: 0.25rem 0.55rem; font-size: 0.775rem; }
    .btn-primary { background: #1f2937; color: #fff; }
    .btn-secondary { background: #fff; color: #374151; border-color: #d1d5db; }
    .btn-ghost { background: transparent; color: #4b5563; }
    .btn-ghost-danger { background: transparent; color: #dc2626; }
    .btn-danger { background: #dc2626; color: #fff; }

    .modal-backdrop { position: fixed; inset: 0; background: rgba(0, 0, 0, 0.4); display: flex; align-items: center; justify-content: center; z-index: 1000; }
    .modal-card { background: #fff; padding: 1.5rem; border-radius: 8px; width: 100%; max-width: 440px; border: 1px solid #e5e7eb; }
    .form-group { margin-bottom: 0.875rem; }
    .form-row { display: flex; gap: 0.75rem; }
    .form-row .form-group { flex: 1; }
    .form-group label { display: block; margin-bottom: 0.3rem; font-size: 0.8rem; font-weight: 500; }
    .form-control { width: 100%; padding: 0.45rem 0.6rem; border: 1px solid #d1d5db; border-radius: 5px; font-size: 0.875rem; box-sizing: border-box; }
    .modal-actions { display: flex; justify-content: flex-end; gap: 0.5rem; margin-top: 1.25rem; }

    .text-right { text-align: right; }
    .font-medium { font-weight: 500; color: #111827; }
    .center-state { text-align: center; padding: 3rem; color: #6b7280; font-size: 0.875rem; }
    .spinner { border: 2px solid #e5e7eb; border-top: 2px solid #4b5563; border-radius: 50%; width: 20px; height: 20px; animation: spin 0.8s linear infinite; margin: 0 auto 0.75rem; }
    @keyframes spin { to { transform: rotate(360deg); } }
  `]
})
export class PartsPageComponent implements OnInit {
  private partService = inject(PartApiService);

  parts: Part[] = [];
  isLoading = false;
  isSubmitting = false;
  showForm = false;
  showDelete = false;

  errorMessage: string | null = null;
  modalErrorMessage: string | null = null;
  successMessage: string | null = null;
  activePart: Part | null = null;

  form: PartFormData = this.emptyForm();

  ngOnInit(): void { this.loadParts(); }

  private emptyForm(): PartFormData {
    return { sku: '', name: '', category: '', unitOfMeasure: 'Units', quantityOnHand: 0, reorderThreshold: 0, location: '' };
  }

  loadParts(): void {
    this.isLoading = true;
    this.errorMessage = null;
    this.partService.list().subscribe({
      next: (data) => { this.parts = data; this.isLoading = false; },
      error: (err) => {
        this.handleError(err, false);
        if (!this.parts.length) {
          this.errorMessage = null;
        }
        this.isLoading = false;
      }
    });
  }

  openForm(part?: Part): void {
    this.activePart = part || null;
    this.form = part ? { ...part } : this.emptyForm();
    this.modalErrorMessage = null;
    this.showForm = true;
  }

  openDeleteModal(part: Part): void {
    this.activePart = part;
    this.modalErrorMessage = null;
    this.showDelete = true;
  }

  closeModals(): void {
    this.showForm = this.showDelete = false;
    this.activePart = null;
    this.modalErrorMessage = null;
  }

  savePart(): void {
    const sku = this.form.sku?.trim();
    const name = this.form.name?.trim();
    const category = this.form.category?.trim();
    const unitOfMeasure = this.form.unitOfMeasure?.trim();
    const location = this.form.location?.trim();
    const quantityOnHand = this.form.quantityOnHand;
    const reorderThreshold = this.form.reorderThreshold;

    if (!this.activePart && !sku) {
      this.modalErrorMessage = 'SKU is required.';
      return;
    }


    if (quantityOnHand < 0) {
      this.modalErrorMessage = 'Quantity cannot be below 0'
      return;
    }

    this.isSubmitting = true;
    this.modalErrorMessage = null;

    const payload = {
      ...this.form,
      sku,
      name,
      category,
      unitOfMeasure,
      location,
      quantityOnHand: Number(this.form.quantityOnHand),
      reorderThreshold: Number(this.form.reorderThreshold)
    };

    const request$ = this.activePart
      ? this.partService.update(this.activePart.id, payload as UpdatePartRequest)
      : this.partService.create(payload as CreatePartRequest);

    request$.subscribe({
      next: () => {
        this.successMessage = `Part ${this.activePart ? 'updated' : 'created'} successfully.`;
        this.isSubmitting = false;
        this.closeModals();
        this.loadParts();
      },
      error: (err) => {
        this.handleError(err, true);
        this.isSubmitting = false;
      }
    });
  }

  adjustQuantity(part: Part, delta: number): void {
    this.errorMessage = null;
    this.partService.adjust(part.id, { delta }).subscribe({
      next: (res) => { part.quantityOnHand = res.quantityOnHand; part.isLowStock = res.isLowStock; },
      error: (err) => this.handleError(err, false)
    });
  }

  deletePart(): void {
    if (!this.activePart) return;
    this.isSubmitting = true;
    this.modalErrorMessage = null;

    this.partService.remove(this.activePart.id).subscribe({
      next: () => {
        this.successMessage = 'Part deleted successfully.';
        this.isSubmitting = false;
        this.closeModals();
        this.loadParts();
      },
      error: (err) => {
        this.handleError(err, true);
        this.isSubmitting = false;
      }
    });
  }

  private handleError(err: unknown, isModal = false): void {
    const apiError = readApiError(err);

    if (isModal) {
      this.modalErrorMessage = apiError.message;
    } else {
      this.errorMessage = apiError.message;
    }
  }
}

import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AssetApiService } from '../../core/asset-api.service';
import { Asset, AssetStatus, CreateAssetRequest, UpdateAssetRequest } from '../../core/models';
import { readApiError } from '../../core/api-error';

export interface AssetFormData {
  assetTag: string;
  name: string;
  platform: string;
  location: string;
  status: AssetStatus;
  notes: string;
}

@Component({
  selector: 'app-assets-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <section class="page">
      <header class="toolbar">
        <div>
          <h1>Assets</h1>
          <p class="subtitle">Manage lab equipment, status transitions, and checkouts</p>
        </div>
        <button class="btn btn-primary" (click)="openForm()">+ Add Asset</button>
      </header>

      <div *ngIf="errorMessage" class="alert alert-error">
        <span>{{ errorMessage }}</span>
        <button class="close-btn" (click)="errorMessage = null">&times;</button>
      </div>
      <div *ngIf="successMessage" class="alert alert-success">
        <span>{{ successMessage }}</span>
        <button class="close-btn" (click)="successMessage = null">&times;</button>
      </div>

      <div *ngIf="isLoading" class="center-state">
        <div class="spinner"></div>
        <p>Loading assets...</p>
      </div>

      <div *ngIf="!isLoading && !assets.length" class="center-state">
        <p>No assets found.</p>
        <button class="btn btn-secondary" (click)="openForm()">Create First Asset</button>
      </div>

      <div *ngIf="!isLoading && assets.length > 0" class="table-container">
        <table class="data-table">
          <thead>
            <tr>
              <th>Asset Tag</th>
              <th>Name</th>
              <th>Platform</th>
              <th>Location</th>
              <th>Status</th>
              <th>Notes</th>
              <th>Checked Out To</th>
              <th>Checked Out At</th>
              <th class="text-right">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let asset of assets">
              <td><code>{{ asset.assetTag }}</code></td>
              <td class="font-medium">{{ asset.name }}</td>
              <td>{{ asset.platform }}</td>
              <td>{{ asset.location }}</td>
              <td>
                <span class="badge" [ngClass]="badgeMap[asset.status]">
                  {{ asset.status === 'InUse' ? 'In Use' : asset.status }}
                </span>
              </td>
              <td>{{ asset.notes || '—' }}</td>
              <td>{{ asset.checkedOutTo || '—' }}</td>
              <td>{{ asset.checkedOutAt ? (asset.checkedOutAt | date:'mediumDate') + ' ' + (asset.checkedOutAt | date:'shortTime') : '—' }}</td>
              <td class="actions-cell">
                <button class="btn btn-sm btn-ghost" (click)="openForm(asset)">Edit</button>
                <button *ngIf="asset.status === 'Available'" class="btn btn-sm btn-secondary" (click)="openCheckoutModal(asset)">Check Out</button>
                <button *ngIf="asset.status === 'InUse' || asset.checkedOutTo" class="btn btn-sm btn-secondary" (click)="checkIn(asset)">Check In</button>
                <button class="btn btn-sm btn-ghost-danger" (click)="openDeleteModal(asset)">Delete</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <div *ngIf="showForm" class="modal-backdrop">
        <div class="modal-card">
          <h2>{{ activeAsset ? 'Edit Asset' : 'Create Asset' }}</h2>

          <div *ngIf="modalErrorMessage" class="alert alert-error">
            <span>{{ modalErrorMessage }}</span>
            <button class="close-btn" (click)="modalErrorMessage = null">&times;</button>
          </div>

          <form (ngSubmit)="saveAsset()">
            <div class="form-group" *ngIf="!activeAsset">
              <label>Asset Tag *</label>
              <input type="text" class="form-control" [(ngModel)]="form.assetTag" name="assetTag" placeholder="e.g. AST-1001" />
            </div>
            <div class="form-group">
              <label>Name *</label>
              <input type="text" class="form-control" [(ngModel)]="form.name" name="name" placeholder="e.g. NextSeq 550" />
            </div>
            <div class="form-row">
              <div class="form-group">
                <label>Platform *</label>
                <input type="text" class="form-control" [(ngModel)]="form.platform" name="platform" placeholder="e.g. Illumina" />
              </div>
              <div class="form-group">
                <label>Location *</label>
                <input type="text" class="form-control" [(ngModel)]="form.location" name="location" placeholder="e.g. Room 204" />
              </div>
            </div>
            <div class="form-group" *ngIf="activeAsset">
              <label>Status</label>
              <select class="form-control" [(ngModel)]="form.status" name="status">
                <option value="Available">Available</option>
                <option value="InUse">InUse</option>
                <option value="Maintenance">Maintenance</option>
                <option value="Retired">Retired</option>
              </select>
            </div>
            <div class="form-group">
              <label>Notes</label>
              <textarea class="form-control" [(ngModel)]="form.notes" name="notes" rows="2" placeholder="Optional notes..."></textarea>
            </div>
            <div class="modal-actions">
              <button type="button" class="btn btn-secondary" (click)="closeModals()">Cancel</button>
              <button type="submit" class="btn btn-primary" [disabled]="isSubmitting">
                {{ isSubmitting ? 'Saving...' : 'Save Asset' }}
              </button>
            </div>
          </form>
        </div>
      </div>

      <div *ngIf="showCheckout" class="modal-backdrop">
        <div class="modal-card">
          <h2>Check Out Asset</h2>
          <p class="subtitle">Assign <code>{{ activeAsset?.assetTag }}</code> to an operator.</p>

          <div *ngIf="modalErrorMessage" class="alert alert-error">
            <span>{{ modalErrorMessage }}</span>
            <button class="close-btn" (click)="modalErrorMessage = null">&times;</button>
          </div>

          <form (ngSubmit)="submitCheckout()">
            <div class="form-group">
              <label>Assignee Name *</label>
              <input type="text" class="form-control" [(ngModel)]="checkoutAssignee" name="assignee" placeholder="e.g. Dr. Jane Doe" />
            </div>
            <div class="modal-actions">
              <button type="button" class="btn btn-secondary" (click)="closeModals()">Cancel</button>
              <button type="submit" class="btn btn-primary" [disabled]="isSubmitting">
                {{ isSubmitting ? 'Processing...' : 'Confirm Checkout' }}
              </button>
            </div>
          </form>
        </div>
      </div>

      <div *ngIf="showDelete" class="modal-backdrop">
        <div class="modal-card">
          <h2>Delete Asset</h2>
          <p class="subtitle">Are you sure you want to delete <code>{{ activeAsset?.assetTag }}</code>?</p>

          <div *ngIf="modalErrorMessage" class="alert alert-error">
            <span>{{ modalErrorMessage }}</span>
            <button class="close-btn" (click)="modalErrorMessage = null">&times;</button>
          </div>

          <div class="modal-actions">
            <button type="button" class="btn btn-secondary" (click)="closeModals()">Cancel</button>
            <button type="button" class="btn btn-danger" [disabled]="isSubmitting" (click)="deleteAsset()">
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
    .actions-cell { display: flex; gap: 0.35rem; justify-content: flex-end; }

    .badge { padding: 0.15rem 0.5rem; border-radius: 4px; font-size: 0.75rem; font-weight: 500; display: inline-block; border: 1px solid transparent; }
    .badge-available { background: #f0fdf4; color: #166534; border-color: #bbf7d0; }
    .badge-inuse { background: #eff6ff; color: #1e40af; border-color: #bfdbfe; }
    .badge-maintenance { background: #fffbeb; color: #92400e; border-color: #fef3c7; }
    .badge-retired { background: #f3f4f6; color: #4b5563; border-color: #e5e7eb; }

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
export class AssetsPageComponent implements OnInit {
  private assetService = inject(AssetApiService);

  assets: Asset[] = [];
  isLoading = false;
  isSubmitting = false;
  showForm = false;
  showCheckout = false;
  showDelete = false;

  errorMessage: string | null = null;
  modalErrorMessage: string | null = null;
  successMessage: string | null = null;
  activeAsset: Asset | null = null;
  checkoutAssignee = '';

  badgeMap: Record<string, string> = {
    Available: 'badge-available',
    InUse: 'badge-inuse',
    Maintenance: 'badge-maintenance',
    Retired: 'badge-retired'
  };

  form: AssetFormData = this.emptyForm();

  ngOnInit(): void { this.loadAssets(); }

  private emptyForm(): AssetFormData {
    return { assetTag: '', name: '', platform: '', location: '', status: 'Available', notes: '' };
  }

  loadAssets(): void {
    this.isLoading = true;
    this.errorMessage = null;
    this.assetService.list().subscribe({
      next: (data) => { this.assets = data; this.isLoading = false; },
      error: (err) => {
        this.handleError(err, false);
        if (!this.assets.length) {
          this.errorMessage = null;
        }
        this.isLoading = false;
      }
    });
  }

  openForm(asset?: Asset): void {
    this.activeAsset = asset || null;
    this.form = asset ? { ...asset, notes: asset.notes || '' } : this.emptyForm();
    this.modalErrorMessage = null;
    this.showForm = true;
  }

  openCheckoutModal(asset: Asset): void {
    this.activeAsset = asset;
    this.checkoutAssignee = '';
    this.modalErrorMessage = null;
    this.showCheckout = true;
  }

  openDeleteModal(asset: Asset): void {
    this.activeAsset = asset;
    this.modalErrorMessage = null;
    this.showDelete = true;
  }

  closeModals(): void {
    this.showForm = this.showCheckout = this.showDelete = false;
    this.activeAsset = null;
    this.checkoutAssignee = '';
    this.modalErrorMessage = null;
  }

  saveAsset(): void {
    const tag = this.form.assetTag?.trim();
    const name = this.form.name?.trim();
    const platform = this.form.platform?.trim();
    const location = this.form.location?.trim();

    if (!this.activeAsset && !tag || !name || !platform || !location) {
      this.modalErrorMessage = 'Please fill out all required fields (*).';
      return;
    }

    this.isSubmitting = true;
    this.modalErrorMessage = null;

    const payload = {
      ...this.form,
      assetTag: tag,
      name,
      platform,
      location,
      notes: this.form.notes?.trim() || ''
    };

    const request$ = this.activeAsset
      ? this.assetService.update(this.activeAsset.id, payload as UpdateAssetRequest)
      : this.assetService.create(payload as CreateAssetRequest);

    request$.subscribe({
      next: () => {
        this.successMessage = `Asset ${this.activeAsset ? 'updated' : 'created'} successfully.`;
        this.isSubmitting = false;
        this.closeModals();
        this.loadAssets();
      },
      error: (err) => {
        this.handleError(err, true);
        this.isSubmitting = false;
      }
    });
  }

  submitCheckout(): void {
    const assignee = this.checkoutAssignee.trim();

    if (!assignee) {
      this.modalErrorMessage = 'Please provide an assignee name.';
      return;
    }

    if (!this.activeAsset) return;

    this.isSubmitting = true;
    this.modalErrorMessage = null;

    this.assetService.checkOut(this.activeAsset.id, { assignee }).subscribe({
      next: () => {
        this.successMessage = `Asset checked out to ${assignee}.`;
        this.isSubmitting = false;
        this.closeModals();
        this.loadAssets();
      },
      error: (err) => {
        this.handleError(err, true);
        this.isSubmitting = false;
      }
    });
  }

  checkIn(asset: Asset): void {
    this.isLoading = true;
    this.errorMessage = null;
    this.assetService.checkIn(asset.id).subscribe({
      next: () => {
        this.successMessage = `Asset ${asset.assetTag} checked in successfully.`;
        this.loadAssets();
      },
      error: (err) => { this.handleError(err, false); this.isLoading = false; }
    });
  }

  deleteAsset(): void {
    if (!this.activeAsset) return;
    this.isSubmitting = true;
    this.modalErrorMessage = null;

    this.assetService.remove(this.activeAsset.id).subscribe({
      next: () => {
        this.successMessage = 'Asset deleted successfully.';
        this.isSubmitting = false;
        this.closeModals();
        this.loadAssets();
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

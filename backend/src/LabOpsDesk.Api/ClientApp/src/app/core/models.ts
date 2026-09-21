export type AssetStatus = 'Available' | 'InUse' | 'Maintenance' | 'Retired';

export interface Asset {
  id: string;
  assetTag: string;
  name: string;
  platform: string;
  location: string;
  status: AssetStatus;
  checkedOutTo: string | null;
  checkedOutAt: string | null;
  notes: string | null;
}

export interface CreateAssetRequest {
  assetTag: string;
  name: string;
  platform: string;
  location: string;
  notes?: string | null;
}

export interface UpdateAssetRequest {
  name: string;
  platform: string;
  location: string;
  status: AssetStatus;
  notes?: string | null;
}

export interface CheckOutAssetRequest {
  assignee: string;
}

export interface Part {
  id: string;
  sku: string;
  name: string;
  category: string;
  unitOfMeasure: string;
  quantityOnHand: number;
  reorderThreshold: number;
  location: string;
  isLowStock: boolean;
}

export interface CreatePartRequest {
  sku: string;
  name: string;
  category: string;
  unitOfMeasure: string;
  quantityOnHand: number;
  reorderThreshold: number;
  location: string;
}

export interface UpdatePartRequest {
  name: string;
  category: string;
  unitOfMeasure: string;
  quantityOnHand: number;
  reorderThreshold: number;
  location: string;
}

export interface AdjustPartQuantityRequest {
  delta: number;
}

export interface ApiError {
  code: string;
  message: string;
  errors?: Record<string, string[]>;
}

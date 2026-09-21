import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home.component';
import { AssetsPageComponent } from './features/assets/assets-page.component';
import { PartsPageComponent } from './features/parts/parts-page.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'assets', component: AssetsPageComponent },
  { path: 'parts', component: PartsPageComponent },
  { path: '**', redirectTo: '' },
];

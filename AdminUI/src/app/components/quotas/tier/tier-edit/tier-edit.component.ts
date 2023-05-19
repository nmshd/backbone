import { Component } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { Tier, TierService } from 'src/app/services/tier-service/tier.service';
import { HttpResponseEnvelope } from 'src/app/utils/http-response-envelope';

@Component({
  selector: 'app-tier-edit',
  templateUrl: './tier-edit.component.html',
  styleUrls: ['./tier-edit.component.css'],
})
export class TierEditComponent {
  headerEdit: string;
  headerCreate: string;

  tierId?: string;
  editMode: boolean;

  tier: Tier;

  loading: boolean;
  disabled: boolean;

  constructor(
    private route: ActivatedRoute,
    private snackBar: MatSnackBar,
    private tierService: TierService
  ) {
    this.headerEdit = 'Edit Tier';
    this.headerCreate = 'Create Tier';
    this.editMode = false;
    this.loading = true;
    this.disabled = false;
    this.tier = {};
  }

  ngOnInit() {
    this.route.params.subscribe((params) => {
      if (params['id']) {
        this.tierId = params['id'];
        this.editMode = true;
      }
    });

    if (this.editMode) {
      this.getTier();
    } else {
      this.initTier();
    }
  }

  initTier() {
    this.tier = {
      name: '',
    } as Tier;

    this.loading = false;
  }

  getTier() {
    this.loading = true;
    this.tierService.getTierById(this.tierId!).subscribe({
      next: (data: HttpResponseEnvelope<Tier>) => {
        if (data && data.result) {
          this.tier = data.result;
        }
      },
      complete: () => (this.loading = false),
      error: (err: any) => {
        this.loading = false;
        this.snackBar.open(err.message, 'Dismiss');
      },
    });
  }

  createTier() {
    console.log('create');
    this.loading = true;
    this.tierService.createTier(this.tier).subscribe({
      next: (data: HttpResponseEnvelope<Tier>) => {
        if (data && data.result) {
          this.tier = data.result;
        }
        this.snackBar.open('Successfully added tier.', 'Dismiss');
        this.tierId = data.result.id;
        this.editMode = true;
      },
      complete: () => (this.loading = false),
      error: (err: any) => {
        this.loading = false;
        this.snackBar.open(err.message, 'Dismiss');
        this.disabled = true;
      },
    });
  }

  updateTier() {
    this.loading = true;
    this.tierService.updateTier(this.tier).subscribe({
      next: (data: HttpResponseEnvelope<Tier>) => {
        if (data && data.result) {
          this.tier = data.result;
          this.snackBar.open('Successfully updated tier.', 'Dismiss');
        }
      },
      complete: () => (this.loading = false),
      error: (err: any) => {
        this.loading = false;
        this.snackBar.open(err.message, 'Dismiss');
      },
    });
  }

  validateTier(): boolean {
    if (this.tier && this.tier.name && this.tier.name.length > 0) {
      return true;
    }
    return false;
  }
}

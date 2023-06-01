import { Component } from '@angular/core';

@Component({
    selector: 'app-tier-edit-quotas-dialog',
    templateUrl: './tier-edit-quotas-dialog.component.html',
    styleUrls: ['./tier-edit-quotas-dialog.component.css'],
})
export class TierEditQuotasDialogComponent {
    header: string;

    constructor() {
        this.header = 'Assign Quota';
    }
}

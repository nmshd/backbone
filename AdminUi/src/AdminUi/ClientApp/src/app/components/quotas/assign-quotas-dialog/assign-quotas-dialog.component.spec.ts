import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AssignQuotasDialogComponent } from './assign-quotas-dialog.component';

describe('AssignQuotasDialogComponent', () => {
    let component: AssignQuotasDialogComponent;
    let fixture: ComponentFixture<AssignQuotasDialogComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [AssignQuotasDialogComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(AssignQuotasDialogComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});

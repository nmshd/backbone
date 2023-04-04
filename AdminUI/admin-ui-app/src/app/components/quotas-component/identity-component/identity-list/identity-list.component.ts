import { Component, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { LazyLoadEvent, MessageService } from 'primeng/api';
import { Table } from 'primeng/table';
import {
  Identity,
  IdentityDTO,
  IdentityService,
} from 'src/app/services/identity-service/identity.service';

@Component({
  selector: 'app-identity-list-component',
  templateUrl: './identity-list.component.html',
  styleUrls: ['./identity-list.component.css'],
  providers: [MessageService],
})
export class IdentityListComponent {
  @ViewChild('identitiesTable') dt!: Table;
  header: string;

  identities: Identity[];

  totalRecords: number;

  loading: boolean;

  constructor(
    private router: Router,
    private identityService: IdentityService,
    private messageService: MessageService
  ) {
    this.header = '';

    this.identities = [];

    this.totalRecords = 0;
    this.loading = true;
  }

  ngOnInit() {
    this.header = 'Identities';
  }

  loadIdentities(event: LazyLoadEvent) {
    this.loading = true;

    setTimeout(() => {
      this.identityService
        .getIdentities(event)
        .subscribe({
          next: (data: IdentityDTO) => {
            if (data) {
              this.identities = data.identities;
              this.totalRecords = data.totalRecords;
            }
          },
          error: (err: any) =>
            this.messageService.add({
              severity: 'error',
              summary: err.status,
              detail: err.message,
              sticky: true,
            }),
        })
        .add(() => (this.loading = false));
    }, 1000);
  }

  loadIdentitiesMock(event: LazyLoadEvent) {
    this.loading = true;

    setTimeout(() => {
      this.mockFake();
      this.loading = false;
    }, 1000);
  }

  clear() {
    this.dt.clear();
  }

  applyFilter($event: any, matchMode: string) {
    let value = ($event.target as HTMLInputElement)?.value;
    this.dt.filterGlobal(value, matchMode);
  }

  editIdentity(identity: Identity) {
    this.router.navigate([`/identities`, identity.address]);
  }

  mockFake() {
    let identities: Identity[] = [];

    for (let i = 0; i < 10; i++) {
      identities.push({
        address: Math.random().toString(36).slice(2),
        clientId: Math.random().toString(36).slice(2),
        publicKey: Math.random().toString(36).slice(2),
        createdAt: new Date(),
        identityVersion: Math.random().toString(36).slice(2),
      });
    }
    this.identities = identities;
    this.totalRecords = 10;
  }
}

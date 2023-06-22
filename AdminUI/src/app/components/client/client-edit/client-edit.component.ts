import { Component } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import { Client } from 'src/app/services/client-service/client-service';
import { ClientServiceService } from 'src/app/services/client-service/client-service';
import { HttpResponseEnvelope } from 'src/app/utils/http-response-envelope';

@Component({
  selector: 'app-client-edit',
  templateUrl: './client-edit.component.html',
  styleUrls: ['./client-edit.component.css']
})
export class ClientEditComponent {
    headerCreate: string;

    clientId?: string;
    editMode: boolean;

    client: Client;

    loading: boolean;
    disabled: boolean;

    constructor(
        private route: ActivatedRoute,
        private snackBar: MatSnackBar,
        private clientService: ClientServiceService
    ) {
        this.headerCreate = 'Create Client';
        this.editMode = false;
        this.loading = true;
        this.disabled = false;
        this.client = 
        {
          clientId: '',
          displayName: '',
          clientSecret:'',
        } as Client;
    }

    ngOnInit() {
        this.route.params.subscribe((params) => {
            if (params['id']) {
                this.clientId = params['id'];
            }
        });
        this.initClient();
    }

    initClient() {
        this.client = {
            clientId: '',
            displayName: '',
            clientSecret:'',
        } as Client;

        this.loading = false;
    }

    createClient() {
      this.loading = true;
      this.clientService.createClient(this.client).subscribe({
          next: (data: HttpResponseEnvelope<Client>) => {
              if (data && data.result) {
                  this.client = data.result;
              }
              this.snackBar.open('Successfully added client.', 'Dismiss');
          },
          complete: () => (this.loading = false),
          error: (err: any) => {
              this.loading = false;
              this.snackBar.open(err.message, 'Dismiss');
              this.disabled = true;
          },
      });
  }

    validateClient(): boolean {
        if (this.client && this.client.clientId && this.client.clientId.length > 0) {
            return true;
        }
        return false;
    }
    
}

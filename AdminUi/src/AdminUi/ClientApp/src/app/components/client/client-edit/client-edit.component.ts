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
    showPassword: boolean;
    headerCreate: string;
    headerDescription: string;
    clientId?: string;
    editMode: boolean;
    client: Client;
    loading: boolean;
    disabled: boolean;
    displayClientSecretWarning: boolean;

    constructor(
        private route: ActivatedRoute,
        private snackBar: MatSnackBar,
        private clientService: ClientServiceService
    ) {
        this.headerCreate = 'Create Client';
        this.headerDescription = 'Please fill the form below to create your Client';
        this.editMode = false;
        this.loading = true;
        this.disabled = false;
        this.displayClientSecretWarning = false;
        this.showPassword = false;
        this.client = {
            clientId: '',
            displayName: '',
            clientSecret: '',
        } as Client;
    }

    ngOnInit(): void {
        this.route.params.subscribe((params) => {
            if (params['id']) {
                this.clientId = params['id'];
            }
        });
        this.initClient();
    }

    initClient(): void {
        this.client = {
            clientId: '',
            displayName: '',
            clientSecret: '',
        } as Client;

        this.loading = false;
    }

    createClient(): void {
        this.loading = true;
        this.clientService.createClient(this.client).subscribe({
            next: (data: HttpResponseEnvelope<Client>) => {
                if (data && data.result) {
                    this.client = data.result;
                }
                this.displayClientSecretWarning = true;
                this.snackBar.open('Successfully added client.', 'Dismiss', {
                    duration: 4000,
                    verticalPosition: 'top',
                    horizontalPosition: 'center'
                });
            },
            complete: () => (this.loading = false),
            error: (err: any) => {
                this.loading = false;
                let errorMessage = (err.error && err.error.error && err.error.error.message) ? err.error.error.message : err.message;
                this.snackBar.open(errorMessage, 'Dismiss', {
                    verticalPosition: 'top',
                    horizontalPosition: 'center'
                });
                this.disabled = true;
            },
        });
    }

    validateClient(): boolean {
        if (this.client && this.client.displayName && this.client.displayName.length > 0) {
            return true;
        }
        return false;
    }

    togglePasswordVisibility(): void {
        this.showPassword = !this.showPassword;
    }
}

import { Component } from "@angular/core";

@Component({
    selector: "app-download-message-dialog",
    templateUrl: "./download-message-dialog.component.html",
    styleUrl: "./download-message-dialog.component.css"
})
export class DownloadMessageDialogComponent {
    public header: string;
    public symetricKey: string;
    public message: string;

    public loading: boolean;
    public showSymetricKey: boolean;
    public preview: boolean;

    public constructor() {
        this.header = "Download message";
        this.loading = false;
        this.showSymetricKey = false;
        this.preview = false;
        this.symetricKey = "";
        this.message = "This is a message that will be displayed when a correct symetric key is entered, and this message can be downloaded.";
    }

    public isValid(): boolean {
        // return this.metric !== undefined && this.period !== undefined && this.max !== null;
        return true;
    }

    public toggleSymetricKeyVisibility(): void {
        this.showSymetricKey = !this.showSymetricKey;
    }

    public previewMessage(): void {
        if (this.symetricKey === "test") {
            this.preview = true;
        }
    }

    public downloadMessage(): void {
            const message = btoa(this.message)
            const byteCharacters = atob(message);
            const byteArrays = [];
        
            for (let offset = 0; offset < byteCharacters.length; offset += 512) {
              const slice = byteCharacters.slice(offset, offset + 512);
              const byteNumbers = new Array(slice.length);
              for (let i = 0; i < slice.length; i++) {
                byteNumbers[i] = slice.charCodeAt(i);
              }
              const byteArray = new Uint8Array(byteNumbers);
              byteArrays.push(byteArray);
            }
        
            const blob = new Blob(byteArrays, { type: "application/pdf" });
            const url = window.URL.createObjectURL(blob);
            const link = document.createElement("a");
            link.href = url;
            link.download = "file.pdf";
            link.click();
            window.URL.revokeObjectURL(url);
    }
}

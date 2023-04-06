import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root',
})
export class SidebarService {
    private extended: boolean;
    private mobileSize: number;

    constructor() {
        this.extended = false;
        this.mobileSize = 920;
    }

    public isOpen() {
        return this.extended;
    }

    public open() {
        this.extended = true;
    }

    public close() {
        this.extended = false;
    }

    public toggle() {
        this.extended = !this.extended;
    }

    public getMobileSize() {
        return this.mobileSize;
    }
}

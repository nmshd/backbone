import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class SidebarService {
  private extended: boolean;
  private mobileSize: number;

  constructor() {
    this.mobileSize = 920;
    this.extended = !this.isMobile();
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

  public isMobile(): boolean {
    return window.screen.width <= this.getMobileSize();
  }

  public getMobileSize() {
    return this.mobileSize;
  }
}

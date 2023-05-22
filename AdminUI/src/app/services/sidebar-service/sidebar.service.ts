import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class SidebarService {
  private extended: boolean;
  private mobile: boolean;

  constructor(breakpointObserver: BreakpointObserver) {
    this.mobile = false;

    breakpointObserver
      .observe([Breakpoints.XSmall, Breakpoints.Small])
      .subscribe((result) => {
        if (result.matches) {
          this.mobile = true;
        } else {
          this.mobile = false;
        }
      });

    this.extended = !this.isMobile();
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

  public isOpen(): boolean {
    return this.extended;
  }

  public isMobile(): boolean {
    return this.mobile;
  }
}

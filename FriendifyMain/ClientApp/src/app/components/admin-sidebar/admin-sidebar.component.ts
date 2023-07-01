import { Component } from '@angular/core';

@Component({
  selector: 'app-admin-sidebar',
  templateUrl: './admin-sidebar.component.html',
  styleUrls: ['./admin-sidebar.component.scss']
})
export class AdminSidebarComponent {
  model: any[] = [];
  visibleSidebar: boolean = false;
  ngOnInit(): void {
    this.initMenuItems();
  }

  // Define a method to initialize the menu items
  initMenuItems(): void {
    // Assign an array of menu items to the items property
    this.model = [
      {
        label: 'Admin',
        items: [
          { label: 'Dashboard', icon: 'pi pi-fw pi-home', routerLink: ['/admin'] },
          { label: 'Accounts', icon: 'pi pi-fw pi-users', routerLink: ['/admin/users'] }


        ]
      },

    ];

  }

  // Define a method to open the sidebar
  openSidebar(): void {
    // Set the visibleSidebar property to true
    this.visibleSidebar = true;
  }

  // Define a method to close the sidebar
  closeSidebar(): void {
    // Set the visibleSidebar property to false
    this.visibleSidebar = false;
  }
}

import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Contact } from '../../models/contact.model';
import { ContactService } from '../../services/contact.service';
import { ContactFilterComponent } from '../contact-filter/contact-filter.component';
import { ContactListComponent } from '../contact-list/contact-list.component';
import { ContactDetailsComponent } from '../contact-details/contact-details.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    ContactFilterComponent,
    ContactListComponent,
    ContactDetailsComponent
  ],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  title = 'Contact Dashboard';
  
  selectedContact = signal<Contact | null>(null);
  currentFilter = signal<string>('All');
 
  contacts = this.contactService.contacts;
  
  filteredContacts = computed(() => {
    const contacts = this.contacts();
    const filter = this.currentFilter();
    
    if (filter === 'All') {
      return contacts;
    } else {
      return contacts.filter(contact => contact.groups.includes(filter));
    }
  });

  constructor(private contactService: ContactService) {}

  ngOnInit(): void { 
  }

  onFilterChange(filter: string): void {
    this.currentFilter.set(filter);
  }

  onContactSelect(contact: Contact): void {
    this.selectedContact.set(contact);
  }

  onContactUpdate(updatedContact: Contact): void {
    this.contactService.updateContact(updatedContact);
    this.selectedContact.set(updatedContact);
  }
}
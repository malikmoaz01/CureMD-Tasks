import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Contact } from '../../models/contact.model';
import { ContactService } from '../../services/contact.service';
import { ContactFilterComponent } from '../contact-filter/contact-filter.component';
import { ContactListComponent } from '../contact-list/contact-list.component';
import { ContactDetailsComponent } from '../contact-details/contact-details.component';
import { LettersSpacesDirective } from '../../directives/letters-spaces.directive';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    ContactFilterComponent,
    ContactListComponent,
    ContactDetailsComponent,
    LettersSpacesDirective
  ],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  title = 'Contact Dashboard';
  selectedContact = signal<Contact | null>(null);
  currentFilter = signal<string>('All');
  searchTerm = signal<string>('');
  contacts = this.contactService.contacts;
  
  filteredContacts = computed(() => {
    let contacts = this.contacts();
    const filter = this.currentFilter();
    const search = this.searchTerm().trim().toLowerCase();

    if (filter !== 'All') {
      contacts = contacts.filter(contact => contact.groups.includes(filter));
    }
    if (search) {
      contacts = contacts.filter(contact =>
        contact.name.toLowerCase().includes(search)   
      );
    }
    return contacts;
  });

  constructor(private contactService: ContactService) {}

  ngOnInit(): void {}

  onSearchInput(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.searchTerm.set(target.value);
  }

  clearSearch(): void {
    this.searchTerm.set('');
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
import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Contact } from '../../models/contact.model';

@Component({
  selector: 'app-contact-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './contact-list.component.html',
  styleUrls: ['./contact-list.component.scss']
})
export class ContactListComponent {
  contacts = input<Contact[]>([]);
  selectedContact = input<Contact | null>(null);
  contactSelect = output<Contact>();

  onContactClick(contact: Contact): void {
    this.contactSelect.emit(contact);
  }
}

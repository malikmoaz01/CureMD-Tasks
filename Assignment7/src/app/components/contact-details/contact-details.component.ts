import { Component, input, output, OnInit, OnDestroy, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Contact } from '../../models/contact.model';

@Component({
  selector: 'app-contact-details',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './contact-details.component.html',
  styleUrls: ['./contact-details.component.scss']
})
export class ContactDetailsComponent implements OnInit, OnDestroy {
  contact = input<Contact | null>(null);
  contactUpdate = output<Contact>();

  availableGroups = ['Favourites', 'Family', 'Friends', 'Classmates'];

  constructor() { 
    effect(() => {
      const currentContact = this.contact();
      console.log('ContactDetailsComponent input changed', currentContact);
    });
  }

  ngOnInit(): void {
    console.log('ContactDetailsComponent initialized');
  }

  ngOnDestroy(): void {
    console.log('ContactDetailsComponent destroyed');
  }

  toggleGroup(group: string): void {
    const currentContact = this.contact();
    if (!currentContact) return;

    const updatedContact = { ...currentContact };
    const groupIndex = updatedContact.groups.indexOf(group);

    if (groupIndex > -1) {
      updatedContact.groups = updatedContact.groups.filter(g => g !== group);
    } else {
      updatedContact.groups = [...updatedContact.groups, group];
    }

    this.contactUpdate.emit(updatedContact);
  }

  isInGroup(group: string): boolean {
    return this.contact()?.groups.includes(group) || false;
  }
}
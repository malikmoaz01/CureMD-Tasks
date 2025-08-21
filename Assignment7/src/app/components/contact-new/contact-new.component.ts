import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common'; 
import { ContactService } from '../../services/contact.service';

@Component({
  selector: 'app-contact-new',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './contact-new.component.html',
  styleUrls: ['./contact-new.component.scss']  
})
export class ContactNewComponent {

  constructor(private contactService: ContactService) {} 

  contactForm = new FormGroup({
    name: new FormControl(''),
    phone: new FormControl(''),
    email: new FormControl(''),
    gender: new FormControl(''),
    address: new FormControl(''),
    groups: new FormGroup({
      favourites: new FormControl(false),
      family: new FormControl(false),
      friends: new FormControl(false),
      classmates: new FormControl(false)
    })
  });

  addcontact() {
    const formValue = this.contactForm.value;
    const groups = formValue.groups as { [key: string]: boolean | null } ?? {};

    const groupMapping: { [key: string]: string } = {
      favourites: 'Favourites',
      family: 'Family', 
      friends: 'Friends',
      classmates: 'Classmates'
    };

    const selectedGroups = Object.keys(groups)
      .filter(group => groups[group])
      .map(group => groupMapping[group]);

    const newContact = {
      name: formValue.name || '',
      phone: formValue.phone || '',
      email: formValue.email || '',
      gender: formValue.gender || '',
      address: formValue.address || '',
      groups: selectedGroups
    };

    this.contactService.addContact(newContact);
    this.contactForm.reset();
  }
}
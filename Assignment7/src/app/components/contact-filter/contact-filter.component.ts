import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-contact-filter',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './contact-filter.component.html',
  styleUrls: ['./contact-filter.component.scss']
})
export class ContactFilterComponent {
  currentFilter = input<string>('All');
  filterChange = output<string>();

  filters = ['All', 'Favourites', 'Family', 'Friends', 'Classmates'];

  onFilterClick(filter: string): void {
    this.filterChange.emit(filter);
  }
}
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'searchHighlight',
  standalone: true
})
export class SearchHighlightPipe implements PipeTransform {
  transform(text: string, searchTerm: string): string {
    if (!searchTerm || !text) {
      return text;
    }

    const escaped = searchTerm.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'); 
    const regex = new RegExp(escaped, 'gi');
    return text.replace(regex, '<mark class="highlight">$&</mark>');
  }
}

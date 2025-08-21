import { Directive, HostListener } from '@angular/core';

@Directive({
  selector: '[appLettersSpaces]',
  standalone: true
})
export class LettersSpacesDirective {

  @HostListener('keypress', ['$event'])
  onKeyPress(event: KeyboardEvent): boolean {
    const charCode = event.which || event.keyCode;
    const charStr = String.fromCharCode(charCode);
     
    if (/[a-zA-Z\s]/.test(charStr) || charCode === 8 || charCode === 9 || charCode === 46) {
      return true;
    }
    
    event.preventDefault();
    return false;
  }

  @HostListener('input', ['$event'])
  onInput(event: Event): void {
    const target = event.target as HTMLInputElement; 
    target.value = target.value.replace(/[^a-zA-Z\s]/g, '');
  }
}
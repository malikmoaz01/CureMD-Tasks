import { Directive , HostListener } from '@angular/core';

@Directive({
  selector: '[appLettersSpaces]',
  standalone: true
})
export class LettersSpacesDirective {
  private regex = /^[a-zA-Z\s]*$/
  constructor() { }

  @HostListener('input' , ['$event']) onInput(event : any)
  {
    const input = event.target;

    if(!this.regex.test(input.value))
    {
      input.value = input.value.replace(/[^a-zA-Z\s]/g, '');
      event.preventDefault();
    }

  }

}

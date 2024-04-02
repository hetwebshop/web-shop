import { Directive, HostListener } from '@angular/core'
import { NavigationService } from 'src/app/services/navigation.service'

@Directive({
  selector: '[backButton]',
})
export class BackButtonDirective {
  constructor(private navigation: NavigationService) {}

  @HostListener('click')
  onClick(): void {
    console.log("click");
    this.navigation.back()
  }
}
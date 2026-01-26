import { Component, signal } from '@angular/core';
import { RouterOutlet, RouterLink, RouterModule } from '@angular/router';
import { BusketComponent } from './components/busket/busket.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterModule, BusketComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  //title = 'clientt';
  protected readonly title = signal('client');
}

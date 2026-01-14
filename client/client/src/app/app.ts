import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Category } from './components/category/category';

@Component({
  selector: 'app-root',
  imports: [Category],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('client');
}

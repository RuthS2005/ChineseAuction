import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { Auth } from '../services/auth';

@Component({
  selector: 'app-home-page',
  standalone:true,
  imports: [RouterModule],
  templateUrl: './home-page.html',
  styleUrl: './home-page.scss',
})
export class HomePage {
constructor(public authService: Auth) {}
}

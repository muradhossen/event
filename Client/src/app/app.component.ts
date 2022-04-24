import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { User } from './_models/user';
import { AccountService } from './_services/account.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Dating Application';
  users: any;
  constructor(private http: HttpClient, private accountService: AccountService) {

  }

  ngOnInit(): void {
    this.getUsers();
    this.setCurrentUser();
  }

  setCurrentUser() {
    const user: User = JSON.parse(localStorage.getItem('user'));
    this.accountService.serCurrentUser(user);
  }

  getUsers() {
    this.http.get('https://localhost:44348/api/Users').subscribe(response => {
      console.log(response);
      this.users = response;
    }, error => {
      console.log(error);
    });
  }

}

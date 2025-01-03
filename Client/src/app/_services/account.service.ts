import { HttpClient } from '@angular/common/http';
import { ChangeDetectorRef, Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { PresenceService } from './presence.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient, private presenceService : PresenceService) { }

  login(model: any, cd: ChangeDetectorRef) {
    return this.http.post(this.baseUrl + 'Account/login', model).pipe(
      map((response: User) => {
        const user = response;
        if (user) {
          this.serCurrentUser(user);
          this.presenceService.createHubConnection(user, cd);
          return user;
        }
      })
    );
  }
  serCurrentUser(user: User) {

    user.roles = [];

    const roles = this.getDecodedToken(user.token).role;

    Array.isArray(roles) ? user.roles = roles :user.roles.push(roles);

    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }


  logout() {
    localStorage.removeItem('user');
    this.presenceService.stopHubConnection();
    this.currentUserSource.next(null);
  }

  register(user: any, cd: ChangeDetectorRef) {
    return this.http.post(this.baseUrl + 'Account/register', user).pipe(
      map((user: User) => {
        if (user) {
          this.serCurrentUser(user);
          this.presenceService.createHubConnection(user,cd);
        }
      })

    );
  }

  getDecodedToken(token : string){
    return JSON.parse(atob(token.split('.')[1]));
  }

}

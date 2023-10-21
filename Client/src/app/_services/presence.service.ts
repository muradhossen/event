import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {

  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;

  private onlineUserSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUserSource.asObservable();

  constructor(private toster: ToastrService, private router: Router) {
  }


  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start()
      .catch(error => console.log(error));

    this.hubConnection.on("UserIsOnline", username => {  
      debugger  
      console.log(username + " user is connected");

      this.onlineUsers$.pipe(take(1)).subscribe(usernames => {
        this.onlineUserSource.next([...usernames, username]);
        console.log("Previous online users ", usernames);

        this.onlineUsers$.pipe(take(1)).subscribe(newUsernames => {
          console.log("New usernames ", newUsernames);
        })
      })
    });

    this.hubConnection.on("UserIsOffline", username => {       
      console.warn(username + " user is disconnected!");

      this.onlineUsers$.pipe(take(1)).subscribe(usernames => {
        console.log("Previous online users-", usernames );
        this.onlineUserSource.next([...usernames.filter(x => x !== username)]);

        this.onlineUsers$.pipe(take(1)).subscribe(newOnlineUsernames => {
          console.log("New online users-" , newOnlineUsernames);
        })
      })
    });

    this.hubConnection.on("GetOnlineUsers", (usernames: string[]) => {
       this.onlineUserSource.next(usernames);
    });

    this.hubConnection.on("NewMessageRecived", ({ username, knownAs }) => {
 

      console.log("this is toster -> " , this.toster)

      

      this.toster.info(knownAs + " send a new message!")
        .onTap
        .pipe(take(1))
        .subscribe(() => {
          this.router.navigateByUrl("members/" + username + "?tab=3");
        })
    })

  }

  stopHubConnection() {
    this.hubConnection.stop().catch(error => console.error(error));
  }


}

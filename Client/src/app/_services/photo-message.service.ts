import { Injectable } from '@angular/core';
import { Message } from '../_models/message';
import { take } from 'rxjs/operators';
import { Group } from '../_models/group';
import { HttpClient, HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { User } from '../_models/user';
import { BehaviorSubject } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PhotoMessageService {

  baseUrl = environment.apiUrl;
  hubUrl = environment.hubUrl;

  hubConnection: HubConnection;

  private messageThreadSource = new BehaviorSubject<Message[]>([]);
  public messageThread$ = this.messageThreadSource.asObservable();

  constructor(private http: HttpClient) { }

  CreateHubConnection(user: User, otherUsername: string) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + "message" + "?user=" + otherUsername, {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch(error => console.log(error));

    this.hubConnection.on("ReciveMessageThread", (messages: Message[]) => {
      this.messageThreadSource.next([...messages]);
    });

    this.hubConnection.on("NewMessage", message => {
      this.messageThread$.pipe(take(1)).subscribe(messages => {
        this.messageThreadSource.next([...messages, message]);
      })
    });

    this.hubConnection.on("UpdatedGroup", (group : Group) => {
      if (group.connections.some(c => c.username == otherUsername)) {
        this.messageThread$.pipe(take(1)).subscribe(messages => {
          messages.forEach(message => {
            if(!message.dateRead){
              message.dateRead = new Date(Date.now());
            }

            this.messageThreadSource.next([...messages]);
          });
        });
      }
    });
  }

  stopHubConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop();

    }
  } 
 
  async sendMessage(username: string, content: string) { 

    return this.hubConnection.invoke("SendMessage", { recipientUsername: username, content })
    .catch(error => console.log(error));
  }
 

}

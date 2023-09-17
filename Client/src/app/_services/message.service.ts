import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { getPaginatedResult, getPaginationHeader } from './paginationHelper';
import { Message } from '../_models/message';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  baseUrl = environment.apiUrl;

constructor(private http:HttpClient) { }


getMessges(pageNumber, pageSize, container){

  let params = getPaginationHeader(pageNumber, pageSize);
  params = params.append("Container", container);
  return getPaginatedResult<Message[]>(this.baseUrl + "message" , params, this.http);
}

getMessageThread(username : string){
  return this.http.get<Message[]>(this.baseUrl + 'Message/thread/' + username);
}

}

import { Component, OnInit } from '@angular/core';
import { PresenceService } from '../_services/presence.service';


@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  constructor(public  presenceService :PresenceService) { }

  
  ngOnInit() {
 
  }

}

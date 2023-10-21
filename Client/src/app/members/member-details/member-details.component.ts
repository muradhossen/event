import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { Message } from 'src/app/_models/message';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';
import { MessageService } from 'src/app/_services/message.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-member-details',
  templateUrl: './member-details.component.html',
  styleUrls: ['./member-details.component.css']
})
export class MemberDetailsComponent implements OnInit, OnDestroy {
  @ViewChild("memberTabs", { static: true }) memberTabs: TabsetComponent;
  activeTab: TabDirective;

  members: Member[];
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  member: Member;
  messages: Message[] = [];
  user : User;

  constructor(public presenceService: PresenceService
    , private activeRoute: ActivatedRoute
    , private messageService: MessageService
    , private accountService : AccountService
    , private router : Router) { 
      this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
      this.router.routeReuseStrategy.shouldReuseRoute = () => false;
    }
  ngOnInit(): void {


    this.activeRoute.data.subscribe(data => {
      this.member = data.member;
    })

    this.activeRoute.queryParams.subscribe(params => {
      params.tab ? this.selectTab(params.tab) : this.selectTab(0);
    });



    this.galleryOptions = [
      {
        width: '600px',
        height: '400px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false,
        imageBullets: true
      }]


    this.galleryImages = this.getImages();
  }

  getImages(): NgxGalleryImage[] {
    const imageUrl = [];
    for (const photo of this.member.photos) {
      imageUrl.push({
        small: photo?.url,
        medium: photo?.url,
        large: photo?.url
      });
    }
    return imageUrl;
  } 

  loadMessages() {
    this.messageService.getMessageThread(this.member.username).subscribe(messages => {
      this.messages = messages;
    });
  }

  selectTab(tabId: number) {
    debugger
    this.memberTabs.tabs[tabId].active = true;
  }

  onTabActivated(tab: TabDirective) {
    this.activeTab = tab;

    if (this.activeTab.heading == "Messages" && this.messages.length == 0) {
      this.messageService.CreateHubConnection(this.user, this.member.username);
    }
    else{
      this.messageService.stopHubConnection();
    }
  }

  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }
}

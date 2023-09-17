import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-details',
  templateUrl: './member-details.component.html',
  styleUrls: ['./member-details.component.css']
})
export class MemberDetailsComponent implements OnInit {
  members: Member[];
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  member: Member;

  constructor(private memberService: MembersService, private activeRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.loadMember();

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

  loadMember() {
    
    this.memberService.getMember(this.activeRoute.snapshot.paramMap.get('username')).subscribe(user => {
      console.log('user', user);
      this.member = user;
      console.log('member', this.member);
      this.galleryImages = this.getImages();
    })
  }

}

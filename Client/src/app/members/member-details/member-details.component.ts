import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-details',
  templateUrl: './member-details.component.html',
  styleUrls: ['./member-details.component.css']
})
export class MemberDetailsComponent implements OnInit {

  constructor(private memberService: MembersService, private activeRoute: ActivatedRoute) { }
  member: Member;
  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    this.memberService.getMember(this.activeRoute.snapshot.paramMap.get('username')).subscribe(user => {
      console.log('user', user);
      this.member = user;
      console.log('member', this.member);

    })
  }

}

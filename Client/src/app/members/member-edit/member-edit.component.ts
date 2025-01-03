import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm: NgForm;
  member: Member;
  user: User;
  username: string;
  @HostListener('window:beforeunload', ['$event']) unloadNotification($event) {
    if (this.editForm.dirty) {
      $event.returnValue = true;
    }
  }

  constructor(private memberService: MembersService, private accountServie: AccountService, private toasterService: ToastrService) {
    this.accountServie.currentUser$.pipe(take(1)).subscribe(user => {
      if (user) {
        this.user = user;
        this.username = user.userName;
      }
    })
  }

  ngOnInit(): void {
    if (this.user) this.loadMember();
  }

  loadMember() {
    this.memberService.getMember(this.user.userName).subscribe(res => {
      this.member = res;
    })
  }

  updateMember() {
    this.memberService.updateMember(this.member).subscribe(() => {
      this.toasterService.success('Member update successfully.');
      this.editForm.reset(this.member);
    })

  }

}

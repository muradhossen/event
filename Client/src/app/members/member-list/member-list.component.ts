import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs/internal/Observable';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { UserParams } from 'src/app/_models/userParams';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  members : Member[];
  pagination : Pagination;
  user : User;
  userParams : UserParams;
 genderList = [{value : 'female', display : 'Females'}, {value : 'male' , display : 'Male'}];



  constructor(private memberService: MembersService, accountService : AccountService) {
    accountService.currentUser$.pipe(take(1)).subscribe(
      (res : User)=>{
        this.user = res;
        this.userParams = new UserParams(res);
      });
   }

  ngOnInit(): void {
    this.loadMembers()
  }

  loadMembers() {
    this.memberService.getMembers(this.userParams).subscribe((res) => {
      this.members = res.result;
      this.pagination = res.pagination;
    });
  }

  resetFilter(){
    this.userParams = new UserParams(this.user);
    this.loadMembers();
  }

pageChanged(event : any){
  this.userParams.pageNumber = event.page;
  this.loadMembers();
}

}

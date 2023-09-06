import { Component, OnInit } from '@angular/core';
import { MembersService } from '../_services/members.service';
import { Member } from '../_models/member';
import { Pagination } from '../_models/pagination';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {

  predicate: string = "liked";
  members: Partial<Member[]>;
  pageNumber = 1;
  pageSize = 5;
  
  pagination : Pagination = {
    currentPage : 1,
    itemsPerPage : this.pageSize,
    totalCount : 0,
    totalItems : 0
  };

  constructor(private memberService: MembersService) { }

  ngOnInit(): void {
  this.loadMembers();
  }

  loadMembers() {
    this.memberService.getLikes(this.predicate,this.pageSize,this.pageNumber).subscribe((response) => {
      
      this.members = response.result;
      this.pagination = response.pagination;
    })
  }

  pageChanged(event : any){

    this.pageNumber = event.page;
    this.loadMembers();
  }

}

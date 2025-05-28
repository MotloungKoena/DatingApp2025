/*import { Component, inject, OnInit } from '@angular/core';
import { Member } from '../../_models/member';
import { MembersService } from '../../_services/members.service';
import { MemberCardComponent } from "../member-card/member-card.component";
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { AccountService } from '../../_services/account.service';
import { UserParams } from '../../_models/userParams';
import { FormsModule } from '@angular/forms';
import {ButtonsModule} from 'ngx-bootstrap/buttons';

@Component({
  selector: 'app-member-list',
  standalone: true,
  templateUrl: './member-list.component.html',
  styleUrl: './member-list.component.css',
  imports: [MemberCardComponent, PaginationModule, FormsModule, ButtonsModule]
})
export class MemberListComponent implements OnInit {
  memberService = inject(MembersService);
  genderList =[{value: 'male', display: 'Males'}, {value: 'female', display: 'Females'}]

  


  ngOnInit(): void {
    if (!this.memberService.paginatedResult()) this.loadMembers();
  }

  /*loadMembers() {
    this.memberService.getMembers();
  }*/
 /*loadMembers() {
  this.memberService.getMembers().subscribe({
    next: response => {
      console.log('Members fetched:', response.result); // Optional debug
    },
    error: error => {
      console.error('Failed to load members:', error);
    }
  });
}


  resetFilters(){
    this.memberService.resetUserParams();
    this.loadMembers();
  }

  pageChanged(event: any) {
    if (this.memberService.userParams().pageNumber !== event.page) {
      this.memberService.userParams().pageNumber = event.page;
      this.loadMembers();
    }
  }
}*/

import { Component, inject, OnInit } from '@angular/core';
import { Member } from '../../_models/member';
import { Pagination } from '../../_models/pagination';
import { MembersService } from '../../_services/members.service';
import { MemberCardComponent } from "../member-card/member-card.component";
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { AccountService } from '../../_services/account.service';
import { UserParams } from '../../_models/userParams';
import { FormsModule } from '@angular/forms';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { CommonModule } from '@angular/common';
import { effect } from '@angular/core';


@Component({
  selector: 'app-member-list',
  standalone: true,
  templateUrl: './member-list.component.html',
  styleUrl: './member-list.component.css',
  imports: [MemberCardComponent, PaginationModule, FormsModule, ButtonsModule, CommonModule]
})
export class MemberListComponent implements OnInit {
  memberService = inject(MembersService);
  genderList = [{ value: 'male', display: 'Males' }, { value: 'female', display: 'Females' }];

  members: Member[] = [];
  pagination: Pagination | null | undefined = null;

  ngOnInit(): void {
    if (!this.memberService.paginatedResult()) this.loadMembers();
  }

/*ngOnInit(): void {
  this.loadMembers();
}*/



  loadMembers() {
    this.memberService.getMembers().subscribe({
      next: response => {
        this.members = response.result;
        this.pagination = response.pagination;
        console.log('Members fetched:', this.members);
      },
      error: error => {
        console.error('Failed to load members:', error);
      }
    });
  }

  resetFilters() {
    this.memberService.resetUserParams();
    this.loadMembers();
  }

  pageChanged(event: any) {
    if (this.memberService.userParams().pageNumber !== event.page) {
      this.memberService.userParams().pageNumber = event.page;
      this.loadMembers();
    }
  }

  trackById(index: number, member: Member) {
    return member.id;
  }
}

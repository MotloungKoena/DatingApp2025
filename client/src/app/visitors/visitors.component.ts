import { Component, OnInit, inject } from '@angular/core';
import { MembersService } from '../_services/members.service';
import { Visit } from '../_models/visit';
import { Pagination } from '../_models/pagination';
import { MemberCardComponent } from '../members/member-card/member-card.component';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-visitors',
  standalone: true,
  templateUrl: './visitors.component.html',
  styleUrls: ['./visitors.component.css'],
  imports: [CommonModule, FormsModule, PaginationModule, MemberCardComponent]
})
export class VisitorsComponent implements OnInit {
  private memberService = inject(MembersService);
  visits: Visit[] = [];
  predicate: string = 'visitedBy';  // or 'visited'
  filter: string = 'all';           // or 'month'
  pagination?: Pagination;
  pageNumber: number = 1;
  pageSize: number = 5;

  ngOnInit(): void {
    this.loadVisits();
  }

  loadVisits() {
    this.memberService.getVisits(this.predicate, this.filter, this.pageNumber, this.pageSize)
      .subscribe({
        next: response => {
          this.visits = response.result;
          this.pagination = response.pagination;
        },
        error: err => console.error('Failed to load visit data:', err)
      });
  }

  changePredicate(value: string) {
    this.predicate = value;
    this.pageNumber = 1;
    this.loadVisits();
  }

  changeFilter(value: string) {
    this.filter = value;
    this.pageNumber = 1;
    this.loadVisits();
  }

  pageChanged(event: any) {
    this.pageNumber = event.page;
    this.loadVisits();
  }
}

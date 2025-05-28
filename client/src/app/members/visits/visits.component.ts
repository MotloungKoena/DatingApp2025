/*import { Component, OnInit } from '@angular/core';
import { Visit } from '../../_models/visit';
import { PaginatedResult, Pagination } from '../../_models/pagination';
import { MembersService } from '../../_services/members.service';

@Component({
  selector: 'app-visits',
  templateUrl: './visits.component.html',
  styleUrls: ['./visits.component.css']
})
export class VisitsComponent implements OnInit {
  visits: Visit[] = [];
  predicate: string = 'visitedBy'; // or 'visited'
  filter: string = 'all'; // 'all' or 'month'
  pagination?: Pagination;
  pageNumber = 1;
  pageSize = 5;

  constructor(private memberService: MembersService) {}

  ngOnInit(): void {
    this.loadVisits();
  }

  loadVisits() {
  this.memberService.getVisits(this.predicate, this.filter, this.pageNumber, this.pageSize)
    .subscribe({
      next: (response: PaginatedResult<Visit[]>) => {
        this.visits = response.result;
        this.pagination = response.pagination;
      },
      error: (err: any) => {
        console.error('Failed to load visits:', err);
      }
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
}*/

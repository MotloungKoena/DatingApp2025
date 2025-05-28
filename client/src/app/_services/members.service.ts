import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';
import { of } from 'rxjs';
import { Photo } from '../_models/photo';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { Visit } from '../_models/visit';
import { map } from 'rxjs/operators';
import { setPaginatedResponse, setPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  private http = inject(HttpClient);
  private accountService = inject(AccountService);
  baseUrl = environment.apiUrl;
  paginatedResult = signal<PaginatedResult<Member[]> | null>(null);
  memberCache = new Map();
  user = this.accountService.currentUser();
  userParams = signal<UserParams>(new UserParams(this.user));

  resetUserParams() {
    this.userParams.set(new UserParams(this.user));
  }

  /*getMembers() {
    const response = this.memberCache.get(Object.values(this.userParams()).join('-'));
    if (response) return of(response.body);

    let params = setPaginationHeaders(this.userParams().pageNumber, this.userParams().pageSize);
    params = params.append('minAge', this.userParams().minAge);
    params = params.append('maxAge', this.userParams().maxAge);
    params = params.append('gender', this.userParams().gender);
    params = params.append('orderBy', this.userParams().orderBy);

    return this.http.get<Member[]>(this.baseUrl + 'users', { observe: 'response', params }).pipe(
      map(response => {
        const paginatedResult: PaginatedResult<Member[]> = {
          result: response.body as Member[],
          pagination: JSON.parse(response.headers.get('Pagination')!)
        };
        this.memberCache.set(Object.values(this.userParams()).join('-'), response);
        this.paginatedResult.set(paginatedResult);
        return paginatedResult;
      })
    );
  }*/
 getMembers() {

  let params = setPaginationHeaders(this.userParams().pageNumber, this.userParams().pageSize);
  params = params.append('minAge', this.userParams().minAge);
  params = params.append('maxAge', this.userParams().maxAge);
  params = params.append('gender', this.userParams().gender);
  params = params.append('orderBy', this.userParams().orderBy);

  return this.http.get<Member[]>(this.baseUrl + 'users', { observe: 'response', params }).pipe(
    map(response => {
      const paginatedResult: PaginatedResult<Member[]> = {
        result: response.body as Member[],
        pagination: JSON.parse(response.headers.get('Pagination')!)
      };
      this.paginatedResult.set(paginatedResult);
      return paginatedResult;
    })
  );
}


  getMember(username: string) {
    const member = [...this.memberCache.values()]
      .reduce((arr, elem) => arr.concat(elem.body), [] as Member[])
      .find((m: Member) => m.username === username);

    if (member) return of(member);
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member);
  }

  setMainPhoto(photo: Photo) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photo.id, {});
  }

  deletePhoto(photo: Photo) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photo.id);
  }

  trackProfileVisit(username: string) {
    return this.http.post(this.baseUrl + 'users/visited/' + username, {});
  }

  getVisitHistory() {
    return this.http.get<Visit[]>(this.baseUrl + 'users/visited');
  }

  getVisits(predicate: string, filter: string = 'all', pageNumber = 1, pageSize = 5) {
    let params = setPaginationHeaders(pageNumber, pageSize);
    params = params.append('predicate', predicate);
    params = params.append('filter', filter);

    const paginatedResult: PaginatedResult<Visit[]> = new PaginatedResult<Visit[]>();

    return this.http.get<Visit[]>(this.baseUrl + 'users/visits', {
      observe: 'response',
      params
    }).pipe(
      map(response => {
        setPaginatedResponse(response, paginatedResult);
        return paginatedResult;
      })
    );
  }
}

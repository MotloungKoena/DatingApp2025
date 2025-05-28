import { HttpClient, HttpParams, HttpResponse } from "@angular/common/http";
import { signal } from "@angular/core";
import { PaginatedResult } from "../_models/pagination";
import { map } from 'rxjs/operators';


/*export function setPaginatedResponse<T>(response: HttpResponse<T>,
  paginatedResultSignal: ReturnType<typeof signal<PaginatedResult<T> | null>>) {
  paginatedResultSignal.set({
    items: response.body as T,
    pagination: JSON.parse(response.headers.get('Pagination')!)
  })
}*/

export function setPaginationHeaders(pageNumber: number, pageSize: number) {
  let params = new HttpParams();

  if (pageNumber && pageSize) {
    params = params.append('pageNumber', pageNumber);
    params = params.append('pageSize', pageSize);
  }

  return params;
}

export function getPaginatedResult<T>(url: string, params: HttpParams, http: HttpClient) {
  const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();

  return http.get<T>(url, { observe: 'response', params }).pipe(
    map((response: HttpResponse<T>) => {
      paginatedResult.result = response.body!;
      if (response.headers.get('Pagination')) {
        paginatedResult.pagination = JSON.parse(response.headers.get('Pagination')!);
      }
      return paginatedResult;
    })
  );
}

/*export function setPaginatedResponse<T>(
  response: HttpResponse<T>
): PaginatedResult<T> {
  return {
    result: response.body as T,
    pagination: JSON.parse(response.headers.get('Pagination')!)
  };
}*/

export function setPaginatedResponse<T>(
  response: HttpResponse<T>,
  paginatedResult: PaginatedResult<T>
) {
  paginatedResult.result = response.body as T;
  const pagination = response.headers.get('Pagination');
  if (pagination) {
    paginatedResult.pagination = JSON.parse(pagination);
  }
}
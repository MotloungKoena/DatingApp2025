export interface Pagination {
    currentPage: number;
    itemsPerPage: number;
    totalItems: number;
    totalPages: number;
}

export class PaginatedResult<T> {
    items?: T; // Member[]
    result!: T;
    pagination?: Pagination;
}

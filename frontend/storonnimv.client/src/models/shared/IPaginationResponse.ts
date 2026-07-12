export interface IPaginationResponse<T> {
    currentPage: number;
    totalPages: number;
    totalItems: number;
    items: T[];
}
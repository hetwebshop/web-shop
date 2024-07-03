export interface PaginationParameters {
    pageNumber?: number;
    pageSize?: number;
    orderBy?: string;
}

export interface AdsPaginationParameters extends PaginationParameters {
    searchKeyword?: string;
    cityIds?: number[];
    jobCategoryIds?: number[];
    jobTypeIds?: number[];
    fromDate?: Date;
    toDate?: Date;
    advertisementTypeId?: number;
}

export class QueryObject {
    constructor(page, pageSize, search, searchBy, comparison, orderBy, sortBy) {
        this.page = page;
        this.pageSize = pageSize;
        this.searchBy = searchBy;
        this.search = search;
        this.comparison = comparison;
        this.orderBy = orderBy;
        this.sortBy = sortBy;
    }
}

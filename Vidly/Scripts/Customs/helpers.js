
export class Helpers {
    constructor() {
    }

    
    static formatDate(date, format = 'MM/DD/YYYY') {
        return date == null ? null : new moment(date).format(format);
    }

    static setQueryParams = query => {
        let queryParams = [];
        for (const key in query) {
            if (query[key] !== null && query[key] !== undefined && query[key] !== '' && query[key] !== 0) {
                if (Array.isArray(query[key])) {
                    if (query[key].length > 0) {
                        for (const val of query[key])
                            if (val !== null && val !== undefined && val !== '' && val !== 0) queryParams.push(`${key}=${val}`);
                    }
                } else {
                    queryParams.push(`${key}=${query[key]}`);
                }
            }
        }
        return queryParams.join('&');
    };

    static handleException = (request, message, error) => {
        let msg = `${error}<br />`;
        if (request.responseJSON) {
            const response = request.responseJSON;
            msg += `${response.message}<br />`;
            if (response.modelState) {
                for (const prop in response.modelState) {
                    msg += `${response.modelState[prop][0]}<br />`;
                }
            }
        }

        toastr.error(msg);
    }
    
}
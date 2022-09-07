function formatDate(date, format = 'MM/DD/YYYY') {
  return date == null ? null : new moment(date).format(format);
}

function setQueryParams(query) {
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
}



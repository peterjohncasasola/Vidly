﻿using System;
using System.Collections.Generic;

namespace Vidly.Customs.Extensions.Models
{
  public class QueryObject
  {
    
    public string SortBy { get; set; } = "asc";
    public string OrderBy { get; set; } = "Id";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SearchBy { get; set; } = "";
    public string Search { get; set; } = "";
    public string Comparison { get; set; } = "contains";
    public string Fields { get; set; } = "";
  }

  public class DateRange
  {
    public DateTime DateFrom { get; set; } = DateTime.Now;
    public DateTime DateTo { get; set; } = DateTime.Now;
  }

}
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Vidly.Customs.Data_Annotations
{
  public class DateRangeAttribute : ValidationAttribute
  {
    private DateTime Minimum { get; }
    private DateTime Maximum { get; }

    public DateRangeAttribute(string minimum = null, string maximum = null, string format = null)
      : base($@"{{0}} is invalid")
    {
      format = format ?? @"yyyy-MM-dd";

      Minimum = minimum == null ? DateTime.MinValue : DateTime.Parse(minimum);
      Maximum = maximum == null ? DateTime.Now : DateTime.Parse(maximum);

      if (Minimum > Maximum)
        throw new InvalidOperationException($"Specified max-date '{maximum}' is less than the specified min-date '{minimum}'");
    }
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      if (value == null) return ValidationResult.Success;

      if (value is string s && string.IsNullOrEmpty(s))
        return ValidationResult.Success;


      if ((DateTime) value < Minimum || (DateTime)value > Maximum)
        return new ValidationResult($@"Must between {Minimum.ToShortDateString()} and {Maximum.ToShortDateString()}");

      return ValidationResult.Success;
    }
  }
}
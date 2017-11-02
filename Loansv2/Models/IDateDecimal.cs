using System;

namespace Loansv2.Models
{
    public interface IDateDecimal
    {
        DateTime Date { get; set; }
        decimal Value { get; set; }
    }
}
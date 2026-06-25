using System;
using System.Collections.Generic;

namespace CommerceAPI.Models;

public partial class Payment
{
  public int PaymentId { get; set; }

  public int OrderId { get; set; }

  public decimal AmountPaid { get; set; }

  public string? Status { get; set; }

  public DateTime? PaymentDate { get; set; }

  public virtual Order? Order { get; set; }
}

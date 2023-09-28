using System;
using System.Collections.Generic;

namespace TaletApi.Models;

public partial class Sale
{
    public int Id { get; set; }

    public DateTime DateSold { get; set; }

    public int CustomerId { get; set; }

    public int ProductId { get; set; }

    public int StoreId { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual Store Store { get; set; } = null!;
}

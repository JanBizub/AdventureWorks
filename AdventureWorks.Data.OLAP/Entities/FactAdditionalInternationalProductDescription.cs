using System;
using System.Collections.Generic;

namespace AdventureWorks.Data.OLAP.Entities;

public partial class FactAdditionalInternationalProductDescription
{
    public int ProductKey { get; set; }

    public string CultureName { get; set; } = null!;

    public string ProductDescription { get; set; } = null!;
}

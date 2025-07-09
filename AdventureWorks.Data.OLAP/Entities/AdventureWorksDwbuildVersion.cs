using System;
using System.Collections.Generic;

namespace AdventureWorks.Data.OLAP.Entities;

public partial class AdventureWorksDwbuildVersion
{
    public string? Dbversion { get; set; }

    public DateTime? VersionDate { get; set; }
}

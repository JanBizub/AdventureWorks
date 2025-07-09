using System;
using System.Collections.Generic;

namespace AdventureWorks.Data.OLAP.Entities;

public partial class DimScenario
{
    public int ScenarioKey { get; set; }

    public string? ScenarioName { get; set; }
}

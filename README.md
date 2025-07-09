# AdventureWorks
Tech Showcase

---
# SQL Databases
**Commands to reference EF Core to the project**
- dotnet add package Microsoft.EntityFrameworkCore.SqlServer
- dotnet add package Microsoft.EntityFrameworkCore.Tools

**Scaffold Command that creates DBContext + EF Classes**
- ``dotnet ef dbcontext scaffold "Server=localhost;Database=AdventureWorksOLAP;Integrated Security=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o Entities --context AdventureWorksOLAPContext --context-dir .``
- ``dotnet ef dbcontext scaffold "Server=localhost;Database=AdventureWorksOLTP;Integrated Security=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o Entities --context AdventureWorksOLTPContext --context-dir .``

---

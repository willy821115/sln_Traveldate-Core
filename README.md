# sln_Traveldate-Core

改成連線資料庫
Scaffold-DbContext "Data Source=192.168.31.90;Initial Catalog=Traveldate;User ID=tdSQL;Password=1234;Trust Server Certificate=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -force

改成本機資料庫
Scaffold-DbContext "Data Source=.;Initial Catalog=Traveldate;Integrated Security=True;Trust Server Certificate=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -force

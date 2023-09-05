# sln_Traveldate-Core

改成連線資料庫
Scaffold-DbContext "Data Source=192.168.31.90;Initial Catalog=Traveldate;User ID=tdSQL;Password=1234;Trust Server Certificate=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -force

改成本機資料庫
Scaffold-DbContext "Data Source=.;Initial Catalog=Traveldate;Integrated Security=True;Trust Server Certificate=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -force

綠界測試付款
綠界提供測試用的特店管理後台，主要提供查詢測試的訂單相關資訊與執行訂單模擬付款的功能。
一般信用卡測試卡號 : 4311-9522-2222-2222 安全碼 : 222
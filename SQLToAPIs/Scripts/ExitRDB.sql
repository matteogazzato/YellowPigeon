insert INTO [TrafficBlocking]
select
'S' || cast ( S1.ID as nvarchar),
'S' || cast ( S2.ID as nvarchar),
0,
1,
'Exit route from station',
0,
-1,
0,
'',
0,
-1,
0,
-1,
0,
0
from
(select * from Segments) as S1
join
(select * from Segments) as S2
on S1.PEndID = S2.PStartID and S1.ID <> S2.ID
join
(select * from Points) as P1
on S1.PEndID = P1.ID
where
S1.TemplateName IN ('12_Fork_FW_Exit_Warehouse') AND
S2.TemplateName IN ('12_Fork_FW_Curve_v500');
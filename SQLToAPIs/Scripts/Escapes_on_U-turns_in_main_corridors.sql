
insert into [SegmentEscape]
select S1.ID,
       S2.PEndID,       
        1,
        1,
        600,        
        'B-esc on U-turns in main corridors',
        1,  --ChainBlockedAgvs  
        100,  --ChainDelay
        0, -- ChainDelay
        0, -- ChainBlockingDelay
        0, --ViaPointID
        0, --Different target
        0, --EvenIfNotAllocable
        0, --AlternativePointID
        '', --BlockingObject
        1, --BlockCause
        '',
        '',
        0,
        '',
        0,
        0,
        0,
        0,
        0,
        '',
        0,
        -1,
        0,
        0,
        -1        
from
(select * from Points) as P1
join
(select * from Segments) as S1
on P1.ID = S1.PEndID
join
(select * from Segments) as S2
on S1.PStartID = S2.PStartID and S1.ID <> S2.ID
join
(select * from Points) as P2
on S2.PEndID = P2.ID 
where
S1.TemplateName IN ('12_Fork_FW_Curve_U') AND 
S2.TemplateName IN ('12_Fork_FW');



insert into [SegmentEscape]
select S1.ID,
       S2.PEndID,       
        2,
        1,
        10,        
        'D-esc on U-turns in main corridors',
        0,  --ChainBlockedAgvs  
        0,  --ChainDelay
        0, -- ChainDelay
        0, -- ChainBlockingDelay
        0, --ViaPointID
        0, --Different target
        0, --EvenIfNotAllocable
        0, --AlternativePointID
        '', --BlockingObject
        1, --BlockCause
        '',
        '',
        0,
        '',
        0,
        0,
        0,
        0,
        0,
        '',
        0,
        -1,
        0,
        0,
        -1        
from
(select * from Points) as P1
join
(select * from Segments) as S1
on P1.ID = S1.PEndID
join
(select * from Segments) as S2
on S1.PStartID = S2.PStartID and S1.ID <> S2.ID
join
(select * from Points) as P2
on S2.PEndID = P2.ID 
where
where
S1.TemplateName IN ('12_Fork_FW_Curve_U') AND 
S2.TemplateName IN ('12_Fork_FW');
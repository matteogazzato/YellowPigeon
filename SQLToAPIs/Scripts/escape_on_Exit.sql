
insert into [SegmentEscape]
select S1.ID,
       S2.PEndID,       
        1,
        1,
        600,        
        'B-esc on exit',
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
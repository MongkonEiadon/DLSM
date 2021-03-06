USE [DLSM]
GO
/****** Object:  StoredProcedure [dbo].[sp_Stock]    Script Date: 29/8/2018 4:19:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--select * from product

ALTER PROC [dbo].[sp_Stock]
@WhID int,
@StID int

as

set nocount on
create table #tmp(PdID int,WhID int,ProductCode nvarchar(30),ProductName nvarchar(255),Qty int,Borrow int, Transfer int,CheckStatus char(1),MinStock int,Predict numeric(15,2))

create table #rpt2
	(docid int, whid int, pdid int, qty int, processdate datetime,serialbegin nvarchar(255),serialend nvarchar(255))

declare @percentpredict numeric(5,2), @pdid int ,@PredictMonth int,@date1 nvarchar(30),@date2 nvarchar(30), @TotalMonths int = 1
select @percentpredict = [value]/100.00  from Configure where Code = 'PREDICT'

insert into #tmp
select	p.ID,
				@WhID,
				p.Code,
				p.Name,
				0,
				0,
				0,
				'N',
				p.MinStock ,
				CAST(0 as Numeric) as Predict
		from product p
		where not exists(select 1 from stock where PdID = p.ID and whid=@WhID)
union

select	s.PdID,
		s.WhID,
		p.Code ,
		p.Name ,
		s.Qty,
		s.Borrow,
		s.Transfer,
		s.CheckStatus,
		wm.MinStock ,
		CAST(0 as Numeric) as Predict
		from product p 
		join stock s on s.PdId = p.ID
		join warehouse w on s.WhID = w.ID
		left join warehouseminimum wm on wm.PdID = s.PdID and wm.WhID = s.WhID
		where s.WhID = @WhID
		and isnull(wm.PredictMonth , 0) = 0

declare cur cursor for 
select	s.PdID,
		isnull(wm.PredictMonth,0)	
from product p 
join stock s on s.PdId = p.ID
join warehouse w on s.WhID = w.ID
join warehouseminimum wm on wm.PdID = s.PdID and wm.WhID = s.WhID
where s.WhID = @WhID
and  isnull(wm.PredictMonth , 0) > 0
and not exists (select 1 from #tmp where PdID = p.ID)
open cur
fetch next from cur into @pdid,@PredictMonth
while @@fetch_status <> -1
begin
		set @date2 = CAST(GETDATE() as varchar)
		set @date1 = CAST(dateadd(mm,-12,@date2) as varchar)
		

		--select @date1,@date2,@pdid
		insert into #rpt2
		exec sp_RequisitionStock
		@date1 = @date1,
		@date2 = @date2,
		@whid = @WhID,
		@pdid = @pdid,
		@stid = @StID,
		@stidpermit = @StID
		
		SELECT @TotalMonths = DATEDIFF(month, Min(d.[ProcessDate]), GETDATE())
			FROM [DLSM].[dbo].[DocumentDetail] d
			JOIN SerialOut s on d.SerialBegin = s.SerialNo
			Where d.[ProcessDate] is not Null

		IF @TotalMonths > 12
			set @TotalMonths = 12
		
		
		insert into #tmp 
		select	s.PdID,
		s.WhID,
		p.Code,
		p.Name,
		s.Qty,
		s.Borrow,
		s.Transfer,
		s.CheckStatus,
		wm.MinStock ,
		(isnull((select sum(qty) from #rpt2 ), 0)/@TotalMonths)*@percentpredict*@PredictMonth
		from product p 
		join stock s on s.PdId = p.ID
		join warehouse w on s.WhID = w.ID
		left outer join warehouseminimum wm on wm.PdID = s.PdID and wm.WhID = s.WhID
		where s.WhID = @WhID
		and p.ID = @pdid
	
	delete from #rpt2
	fetch next from cur into @pdid,@PredictMonth
end
close cur
deallocate cur


select * from #tmp order by ProductCode
drop table #tmp
drop table #rpt2 




		


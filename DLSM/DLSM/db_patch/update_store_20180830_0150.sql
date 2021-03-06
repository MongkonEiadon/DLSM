USE [DLSM]
GO
/****** Object:  StoredProcedure [dbo].[sp_Stock]    Script Date: 29/8/2018 5:16:13 PM ******/
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
create table #tmp(PdID int,WhID int,ProductCode nvarchar(30),ProductName nvarchar(255),Qty int,Borrow int, Transfer int,CheckStatus char(1),MinStock int,Predict numeric(15,2),TotalMonths int)

create table #rpt2
	(docid int, whid int, pdid int, qty int, processdate datetime,serialbegin nvarchar(255),serialend nvarchar(255))

declare @percentpredict numeric(5,2), @pdid int,@pdcode nvarchar(30) ,@PredictMonth int,@date1 nvarchar(30),@date2 nvarchar(30), @TotalMonths int = 1, @AverageQty int, @CardAverageQty int
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
				isnull(p.MinStock, 0) as MinStock,
				CAST(0 as Numeric) as Predict,
				@TotalMonths as TotalMonths
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
		isnull(wm.MinStock, 0) as MinStock,
		CAST(0 as Numeric) as Predict,
		@TotalMonths as TotalMonths
		from product p 
		join stock s on s.PdId = p.ID
		join warehouse w on s.WhID = w.ID
		left join warehouseminimum wm on wm.PdID = s.PdID and wm.WhID = s.WhID
		where s.WhID = @WhID
		and isnull(wm.PredictMonth , 0) = 0
order by p.Code

declare cur cursor for 
select	s.PdID,
		p.Code,
		isnull(wm.PredictMonth,0)	
from product p 
join stock s on s.PdId = p.ID
join warehouse w on s.WhID = w.ID
join warehouseminimum wm on wm.PdID = s.PdID and wm.WhID = s.WhID
where s.WhID = @WhID
and  isnull(wm.PredictMonth , 0) > 0
and not exists (select 1 from #tmp where PdID = p.ID)
order by p.Code

open cur
fetch next from cur into @pdid, @pdcode, @PredictMonth
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
			Where d.PdID = @pdid and d.[ProcessDate] is not Null

		IF @TotalMonths > 12
			set @TotalMonths = 12			
		
		select @AverageQty = isnull(sum(qty), 0.0)/@TotalMonths  from #rpt2
		
		insert into #tmp 
		select	s.PdID,
		s.WhID,
		p.Code,
		p.Name,
		s.Qty,
		s.Borrow,
		s.Transfer,
		s.CheckStatus,
		isnull(wm.MinStock, 0) as MinStock,
		isnull(@AverageQty, 0)*@percentpredict*@PredictMonth as Predict,
		@TotalMonths as TotalMonths
		from product p 
		join stock s on s.PdId = p.ID
		join warehouse w on s.WhID = w.ID
		left outer join warehouseminimum wm on wm.PdID = s.PdID and wm.WhID = s.WhID
		where s.WhID = @WhID
		and p.ID = @pdid

		if @pdcode = '0100300'
			select @CardAverageQty = MinStock/@TotalMonths from #tmp where TRIM(ProductCode) = '0100300'
	
	delete from #rpt2
	fetch next from cur into @pdid, @pdcode,@PredictMonth
end
close cur
deallocate cur

select  PdID,
		WhID, 
		ProductCode, 
		ProductName , 
		Qty, 
		Borrow, 
		Transfer, 
		CheckStatus, 
		MinStock, 
		CASE WHEN TRIM(ProductCode) in ('02020222','02020223') 
			THEN ceiling(@CardAverageQty/500.0)
			ELSE Predict END as Predict
		--,@CardAverageQty as CardAverageQty  --uncomment for debug
		--,TotalMonths  --uncomment for debug
from #tmp t order by ProductCode
drop table #tmp
drop table #rpt2 




		


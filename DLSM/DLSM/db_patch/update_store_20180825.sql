USE [DLSM]
GO

/****** Object:  StoredProcedure [dbo].[sp_SearchCard_bak_20180825]    Script Date: 25/08/2018 13:27:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[sp_SearchCard_bak_20180825]
	@serialBegin varchar(255), 
	@serialEnd varchar(255), 
	@BeginDate Datetime, 
	@EndDate Datetime 
as
begin
--This is backup of [dbo].[sp_SearchCard] as of 25 aug 2018 Create Backup by Narong Sungkhamalai.
	declare @FailCard table (id int , SerialBegin varchar(255), whid int)
	declare @sBegin decimal(15,0), @sEnd decimal(15,0), @whid int
	declare @i int = 1

	select isnull(convert(decimal(15,0),ss.SerialBegin),0) SerialBegin,
			isnull(convert(decimal(15,0),ss.SerialEnd),0) SerialEnd,
			ss.WhID
	into #tmp 
	from StockSerial ss
	left join DocumentDetail dd on dd.DocID = ss.DocID
	left join Document d on d.ID = ss.DocID
	--where '%' + @serialBegin + '%' between ss.SerialBegin and ss.SerialEnd or @serialBegin is null
	--and '%' + @serialEnd + '%' between ss.SerialBegin and ss.SerialEnd or @serialEnd is null
	where ss.SerialBegin like '%' + @serialBegin +'%' or @serialBegin = '0' 
	and ss.SerialEnd like '%' + isnull(@serialEnd,'') + '%' or @serialEnd = '0'
	and d.DocDate >= @BeginDate or @BeginDate is null 
	and d.DocDate <= @EndDate or @EndDate is null 

	declare cur_pointer cursor for
	SELECT * 
	FROM #tmp

	open cur_pointer 
	fetch from cur_pointer into @sBegin, @sEnd, @whid
	while @@fetch_status = 0
	begin
		--select @sBegin,@sEnd
		while @sBegin <= @sEnd
			begin
				insert into @FailCard
				select @i, Convert(varchar(255),@sBegin), @whid

				set @i = @i + 1
				set @sBegin = @sBegin + 1 
			end
		fetch from cur_pointer into @sBegin, @sEnd, @whid
	end
		
	close cur_pointer
	deallocate cur_pointer

	select * from @FailCard order by id

	drop table #tmp

end
GO

-- This is Create update store [dbo].[sp_SearchCard] by query method, discard cursor method
-- Create By Narong Sungkhamalai
USE [DLSM]
GO

/****** Object:  StoredProcedure [dbo].[sp_SearchCard]    Script Date: 25/08/2018 13:29:35 ******/
DROP PROCEDURE [dbo].[sp_SearchCard]
GO

/****** Object:  StoredProcedure [dbo].[sp_SearchCard]    Script Date: 25/08/2018 13:29:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE procedure [dbo].[sp_SearchCard]
	@serialBegin varchar(255)='', 
	@serialEnd varchar(255)='', 
	@BeginDate Datetime, 
	@EndDate Datetime 
as
begin

    DECLARE @card_product_id int
	SET @card_product_id=11; --11 is CARD
	select  
		 CAST(ROW_NUMBER() OVER(ORDER BY ss.ID ASC) AS int) AS id
		  , ss.SerialBegin
		  ,	ss.WhID
		from StockSerial ss
	left join DocumentDetail dd on dd.DocID = ss.DocID
	left join Document d on d.ID = ss.DocID
	where (ss.SerialBegin like '%' + isnull(@serialBegin,'') +'%' or @serialBegin = '0' )
	and (ss.SerialEnd like '%' + isnull(@serialEnd,'') + '%' or @serialEnd = '0')
	and (d.DocDate >= @BeginDate or @BeginDate is null) 
	and (d.DocDate <= @EndDate or @EndDate is null )
	and ss.PdID=@card_product_id
	and ss.SerialCount!=0

end
GO



/*    ==Scripting Parameters==

    Source Server Version : SQL Server 2017 (14.0.800)
    Source Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Source Database Engine Type : Standalone SQL Server

    Target Server Version : SQL Server 2017
    Target Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Target Database Engine Type : Standalone SQL Server
*/

USE [DLSM]
GO

ALTER TABLE [dbo].[SerialOut] DROP CONSTRAINT [FK_SerialOut_DocumentDetail]
GO

/****** Object:  Table [dbo].[SerialOut]    Script Date: 1/9/2560 11:41:25 ******/
DROP TABLE [dbo].[SerialOut]
GO

/****** Object:  Table [dbo].[SerialOut]    Script Date: 1/9/2560 11:41:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SerialOut](
	[SerialNo] [varchar](40) NOT NULL,
	[DocID] [int] NOT NULL,
	[RefNo] [varchar](40) NULL,
	[RefTitle] [varchar](30) NULL,
	[RefFirstName] [varchar](30) NULL,
	[RefLastName] [varchar](30) NULL,
	[RefIssueDate] [datetime] NULL,
	[RefExpireDate] [datetime] NULL,
	[RefImage] [varbinary](max) NULL,
	[RefType] [varchar](2) NULL,
	[ProcessDate] [datetime] NULL,
	[SeqNo] [int] NULL,
	[PrintStatus] [char](1) NULL,
	[PrinterStatus] [text] NULL,
 CONSTRAINT [PK_SerialOut] PRIMARY KEY CLUSTERED 
(
	[SerialNo] ASC,
	[DocID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[SerialOut]  WITH CHECK ADD  CONSTRAINT [FK_SerialOut_DocumentDetail] FOREIGN KEY([DocID], [SeqNo])
REFERENCES [dbo].[DocumentDetail] ([DocID], [SeqNo])
GO

ALTER TABLE [dbo].[SerialOut] CHECK CONSTRAINT [FK_SerialOut_DocumentDetail]
GO

/*    ==Scripting Parameters==

    Source Server Version : SQL Server 2017 (14.0.800)
    Source Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Source Database Engine Type : Standalone SQL Server

    Target Server Version : SQL Server 2017
    Target Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Target Database Engine Type : Standalone SQL Server
*/

USE [DLSM]
GO

/****** Object:  Table [dbo].[SerialImagePart]    Script Date: 1/9/2560 11:41:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SerialImagePart](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SerialNo] [varchar](40) NOT NULL,
	[ImageSize] [varchar](20) NOT NULL,
	[PartSize] [varchar](20) NOT NULL,
	[PartPosition] [varchar](20) NOT NULL,
	[PartData] [text] NULL,
 CONSTRAINT [PK_SerialImagePart] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/*    ==Scripting Parameters==

    Source Server Version : SQL Server 2017 (14.0.800)
    Source Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Source Database Engine Type : Standalone SQL Server

    Target Server Version : SQL Server 2017
    Target Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Target Database Engine Type : Standalone SQL Server
*/

USE [DLSM]
GO

/****** Object:  StoredProcedure [dbo].[sp_CreateCardUsageDoc]    Script Date: 1/9/2560 11:42:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_CreateCardUsageDoc]
@whID int,
@stID int,
@CardNo varchar(40)
AS
BEGIN
	SET NOCOUNT ON;
	declare @automanager int

	select @automanager = StID from StaffWarehouse where WhID = @WhID and IsManager= '1'

	declare @CardPdID int 
	select @CardPdID = ID from Product where Code = '03' -- 03 is product code

	declare @newID int, @spID int
	select @spID = ID from Supplier where Name like '%ไม่ระบุ%'
	
	declare @whcode varchar(4)
	select @whcode = code from Warehouse where id = @whID

	declare @docno varchar(20)
	select @docno = 'PN-'
					+ @whcode
					+ '-'
					+ substring(convert(varchar(10),getdate(),112),3,4)

	declare @countPRN int
	select @countPRN = count(*) 
	from document 
	where DocNo like @docno + '%'

	select @countPRN = isnull(@countPRN,0) + 1

	declare @run varchar(20)
	select @run = '00000000' + convert(varchar(8),@countPRN)
	select @run = substring(@run, datalength(@run) - 7, 8)
	select @docno = @docno + '-' + @run

	insert into document 
	(docno, docdate, CreateBy, docType, whID, SpID, Remark, Status, ApproveDate, ApproveBy)
	select 
	 @docno, 
	 convert(datetime,convert(varchar(20),getdate(),112)),
	 @stID,
	 '2',
	 @WhID,
	 @SpID,
	 '',
	 '3',
	 getdate(),
	 @automanager

	if @@ERROR <> 0
		return -99

	select @newID = @@IDENTITY 

	insert into DocumentDetail
	(docid, pdid, trntype, qty, SerialBegin, SerialEnd)
	values
	(@newid, @CardPdID, 'O', 1, @CardNo, @CardNo)

	if @@ERROR <> 0
		return -99

	declare @retcode int
	exec @retcode = sp_ProcessDocument @newid, @stid
	if @retcode < 0
		return @retcode
	else
		return @newID
END
GO



/*    ==Scripting Parameters==

    Source Server Version : SQL Server 2017 (14.0.800)
    Source Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Source Database Engine Type : Standalone SQL Server

    Target Server Version : SQL Server 2017
    Target Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Target Database Engine Type : Standalone SQL Server
*/

USE [DLSM]
GO

/****** Object:  StoredProcedure [dbo].[sp_ApiCheckCardSerial]    Script Date: 1/9/2560 11:41:57 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_ApiCheckCardSerial]
@whid int,
@serialNo varchar(40)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @CardPdID int 
	select @CardPdID = ID from Product where Code = '03' -- 03 is product code

	select * from StockSerial 
	where WhID = @whID 
	and PdID = @CardPdID
	and SerialCount > 0 
	and @serialNo between SerialBegin and SerialEnd

END
GO

/*    ==Scripting Parameters==

    Source Server Version : SQL Server 2017 (14.0.800)
    Source Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Source Database Engine Type : Standalone SQL Server

    Target Server Version : SQL Server 2017
    Target Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Target Database Engine Type : Standalone SQL Server
*/

USE [DLSM]
GO

/****** Object:  StoredProcedure [dbo].[sp_ApiUpdateCardInfo]    Script Date: 1/9/2560 11:42:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_ApiUpdateCardInfo]
@StID int,
@WhID int,
@SerialNo varchar(40),
@RefNo varchar(40),
@RefTitle varchar(30),
@RefFirstName varchar(30),
@RefLastName varchar(30),
@RefIssueDate datetime,
@RefExpireDate datetime,
@RefType varchar(2)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	Declare @DocID int

	exec @DocID = [sp_CreateCardUsageDoc] @WhID, @StID, @SerialNo

	if @DocID < 0
	begin
		select -1 as Code
		return -99
	end

	INSERT INTO [dbo].[SerialOut]
           ([SerialNo]
           ,[DocID]
           ,[RefNo]
           ,[RefTitle]
           ,[RefFirstName]
           ,[RefLastName]
           ,[RefIssueDate]
           ,[RefExpireDate]
           ,[RefType]
           ,[ProcessDate]
		   ,[PrintStatus])
     VALUES
           (@SerialNo
           ,@DocID
           ,@RefNo
           ,@RefTitle
           ,@RefFirstName
           ,@RefLastName
           ,@RefIssueDate
           ,@RefExpireDate
           ,@RefType
		   ,getdate()
		   ,'N')

	if @@ERROR <> 0
	begin
		select -1 as Code
		return - 99
	end
	select 0 as Code
END
GO

/*    ==Scripting Parameters==

    Source Server Version : SQL Server 2017 (14.0.800)
    Source Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Source Database Engine Type : Standalone SQL Server

    Target Server Version : SQL Server 2017
    Target Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Target Database Engine Type : Standalone SQL Server
*/

USE [DLSM]
GO

/****** Object:  StoredProcedure [dbo].[sp_ApiUpdatePicture]    Script Date: 1/9/2560 11:42:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_ApiUpdatePicture]
@SerialNo varchar(40),
@ImageSize varchar(20),	
@PartSize varchar(20),
@PartPosition varchar(20),
@PartData text
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO [dbo].[SerialImagePart]
           ([SerialNo]
           ,[ImageSize]
           ,[PartSize]
           ,[PartPosition]
           ,[PartData])
     VALUES
           (@SerialNo
           ,@ImageSize
           ,@PartSize
           ,@PartPosition
           ,@PartData)
	
	SELECT @@IDENTITY as ID
END
GO

/*    ==Scripting Parameters==

    Source Server Version : SQL Server 2017 (14.0.800)
    Source Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Source Database Engine Type : Standalone SQL Server

    Target Server Version : SQL Server 2017
    Target Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Target Database Engine Type : Standalone SQL Server
*/

USE [DLSM]
GO

/****** Object:  StoredProcedure [dbo].[sp_ApiUpdatePrintStatus]    Script Date: 1/9/2560 11:42:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_ApiUpdatePrintStatus]
@SerialNo varchar(40),
@PrintStatus char(1),
@PrinterStatus text
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	update [dbo].[SerialOut] set
		PrintStatus = @PrintStatus,
		PrinterStatus = @PrinterStatus
    where SerialNo = @SerialNo

	if @@ERROR <> 0
	begin
		select -1 as Code
		return - 99
	end
	select 0 as Code
END
GO

/*    ==Scripting Parameters==

    Source Server Version : SQL Server 2017 (14.0.800)
    Source Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Source Database Engine Type : Standalone SQL Server

    Target Server Version : SQL Server 2017
    Target Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Target Database Engine Type : Standalone SQL Server
*/

USE [DLSM]
GO

/****** Object:  StoredProcedure [dbo].[sp_RptBalance]    Script Date: 1/9/2560 11:42:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_RptBalance]
@whid int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT 
		w.ID WhID,
		w.Code WhCode,
		w.Name WhName,
		p.ID PdID,
		p.Code PdCode,
		p.Name PdName,
		p.MinStock,
		s.Qty,
		s.Transfer,
		s.Borrow
	from STOCK s
	inner join Warehouse w on s.WhID = w.ID
	inner join Product p on s.PdID = p.ID
	where (s.WhID = @whid or @whid = 0)
	order by w.Code, p.Code
END
GO

/*    ==Scripting Parameters==

    Source Server Version : SQL Server 2017 (14.0.800)
    Source Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Source Database Engine Type : Standalone SQL Server

    Target Server Version : SQL Server 2017
    Target Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Target Database Engine Type : Standalone SQL Server
*/

USE [DLSM]
GO

/****** Object:  StoredProcedure [dbo].[sp_RptMovement]    Script Date: 1/9/2560 11:43:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_RptMovement]
	@date1 varchar(30),
	@date2 varchar(30),
	@whid int,
	@pdid int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @d1 datetime, @d2 datetime
	select @d1 = Convert(DateTime,@date1),
			@d2 = Convert(DateTime,@date2)

    create table #rpt(
		DocID int,
		DocNo varchar(20),
		DocType char(3),
		CreateBy int,
		WhID int,
		PdID int,
		Qty int,
		BF int,
		CF int,
		ProcessDate datetime
	)
    
	insert into #rpt
	select
		d.DocID,
		h.DocNo,
		h.DocType,
		h.CreateBy,
		h.WhID,
		d.PdID,
		d.Qty,
		d.BF,
		d.CF,
		d.ProcessDate
	from DocumentDetail d
	inner join Document h
		on h.ID = d.DocID
	where d.ProcessDate between @d1 and @d2
	and h.DocType in ('1', '2') -- รับ, จ่าย
	and h.Status = '3' -- อนุมัติ
	and (h.WhID = @whid or @whid = 0)
	and (d.PdID = @pdid or @pdid = 0)
	
	insert into #rpt
	select
		d.DocID,
		h.DocNo,
		h.DocType,
		h.CreateBy,
		h.WhID,
		d.PdID,
		d.Qty,
		d.BF,
		d.CF,
		d.ProcessDate
	from DocumentDetail d
	inner join Document h
		on h.ID = d.DocID
	where d.ProcessDate between @d1 and @d2
	and h.DocType in ('3', '4', '5') -- โอน, ยืม, คืน
	and h.Status in ('3','5','6','7','8') -- อนุมัติออกแล้วแสดงหมด
	and d.TrnType = 'O'
	and (h.WhID = @whid or @whid = 0)
	and (d.PdID = @pdid or @pdid = 0)

	insert into #rpt
	select
		d.DocID,
		h.DocNo,
		h.DocType,
		h.CreateBy,
		h.WhID,
		d.PdID,
		d.Qty,
		d.BF,
		d.CF,
		d.ProcessDate
	from DocumentDetail d
	inner join Document h
		on h.ID = d.DocID
	where d.ProcessDate between @d1 and @d2
	and h.DocType in ('3', '4', '5') -- โอน, ยืม, คืน
	and h.Status in ('6','8') -- อนุมัติรับของ, รับคืน
	and d.TrnType = 'I'
	and (h.WhID = @whid or @whid = 0)
	and (d.PdID = @pdid or @pdid = 0)
	
	select
		r.DocID,
		r.DocNo,
		r.DocType,
		r.CreateBy,
		r.WhID,
		r.PdID,
		r.Qty,
		r.BF,
		r.CF,
		r.ProcessDate,
		s.Name StName,
		p.Code PdCode,
		p.Name PdName,
		w.Code WhCode,
		w.Name WhName
	from #rpt r
	inner join Staff s on r.CreateBy = s.ID
	inner join Product p on r.PdID = p.ID
	inner join Warehouse w on r.WhID = w.ID
	order by w.Code, p.Code, r.ProcessDate

	drop table #rpt
END
GO

/*    ==Scripting Parameters==

    Source Server Version : SQL Server 2017 (14.0.800)
    Source Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Source Database Engine Type : Standalone SQL Server

    Target Server Version : SQL Server 2017
    Target Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Target Database Engine Type : Standalone SQL Server
*/

USE [DLSM]
GO

/****** Object:  StoredProcedure [dbo].[sp_RptNotEnough]    Script Date: 1/9/2560 11:43:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_RptNotEnough]
@whid int
AS
BEGIN
	
	SET NOCOUNT ON;
	create table #rpt (
		whid int,
		pdid int,
		minqty int
	)

	insert into #rpt
	select w.id, p.id, p.minstock
	from Product p
	cross join Warehouse w
	where (w.id = @whid or @whid = 0)

    SELECT
		w.ID WhID,
		w.Code WhCode,
		w.Name WhName,
		p.ID PdID,
		p.Code PdCode,
		p.Name PdName,
		p.MinStock,
		isnull(s.Qty,0) Qty
	FROM #rpt r
	join Product p
		on r.pdid = p.id
	left join Warehouse w
		on r.whid = w.id
	left join Stock s
		on r.pdid = s.pdid
		and r.whid = s.whid
	WHERE (isnull(s.Qty,0) <= p.MinStock)
	order by w.ID, p.ID


	drop table #rpt
END
GO


/*    ==Scripting Parameters==

    Source Server Version : SQL Server 2017 (14.0.800)
    Source Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Source Database Engine Type : Standalone SQL Server

    Target Server Version : SQL Server 2017
    Target Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Target Database Engine Type : Standalone SQL Server
*/

USE [DLSM]
GO

/****** Object:  StoredProcedure [dbo].[sp_RptReceive]    Script Date: 1/9/2560 11:43:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_RptReceive] 
	@date1 varchar(30),
	@date2 varchar(30),
	@whid int,
	@pdid int,
	@stid int
AS
BEGIN
	
	SET NOCOUNT ON;
	declare @d1 datetime, @d2 datetime
	select @d1 = convert(datetime,@date1), @d2 = convert(datetime,@date2)

	create table #rpt
	(docid int, whid int, pdid int, qty int, processdate datetime)
    
	insert into #rpt
	select h.id, h.whid, d.pdid, d.qty, d.processdate
	from document h
	inner join DocumentDetail d
		on h.id = d.docid and d.TrnType = 'I'
	inner join product p
		on d.pdid = p.id
	where (d.ProcessDate between @d1 and @d2)
	and (h.whid = @whid or @whid = 0)
	and (h.createby = @stid or @stid = 0)
	and (d.pdid = @pdid or @pdid = 0)
	and (h.doctype in ('1','2')) -- receive, requisition
	and (h.status = '3') -- approve
	
	insert into #rpt
	select h.id, h.towhid, d.pdid, d.qty, d.processdate
	from document h
	inner join DocumentDetail d
		on h.id = d.docid and d.TrnType = 'I'
	inner join product p
		on d.pdid = p.id
	where (d.ProcessDate between @d1 and @d2)
	and (h.TOWHID = @whid or @whid = 0)
	and (h.createby = @stid or @stid = 0)
	and (d.pdid = @pdid or @pdid = 0)
	and (h.doctype = '3') -- transfer
	and (h.status in ('6','8')) -- receive approve, return receive

	insert into #rpt
	select h.id, h.towhid, d.pdid, d.qty, d.processdate
	from document h
	inner join DocumentDetail d
		on h.id = d.docid and d.TrnType = 'I'
	inner join product p
		on d.pdid = p.id
	where (d.ProcessDate between @d1 and @d2)
	and (h.TOWHID = @whid or @whid = 0)
	and (h.createby = @stid or @stid = 0)
	and (d.pdid = @pdid or @pdid = 0)
	and (h.doctype in ('4','5')) -- borrow/return
	and (h.status in ('6','8')) -- receive approve, return receive

	select
		r.WhID,
		w.Name WhName,
		r.DocID, 
		h.DocNo,
		h.DocDate,
		h.DocType,
		(case h.DocType 
		when '1' then 'รับ'
		when '2' then 'จ่าย'
		when '3' then 'โอนย้าย'
		when '4' then 'ยืม'
		when '5' then 'คืน(ยืม)'
		end) DocTypeName,
		s.ID ReceiveByID,
		s.Name ReceiveByName,
		p.ID PdID,
		p.Code PdCode,
		p.Name PdName,
		r.qty,
		r.ProcessDate
	from #rpt r
	inner join document h
		on r.docid = h.id
	inner join warehouse w
		on r.whid = w.id
	inner join product p
		on r.pdid = p.id
	inner join Staff s
		on h.CreateBy = s.ID
	order by r.WhID, r.ProcessDate

	drop table #rpt		
END
GO

/*    ==Scripting Parameters==

    Source Server Version : SQL Server 2017 (14.0.800)
    Source Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Source Database Engine Type : Standalone SQL Server

    Target Server Version : SQL Server 2017
    Target Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Target Database Engine Type : Standalone SQL Server
*/

USE [DLSM]
GO

/****** Object:  StoredProcedure [dbo].[sp_RptRequisition]    Script Date: 1/9/2560 11:43:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_RptRequisition]
	@date1 varchar(30),
	@date2 varchar(30),
	@whid int,
	@pdid int,
	@stid int
AS
BEGIN
	
	SET NOCOUNT ON;
	declare @d1 datetime, @d2 datetime
	select @d1 = convert(datetime,@date1), @d2 = convert(datetime,@date2)

	create table #rpt
	(docid int, whid int, pdid int, qty int, processdate datetime)
    
	insert into #rpt
	select h.id, h.whid, d.pdid, d.qty, d.processdate
	from document h
	inner join DocumentDetail d
		on h.id = d.docid and d.TrnType = 'O'
	inner join product p
		on d.pdid = p.id
	where (d.ProcessDate between @d1 and @d2)
	and (h.whid = @whid or @whid = 0)
	and (h.createby = @stid or @stid = 0)
	and (d.pdid = @pdid or @pdid = 0)
	and (h.doctype in ('1','2')) -- receive, requisition
	and (h.status = '3') -- approve
	
	insert into #rpt
	select h.id, h.towhid, d.pdid, d.qty, d.processdate
	from document h
	inner join DocumentDetail d
		on h.id = d.docid and d.TrnType = 'O'
	inner join product p
		on d.pdid = p.id
	where (d.ProcessDate between @d1 and @d2)
	and (h.TOWHID = @whid or @whid = 0)
	and (h.createby = @stid or @stid = 0)
	and (d.pdid = @pdid or @pdid = 0)
	and (h.doctype = '3') -- transfer
	and (h.status in ('3','5','6','7','8')) 

	insert into #rpt
	select h.id, h.towhid, d.pdid, d.qty, d.processdate
	from document h
	inner join DocumentDetail d
		on h.id = d.docid and d.TrnType = 'O'
	inner join product p
		on d.pdid = p.id
	where (d.ProcessDate between @d1 and @d2)
	and (h.TOWHID = @whid or @whid = 0)
	and (h.createby = @stid or @stid = 0)
	and (d.pdid = @pdid or @pdid = 0)
	and (h.doctype in ('4','5')) -- borrow/return
	and (h.status in ('3','5','6','7','8')) -- receive approve, return receive

	select
		r.WhID,
		w.Name WhName,
		r.DocID, 
		h.DocNo,
		h.DocDate,
		h.DocType,
		(case h.DocType 
		when '1' then 'รับ'
		when '2' then 'จ่าย'
		when '3' then 'โอนย้าย'
		when '4' then 'ยืม'
		when '5' then 'คืน(ยืม)'
		end) DocTypeName,
		s.ID ReceiveByID,
		s.Name ReceiveByName,
		p.ID PdID,
		p.Code PdCode,
		p.Name PdName,
		r.qty,
		r.ProcessDate
	from #rpt r
	inner join document h
		on r.docid = h.id
	inner join warehouse w
		on r.whid = w.id
	inner join product p
		on r.pdid = p.id
	inner join Staff s
		on h.CreateBy = s.ID
	order by r.WhID, r.ProcessDate

	drop table #rpt		
END
GO

/*    ==Scripting Parameters==

    Source Server Version : SQL Server 2017 (14.0.800)
    Source Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Source Database Engine Type : Standalone SQL Server

    Target Server Version : SQL Server 2017
    Target Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Target Database Engine Type : Standalone SQL Server
*/

USE [DLSM]
GO

/****** Object:  StoredProcedure [dbo].[sp_RptTransfer]    Script Date: 1/9/2560 11:43:31 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_RptTransfer]
	@whid int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT
		d.DocID,
		h.DocNo,
		h.DocType,
		h.CreateBy,
		s.Name StName,
		h.WhID,
		w.Code WhCode,
		w.Name WhName,
		h.ToWhID,
		t.Code ToWhCode,
		t.Name ToWhName,
		d.PdID,
		p.Code PdCode,
		p.Name PdName,
		d.Qty,
		d.ProcessDate
	FROM documentdetail d
	inner join document h on d.docid = h.id
	inner join Staff s on h.CreateBy = s.ID
	inner join Warehouse w on h.WhID = w.ID
	inner join Warehouse t on h.ToWhID = t.ID
	inner join Product p on d.PdID = p.ID
	WHERE (h.WhID = @whid or @whid = 0)
	and (h.DocType in ('3','4','5'))
	and (h.status in ('3','5','7'))
	and (d.TrnType = 'O')
	order by w.Code, p.Code, t.Code, d.ProcessDate
END
GO



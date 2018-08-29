USE [DLSM]
GO

/****** Object:  StoredProcedure [dbo].[sp_SearchCard]    Script Date: 29/08/2018 13:35:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER procedure [dbo].[sp_SearchCard]
	@serialBegin varchar(255)='0', 
	@serialEnd varchar(255)='0', 
	@BeginDate Datetime, 
	@EndDate Datetime 
as
begin
 DECLARE @card_product_id int
 SET @card_product_id=(select TOP 1 ID from product a join configure b on a.code=b.value and b.code='PRD-CARD')
	SELECT DISTINCT
       ss.ID as id
      ,ss.[WhID] as whid
      ,ss.[SerialBegin]
      ,ss.[SerialEnd]
      ,ss.[SerialCount]
	  ,d.DocDate
  FROM [dbo].[StockSerial]  as ss 
  LEFT OUTER JOIN  Document d on d.ID = ss.DocID  
  WHERE ss.PdID=@card_product_id
  AND (ss.SerialBegin like '%' + isnull(@serialBegin,'') +'%' or @serialBegin = '0' )
  AND (ss.SerialEnd like '%' + isnull(@serialEnd,'') + '%' or @serialEnd = '0')
  AND (d.DocDate >= @BeginDate or @BeginDate is null) 
  AND (d.DocDate <= @EndDate or @EndDate is null )
  AND ss.SerialCount>0 


end
GO


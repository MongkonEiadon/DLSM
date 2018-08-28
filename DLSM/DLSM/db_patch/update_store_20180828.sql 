USE [DLSM]
GO

/****** Object:  StoredProcedure [dbo].[sp_SearchCard]    Script Date: 28/08/2018 12:19:30 ******/
DROP PROCEDURE [dbo].[sp_SearchCard]
GO

/****** Object:  StoredProcedure [dbo].[sp_SearchCard]    Script Date: 28/08/2018 12:19:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[sp_SearchCard]
	@serialBegin varchar(255)='0', 
	@serialEnd varchar(255)='0', 
	@BeginDate Datetime, 
	@EndDate Datetime 
as
begin

	SELECT DISTINCT
       ss.ID as id
      ,ss.[WhID] as whid
      ,ss.[SerialBegin]
      ,ss.[SerialEnd]
      ,ss.[SerialCount]
	  ,d.DocDate
  FROM [dbo].[StockSerial]  as ss 
  LEFT OUTER JOIN  Document d on d.ID = ss.DocID
  INNER JOIN dbo.Product as p on ss.PdID=p.ID
  WHERE UPPER(p.[Name])='CARD'
  AND (ss.SerialBegin like '%' + isnull(@serialBegin,'') +'%' or @serialBegin = '0' )
  AND (ss.SerialEnd like '%' + isnull(@serialEnd,'') + '%' or @serialEnd = '0')
  AND (d.DocDate >= @BeginDate or @BeginDate is null) 
  AND (d.DocDate <= @EndDate or @EndDate is null )
  AND ss.SerialCount>0 


end
GO

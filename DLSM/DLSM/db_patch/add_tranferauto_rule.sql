INSERT INTO [dbo].[Module]
([Code]
,[Name]
,[ParentMenu]
,[NameTH])
VALUES
('AT','TransferAuto','Transactions','จ่ายอัตโนมัติ');

--for admin user
INSERT INTO [dbo].[Permission]
([UgID]
,[MdCode]
,[Active])
VALUES
(1   ,'AT' ,1)
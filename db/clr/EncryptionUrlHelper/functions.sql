-- Functions bound to [EncryptionUrlHelper]; same definitions as db/db.sql.
IF OBJECT_ID(N'dbo.EncryptUrl') IS NULL
    EXEC (N'CREATE FUNCTION [dbo].[EncryptUrl](@plainText [nvarchar](max))
RETURNS [nvarchar](max) WITH EXECUTE AS CALLER
AS EXTERNAL NAME [EncryptionUrlHelper].[DubaiInvestment.PMS.CLR.SQL.EncryptionHelper].[EncryptUrl]');
GO
IF OBJECT_ID(N'dbo.DecryptUrl') IS NULL
    EXEC (N'CREATE FUNCTION [dbo].[DecryptUrl](@plainText [nvarchar](max))
RETURNS [nvarchar](max) WITH EXECUTE AS CALLER
AS EXTERNAL NAME [EncryptionUrlHelper].[DubaiInvestment.PMS.CLR.SQL.EncryptionHelper].[DecryptUrl]');
GO

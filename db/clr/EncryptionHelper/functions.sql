-- Functions bound to [EncryptionHelper]; same definition as db/db.sql.
IF OBJECT_ID(N'dbo.EncryptUrlSafe') IS NULL
    EXEC (N'CREATE FUNCTION [dbo].[EncryptUrlSafe](@plainText [nvarchar](max))
RETURNS [nvarchar](max) WITH EXECUTE AS CALLER
AS EXTERNAL NAME [EncryptionHelper].[DubaiInvestment.PMS.SQL.CLR.EncryptionHelper].[EncryptUrlSafe]');
GO

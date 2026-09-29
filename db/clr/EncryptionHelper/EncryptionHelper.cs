using System.Data.SqlTypes;
using DubaiInvestment.PMS.Clr;
using Microsoft.SqlServer.Server;

// Namespace, class and method names match the original assembly, so db.sql's
// CREATE FUNCTION statements bind to this build unchanged.
namespace DubaiInvestment.PMS.SQL.CLR
{
    public static class EncryptionHelper
    {
        /// <summary>dbo.EncryptUrlSafe.</summary>
        [SqlFunction(IsDeterministic = true, IsPrecise = true)]
        public static SqlString EncryptUrlSafe(SqlString plainText)
        {
            return plainText.IsNull ? SqlString.Null : new SqlString(AesUrl.Encrypt(plainText.Value));
        }

        /// <summary>Present in the original assembly; not registered as a SQL function.</summary>
        public static string DecryptUrlSafe(string urlSafeBase64)
        {
            return AesUrl.Decrypt(urlSafeBase64);
        }
    }
}

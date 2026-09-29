using System.Data.SqlTypes;
using DubaiInvestment.PMS.Clr;
using Microsoft.SqlServer.Server;

// Namespace, class and method names match the original assembly, so db.sql's
// CREATE FUNCTION statements bind to this build unchanged.
namespace DubaiInvestment.PMS.CLR.SQL
{
    public static class EncryptionHelper
    {
        /// <summary>dbo.EncryptUrl. Deterministic: used by the persisted column MaintainEORegistrations.EncryptedRefNo.</summary>
        [SqlFunction(IsDeterministic = true, IsPrecise = true)]
        public static SqlString EncryptUrl(SqlString plainText)
        {
            return plainText.IsNull ? SqlString.Null : new SqlString(AesUrl.Encrypt(plainText.Value));
        }

        /// <summary>dbo.DecryptUrl.</summary>
        public static string DecryptUrl(string urlSafeBase64)
        {
            return AesUrl.Decrypt(urlSafeBase64);
        }
    }
}

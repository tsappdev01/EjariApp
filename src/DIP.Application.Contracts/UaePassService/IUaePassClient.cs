using DIP.EServices;
using StgDipService;
using System.Threading.Tasks;

namespace DIP.UaePassService
{
    public interface IUaePassClient
    {
        Task<string> GetAccessTokenAsync();
        Task<string> GetNOcDeclarationEnpoint();
        Task<SignProcessResponse> CreateSignProcessAsync(string base64PdfString, string UniqueString, string lang, string FileName, string _accessToken, EOGETCurrentLandlordSignatureDetailsResult CSDetails);
        Task<byte[]> FetchSignedDocumentAsync(string documentId, string _accessToken);

        Task<UaePassTokenResponse> GetAccessTokenByAuthorizationCodeAsync(string code, string rediractionToken);
        string GetuaepassloginRedirectionLink(string token);
        string GetuaepassLogoutUrlwithLoginRedirectionlink(string link);
        // New: retrieve user info using a bearer access token
        Task<UserInfoResponse> GetUserInfoAsync(string accessToken);
        string GetuaepasslogoutRedirectionLink(string token);
    }
}

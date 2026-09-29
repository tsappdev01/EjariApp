using System.Data.SqlTypes;
using System.Text.RegularExpressions;
using Xunit;
using AppHelper = global::EncryptionHelper;
using UrlHelper = DubaiInvestment.PMS.CLR.SQL.EncryptionHelper;
using SafeHelper = DubaiInvestment.PMS.SQL.CLR.EncryptionHelper;

public class EncryptionCompatibilityTests
{
    public static TheoryData<string> Inputs => new()
    {
        "",
        "x",
        "ABC123",
        "DIP-EJ-2026-000123",
        "ref/with+odd=chars & spaces",
        "عربي",
        new string('9', 1000),
    };

    [Theory, MemberData(nameof(Inputs))]
    public void EncryptUrl_matches_the_web_app(string input)
    {
        Assert.Equal(AppHelper.EncryptUrlSafe(input), UrlHelper.EncryptUrl(new SqlString(input)).Value);
    }

    [Theory, MemberData(nameof(Inputs))]
    public void EncryptUrlSafe_matches_EncryptUrl(string input)
    {
        Assert.Equal(UrlHelper.EncryptUrl(new SqlString(input)).Value, SafeHelper.EncryptUrlSafe(new SqlString(input)).Value);
    }

    [Theory, MemberData(nameof(Inputs))]
    public void Web_app_decrypts_what_the_database_encrypts(string input)
    {
        Assert.Equal(input, AppHelper.DecryptUrlSafe(UrlHelper.EncryptUrl(new SqlString(input)).Value));
    }

    [Theory, MemberData(nameof(Inputs))]
    public void DecryptUrl_reverses_EncryptUrl(string input)
    {
        var encrypted = UrlHelper.EncryptUrl(new SqlString(input)).Value;
        Assert.Equal(input, UrlHelper.DecryptUrl(encrypted));
        Assert.Equal(input, SafeHelper.DecryptUrlSafe(encrypted));
        Assert.Equal(input, UrlHelper.DecryptUrl(AppHelper.EncryptUrlSafe(input)));
    }

    [Theory, MemberData(nameof(Inputs))]
    public void Output_is_url_safe_and_unpadded(string input)
    {
        Assert.Matches(new Regex("^[A-Za-z0-9_-]+$"), UrlHelper.EncryptUrl(new SqlString(input)).Value);
    }

    [Fact]
    public void Is_deterministic_as_required_by_the_persisted_column()
    {
        Assert.Equal(UrlHelper.EncryptUrl(new SqlString("DIP-1")).Value, UrlHelper.EncryptUrl(new SqlString("DIP-1")).Value);
    }

    [Fact]
    public void Null_in_null_out()
    {
        Assert.True(UrlHelper.EncryptUrl(SqlString.Null).IsNull);
        Assert.True(SafeHelper.EncryptUrlSafe(SqlString.Null).IsNull);
    }
}

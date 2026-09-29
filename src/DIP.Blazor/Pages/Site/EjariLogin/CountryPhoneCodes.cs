
namespace DIP.Blazor.Pages.Site.EjariLogin
{
    public class CountryPhoneCodes
    {
    }

    public class CountryItem
    {
        public string Label { get; set; } =  string.Empty;
        public string Value { get; set; }= string.Empty;
        public string DialCode { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }

    public static class CountryList
    {
        public static readonly List<CountryItem> Countries =
        [
            new() { Label="United Arab Emirates", Value="AE", DialCode="+971", Icon="🇦🇪" },
            new() { Label="Afghanistan", Value="AF", DialCode="+93", Icon="🇦🇫" },
            new() { Label="Albania", Value="AL", DialCode="+355", Icon="🇦🇱" },
            new() { Label="Algeria", Value="DZ", DialCode="+213", Icon="🇩🇿" },
            new() { Label="Andorra", Value="AD", DialCode="+376", Icon="🇦🇩" },
            new() { Label="Angola", Value="AO", DialCode="+244", Icon="🇦🇴" },
            new() { Label="Argentina", Value="AR", DialCode="+54", Icon="🇦🇷" },
            new() { Label="Armenia", Value="AM", DialCode="+374", Icon="🇦🇲" },
            new() { Label="Australia", Value="AU", DialCode="+61", Icon="🇦🇺" },
            new() { Label="Austria", Value="AT", DialCode="+43", Icon="🇦🇹" },
            new() { Label="Azerbaijan", Value="AZ", DialCode="+994", Icon="🇦🇿" },

            new() { Label="Bahamas", Value="BS", DialCode="+1-242", Icon="🇧🇸" },
            new() { Label="Bahrain", Value="BH", DialCode="+973", Icon="🇧🇭" },
            new() { Label="Bangladesh", Value="BD", DialCode="+880", Icon="🇧🇩" },
            new() { Label="Belarus", Value="BY", DialCode="+375", Icon="🇧🇾" },
            new() { Label="Belgium", Value="BE", DialCode="+32", Icon="🇧🇪" },
            new() { Label="Belize", Value="BZ", DialCode="+501", Icon="🇧🇿" },
            new() { Label="Benin", Value="BJ", DialCode="+229", Icon="🇧🇯" },
            new() { Label="Bhutan", Value="BT", DialCode="+975", Icon="🇧🇹" },
            new() { Label="Bolivia", Value="BO", DialCode="+591", Icon="🇧🇴" },
            new() { Label="Bosnia and Herzegovina", Value="BA", DialCode="+387", Icon="🇧🇦" },
            new() { Label="Botswana", Value="BW", DialCode="+267", Icon="🇧🇼" },
            new() { Label="Brazil", Value="BR", DialCode="+55", Icon="🇧🇷" },
            new() { Label="Brunei", Value="BN", DialCode="+673", Icon="🇧🇳" },
            new() { Label="Bulgaria", Value="BG", DialCode="+359", Icon="🇧🇬" },

            new() { Label="Cambodia", Value="KH", DialCode="+855", Icon="🇰🇭" },
            new() { Label="Cameroon", Value="CM", DialCode="+237", Icon="🇨🇲" },
            new() { Label="Canada", Value="CA", DialCode="+1", Icon="🇨🇦" },
            new() { Label="Chile", Value="CL", DialCode="+56", Icon="🇨🇱" },
            new() { Label="China", Value="CN", DialCode="+86", Icon="🇨🇳" },
            new() { Label="Colombia", Value="CO", DialCode="+57", Icon="🇨🇴" },
            new() { Label="Costa Rica", Value="CR", DialCode="+506", Icon="🇨🇷" },
            new() { Label="Croatia", Value="HR", DialCode="+385", Icon="🇭🇷" },
            new() { Label="Cuba", Value="CU", DialCode="+53", Icon="🇨🇺" },
            new() { Label="Cyprus", Value="CY", DialCode="+357", Icon="🇨🇾" },
            new() { Label="Czech Republic", Value="CZ", DialCode="+420", Icon="🇨🇿" },

            new() { Label="Denmark", Value="DK", DialCode="+45", Icon="🇩🇰" },
            new() { Label="Dominican Republic", Value="DO", DialCode="+1-809", Icon="🇩🇴" },

            new() { Label="Ecuador", Value="EC", DialCode="+593", Icon="🇪🇨" },
            new() { Label="Egypt", Value="EG", DialCode="+20", Icon="🇪🇬" },
            new() { Label="El Salvador", Value="SV", DialCode="+503", Icon="🇸🇻" },
            new() { Label="Estonia", Value="EE", DialCode="+372", Icon="🇪🇪" },
            new() { Label="Ethiopia", Value="ET", DialCode="+251", Icon="🇪🇹" },

            new() { Label="Finland", Value="FI", DialCode="+358", Icon="🇫🇮" },
            new() { Label="France", Value="FR", DialCode="+33", Icon="🇫🇷" },

            new() { Label="Georgia", Value="GE", DialCode="+995", Icon="🇬🇪" },
            new() { Label="Germany", Value="DE", DialCode="+49", Icon="🇩🇪" },
            new() { Label="Ghana", Value="GH", DialCode="+233", Icon="🇬🇭" },
            new() { Label="Greece", Value="GR", DialCode="+30", Icon="🇬🇷" },

            new() { Label="Hong Kong", Value="HK", DialCode="+852", Icon="🇭🇰" },
            new() { Label="Hungary", Value="HU", DialCode="+36", Icon="🇭🇺" },

            new() { Label="Iceland", Value="IS", DialCode="+354", Icon="🇮🇸" },
            new() { Label="India", Value="IN", DialCode="+91", Icon="🇮🇳" },
            new() { Label="Indonesia", Value="ID", DialCode="+62", Icon="🇮🇩" },
            new() { Label="Iran", Value="IR", DialCode="+98", Icon="🇮🇷" },
            new() { Label="Iraq", Value="IQ", DialCode="+964", Icon="🇮🇶" },
            new() { Label="Ireland", Value="IE", DialCode="+353", Icon="🇮🇪" },
            new() { Label="Israel", Value="IL", DialCode="+972", Icon="🇮🇱" },
            new() { Label="Italy", Value="IT", DialCode="+39", Icon="🇮🇹" },

            new() { Label="Japan", Value="JP", DialCode="+81", Icon="🇯🇵" },
            new() { Label="Jordan", Value="JO", DialCode="+962", Icon="🇯🇴" },

            new() { Label="Kazakhstan", Value="KZ", DialCode="+7", Icon="🇰🇿" },
            new() { Label="Kenya", Value="KE", DialCode="+254", Icon="🇰🇪" },
            new() { Label="Kuwait", Value="KW", DialCode="+965", Icon="🇰🇼" },

            new() { Label="Latvia", Value="LV", DialCode="+371", Icon="🇱🇻" },
            new() { Label="Lebanon", Value="LB", DialCode="+961", Icon="🇱🇧" },
            new() { Label="Lithuania", Value="LT", DialCode="+370", Icon="🇱🇹" },
            new() { Label="Luxembourg", Value="LU", DialCode="+352", Icon="🇱🇺" },

            new() { Label="Malaysia", Value="MY", DialCode="+60", Icon="🇲🇾" },
            new() { Label="Mexico", Value="MX", DialCode="+52", Icon="🇲🇽" },
            new() { Label="Morocco", Value="MA", DialCode="+212", Icon="🇲🇦" },

            new() { Label="Netherlands", Value="NL", DialCode="+31", Icon="🇳🇱" },
            new() { Label="New Zealand", Value="NZ", DialCode="+64", Icon="🇳🇿" },
            new() { Label="Nigeria", Value="NG", DialCode="+234", Icon="🇳🇬" },
            new() { Label="Norway", Value="NO", DialCode="+47", Icon="🇳🇴" },

            new() { Label="Pakistan", Value="PK", DialCode="+92", Icon="🇵🇰" },
            new() { Label="Philippines", Value="PH", DialCode="+63", Icon="🇵🇭" },
            new() { Label="Poland", Value="PL", DialCode="+48", Icon="🇵🇱" },
            new() { Label="Portugal", Value="PT", DialCode="+351", Icon="🇵🇹" },

            new() { Label="Qatar", Value="QA", DialCode="+974", Icon="🇶🇦" },

            new() { Label="Romania", Value="RO", DialCode="+40", Icon="🇷🇴" },
            new() { Label="Russia", Value="RU", DialCode="+7", Icon="🇷🇺" },

            new() { Label="Saudi Arabia", Value="SA", DialCode="+966", Icon="🇸🇦" },
            new() { Label="Singapore", Value="SG", DialCode="+65", Icon="🇸🇬" },
            new() { Label="South Africa", Value="ZA", DialCode="+27", Icon="🇿🇦" },
            new() { Label="South Korea", Value="KR", DialCode="+82", Icon="🇰🇷" },
            new() { Label="Spain", Value="ES", DialCode="+34", Icon="🇪🇸" },
            new() { Label="Sweden", Value="SE", DialCode="+46", Icon="🇸🇪" },
            new() { Label="Switzerland", Value="CH", DialCode="+41", Icon="🇨🇭" },

            new() { Label="Thailand", Value="TH", DialCode="+66", Icon="🇹🇭" },
            new() { Label="Turkey", Value="TR", DialCode="+90", Icon="🇹🇷" },

            new() { Label="Ukraine", Value="UA", DialCode="+380", Icon="🇺🇦" },
            new() { Label="United Kingdom", Value="GB", DialCode="+44", Icon="🇬🇧" },
            new() { Label="United States", Value="US", DialCode="+1", Icon="🇺🇸" },

            new() { Label="Vietnam", Value="VN", DialCode="+84", Icon="🇻🇳" },
            new() { Label="Zimbabwe", Value="ZW", DialCode="+263", Icon="🇿🇼" }
        ];
    }
}

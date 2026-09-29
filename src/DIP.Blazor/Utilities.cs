using PdfSharpCore.Pdf.IO;
using QRCoder;
using StgDipService;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static StgDipService.NOCServiceClient;

namespace DIP.Blazor
{
    public class Utilities
    {
        NOCServiceClient _client;
        ClsCredentials _credentials;
        public Utilities(string appKey)
        {
            _client = new NOCServiceClient(EndpointConfiguration.BasicHttpsBinding_INOCService);
            _credentials = new ClsCredentials { AppKey = appKey };
        }

        static Random random = new Random();
        public char GetLetter()
        {
            // This method returns a random lowercase letter
            // ... Between 'a' and 'z' inclusize.
            int num = random.Next(0, 26); // Zero to 25
            char let = (char)('a' + num);
            return let;
        }

        public int GetNumber()
        {
            // This method returns a random lowercase letter
            // ... Between 'a' and 'z' inclusize.
            int num = random.Next(0, 100000000); // Zero to 25

            return num;
        }

        //public void Log(string message)
        //{
        //    File.AppendAllText(ConfigurationManager.AppSettings["LogPath"], message);
        //}

        public string MerchantId
        {
            get
            {
                return _client.GetMerchantIdAsync(_credentials).Result.GetMerchantIdResult;
            }
        }

        public string MerchantPassword
        {
            get
            {
                return _client.GetMerchantPasscodeAsync(_credentials).Result.GetMerchantPasscodeResult;
            }
        }

        public string HASHKEY
        {
            get
            {
                return _client.GetHashKeyAsync(_credentials).Result.GetHashKeyResult;
            }
        }

        public string PGSVersion
        {
            get
            {
                return _client.GetPGSVersionAsync(_credentials).Result.GetPGSVersionResult;
            }
        }

        public string PGSTranType
        {
            get
            {


                return _client.GetTransactionTypeAsync(_credentials).Result.GetTransactionTypeResult;
            }
        }

        public string CreateHMACSha256Str(string key, string message)
        {
            string hashHMACHex = HashHMACHex(key, message);
            return hashHMACHex.ToUpper();
        }


        private byte[] StringEncode(string text)
        {
            Encoding iso = Encoding.GetEncoding("ISO-8859-1");
            Encoding utf8 = Encoding.UTF8;
            byte[] utfBytes = utf8.GetBytes(text);
            byte[] isoBytes = Encoding.Convert(utf8, iso, utfBytes);
            return isoBytes;
        }

        private byte[] StringUTFEncode(string text)
        {
            //  var encoding = new UTF8Encoding();
            Encoding utf8 = Encoding.UTF8;
            return utf8.GetBytes(text);
        }


        private string HashEncode(byte[] hash)
        {
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }


        private string HashHMACHex(string keyHex, string message)
        {
            byte[] hash = HashHMAC(StringUTFEncode(keyHex), StringEncode(message));
            return HashEncode(hash);
        }

        private byte[] HashHMAC(byte[] key, byte[] message)
        {
            var hash = new HMACSHA256(key);
            return hash.ComputeHash(message);
        }

        public string PreparePOSTForm(string url, SortedList transactionData)
        {
            StringBuilder s = new StringBuilder();
            s.Append("<html>");
            s.AppendFormat("<body onload='document.forms[\"form\"].submit()'>");
            s.AppendFormat("<form name='form' action='{0}' method='post'>", url);
            foreach (DictionaryEntry item in transactionData)
            {
                s.AppendFormat("<input type='hidden' name='{0}' value='{1}' />", item.Key, item.Value);
            }
            s.Append("</form></body></html>");
            return s.ToString();
        }

    }

    #region VPCStringComparer

    class VPCStringComparer : IComparer
    {
        public int Compare(object a, object b)
        {
            /*
             <summary>Compare method using Ordinal comparison</summary>
             <param name="a">The first string in the comparison.</param>
             <param name="b">The second string in the comparison.</param>
             <returns>An int containing the result of the comparison.</returns>
             */

            // Return if we are comparing the same object or one of the 
            // objects is null, since we don't need to go any further.
            if (a == b) return 0;
            if (a == null) return -1;
            if (b == null) return 1;

            // Ensure we have string to compare
            string sa = a as string;
            string sb = b as string;

            // Get the CompareInfo object to use for comparing
            CompareInfo myComparer = CompareInfo.GetCompareInfo("en-US");
            if (sa != null && sb != null)
            {
                // Compare using an Ordinal Comparison.
                return myComparer.Compare(sa, sb, CompareOptions.Ordinal);
            }
            throw new ArgumentException("a and b should be strings.");
        }


    }
    #endregion


    #region DateValidators
    public static class DateValidators
    {
        public static DateTime? IsDateGreater(string firstDateString, DateTime secondDate, string DocumentCode)
        {
            if (string.IsNullOrWhiteSpace(firstDateString))
                return null;

            // Normalize common noise: trim, remove extra commas, remove ordinal suffixes like 1st/2nd/3rd/4th
            string normalized = NormalizeDateString(firstDateString);

            string[] globalformat = [
                // Day/Month/Year numeric
                "d/M/yyyy", "dd/MM/yyyy", "M/d/yyyy", "MM/dd/yyyy", "yyyy/M/d", "yyyy/MM/dd",
                "d-M-yyyy", "dd-MM-yyyy", "M-d-yyyy", "MM-dd-yyyy", "yyyy-M-d", "yyyy-MM-dd",
                "d.MM.yyyy", "dd.MM.yyyy", "M.MM.yyyy", "MM.dd.yyyy",

                // Day MonthName Year
                "d MMM yyyy", "dd MMM yyyy", "d MMMM yyyy", "dd MMMM yyyy",
                "MMM d, yyyy", "MMMM d, yyyy", "MMM dd, yyyy", "MMMM dd, yyyy",
                "yyyy MMM d", "yyyy MMMM d",

                // With times (24-hour)
                "d/M/yyyy H:mm", "dd/MM/yyyy H:mm", "d/M/yyyy HH:mm", "dd/MM/yyyy HH:mm",
                "d-M-yyyy H:mm", "dd-MM-yyyy HH:mm", "yyyy-MM-dd HH:mm",
                "d MMM yyyy H:mm", "dd MMM yyyy H:mm", "dd MMMM yyyy HH:mm",

                // With seconds
                "dd/MM/yyyy H:mm:ss", "dd-MM-yyyy H:mm:ss", "yyyy-MM-dd H:mm:ss",
                "d MMM yyyy H:mm:ss", "dd MMM yyyy H:mm:ss", "dd MMMM yyyy H:mm:ss",

                // 12-hour with AM/PM
                "dd/MM/yyyy h:mm tt", "MM/dd/yyyy h:mm tt", "d MMM yyyy h:mm tt", "dd MMM yyyy h:mm tt",
                "dd MMMM yyyy h:mm tt", "MMM d, yyyy h:mm tt", "MMMM d, yyyy h:mm tt",

                // ISO and variations
                "yyyyMMdd", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm:ssZ",
                "yyyy-MM-ddTHH:mm:ss.fff", "yyyy-MM-ddTHH:mm:ss.fffZ",
                "yyyy-MM-ddTHH:mm:ssK", "yyyy-MM-dd HH:mm:ssK",

                // RFC / weekday prefixed
                "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'", // RFC1123-like
                "ddd, dd MMM yyyy HH:mm:ss zzz",

                // Month name first
                "MMM dd yyyy", "MMMM dd yyyy", "MMM d yyyy", "MMMM d yyyy",
                "MMMM dd, yyyy", "MMM dd yyyy HH:mm", "MMMM dd yyyy HH:mm",
            
                // Short year variants
                "dd/MM/yy", "MM/dd/yy", "yy/MM/dd", "dd-MM-yy", "MM-dd-yy"
            ];
            string[] formats = [];

            //Try exact formats (comprehensive list)
            formats = DocumentCode switch
            {
                "DMSEID" => ["dd/MM/yyyy", "dd-MM-yyyy", "dd.MM.yyyy", "dd MM yyyy", "dd/MMM/yy", "dd-MMM-yy", "dd.MMM.yy", "dd MMM yy",],
                "DMSEPP" => ["dd/MM/yyyy", "dd-MM-yyyy", "dd.MM.yyyy", "dd MM yyyy"],
                "DMSTNCONT" => ["dd.MM.yyyy", "dd-MM-yyyy", "dd/MM/yyyy", "dd MM yyyy","dd MMMM yyyy", "dd-MMMM-yyyy", "dd/MMMM/yyyy", "dd MMMM yyyy","dd-MMM-yyyy", "dd/MMM/yyyy", "dd MMM yyyy", "dd-MMM-yyyy","dd'th' MMMM yyyy", "dd-th-MMMM-yyyy", "dd/th/MMMM/yyyy", "dd th MMMM yyyy","dd'nd' MMMM yyyy", "dd-nd-MMMM-yyyy", "dd/nd/MMMM/yyyy", "dd nd MMMM yyyy",
                                "dd'rd' MMMM yyyy", "dd-rd-MMMM-yyyy", "dd/rd/MMMM/yyyy", "dd rd MMMM yyyy","dd'st' MMMM yyyy", "dd-st-MMMM-yyyy", "dd/st/MMMM/yyyy", "dd st MMMM yyyy",
                                "yyyy dd MM","yyyy-dd-MM"],
                "DMSETD" => ["dd-MM-yyyy", "dd/MM/yyyy", "dd.MM.yyyy", "dd MM yyyy",],
                "DMSCTLIC" => [ "dd/MM/yyyy", "dd-MM-yyyy", "dd.MM.yyyy", "dd MM yyyy","dd/MMM/yyyy", "dd-MMM-yyyy", "dd.MMM.yyyy", "dd MMM yyyy","yyyy-MM-dd", "yyyy/MM/dd", "yyyy.MM.dd", "yyyy MM dd",
                                "dd/MMMMM/yyyy", "dd-MMMMM-yyyy", "dd.MMMMM.yyyy", "dd MMMMM yyyy",],
                "DMSEBNEC" => ["dd/MM/yyyy", "dd-MM-yyyy", "dd.MM.yyyy", "dd MM yyyy"],
                "DMSEIAC" => ["dd/MM/yyyy", "dd-MM-yyyy", "dd.MM.yyyy", "dd MM yyyy",],
                "DMSERV" => ["yyyy/MM/dd", "yyyy-MM-dd", "yyyy.MM.dd", "yyyy MM dd", "dd/MM/yyyy", "dd-MM-yyyy", "dd.MM.yyyy", "dd MM yyyy"],
                _ => globalformat
            };


            if (DateTime.TryParseExact(normalized, formats, CultureInfo.InvariantCulture,
                DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal | DateTimeStyles.AdjustToUniversal, out DateTime parsed))
            {
                return parsed;
            }
            // Try parse exact with the list
            if (DateTime.TryParseExact(normalized, globalformat, CultureInfo.InvariantCulture,
                DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal | DateTimeStyles.AdjustToUniversal, out parsed))
            {
                return parsed;
            }



            // Last resort: attempt parse after removing redundant characters (e.g. multiple spaces)
            string collapsed = Regex.Replace(normalized, @"\s{2,}", " ").Trim();
            if (DateTime.TryParse(collapsed, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal, out parsed))
            {
                return parsed;
            }

            // If all parsing attempts failed -> return false
            return null;
        }

        private static string NormalizeDateString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            string s = input.Trim();

            // Remove commas commonly used in "Nov 14, 2025" etc. Keep them removed because formats above handle both.
            s = s.Replace(",", " ");

            // Remove ordinal suffixes: 1st, 2nd, 3rd, 4th, ... up to "31st"
            s = Regex.Replace(s, @"\b(\d{1,2})(st|nd|rd|th)\b", "$1", RegexOptions.IgnoreCase);

            s = Regex.Replace(s, @"(\d+)(ST|ND|RD|TH)", "$1");

            // Normalize multiple spaces to single
            s = Regex.Replace(s, @"\s+", " ").Trim();

            return s;
        }
    }

    #endregion


    public static class DocumentIntelligentVAlidators
    {
        #region GET Valid UAE Numbers
        /// <summary>
        /// At least 9 digit positions (national mobile 5xxxxxxxx), or 10+ for 05xxxxxxxx / international, with + optional;
        /// ASCII/Unicode spaces (\p{Zs}), and . - / ' allowed between digits.
        /// Boundaries: not immediately preceded/followed by another digit (avoids swallowing adjacent numbers).
        /// Note: A single space between two full local numbers is still one "block"; <see cref="ProcessDigitRunForUaeMobiles" /> splits those.
        /// </summary>
        private static readonly Regex UaePhoneDigitBlockRegex = new(
            @"(?<![\d+])\+?(?:[\s\-\./'\p{Zs}]*\d){9,}(?!\d)",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>UAE local mobile: 05[0245689] + 7 digits (10 total).</summary>
        private static readonly Regex UaeLocalTenDigitInRunRegex = new(
            @"05[0245689]\d{7}", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>Already-normalized intl mobile: 9715 + 8 digits (12 total).</summary>
        private static readonly Regex UaeIntl12InRunRegex = new(
            @"9715\d{8}", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>National mobile without leading 0: 5[0245689] + 7 digits (9 total).</summary>
        private static readonly Regex UaeNationalNineInRunRegex = new(
            @"5[0245689]\d{7}", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>Maps Unicode spaces (NBSP from PDF, etc.) to ASCII space.</summary>
        private static string NormalizePhoneInputWhitespace(string input) =>
            Regex.Replace(input.Replace("\n", " ").Trim().TrimStart('\ufeff'), @"\p{Zs}+", " ", RegexOptions.CultureInvariant);

        private static void TryAddNormalizedUaeMobile(string digitsOnly, string[] localMobilePrefixes, List<string> validNumbers)
        {
            if (string.IsNullOrEmpty(digitsOnly))
                return;

            string number = NormalizeInternationalUaeDigitPrefix(digitsOnly);

            if (localMobilePrefixes.Any(p => number.StartsWith(p)))
                number = "971" + number.Substring(1);
            else if (number.StartsWith("5") && !number.StartsWith("971"))
                number = "971" + number;

            if (IsNormalizedUaeMobileNumber(number) && !validNumbers.Contains(number))
                validNumbers.Add(number);
        }

        /// <summary>
        /// One regex "block" may glue several numbers (e.g. "0529893884 0561147258" or "+971524085515 0554613849").
        /// Try the full run first; if that fails, scan for all embedded local, intl, and national patterns (no early exit between scans).
        /// </summary>
        private static void ProcessDigitRunForUaeMobiles(string digitsOnly, string[] localMobilePrefixes, List<string> validNumbers)
        {
            if (string.IsNullOrEmpty(digitsOnly))
                return;

            int before = validNumbers.Count;
            TryAddNormalizedUaeMobile(digitsOnly, localMobilePrefixes, validNumbers);
            if (validNumbers.Count > before)
                return;

            foreach (Match sm in UaeLocalTenDigitInRunRegex.Matches(digitsOnly))
                TryAddNormalizedUaeMobile(sm.Value, localMobilePrefixes, validNumbers);

            foreach (Match sm in UaeIntl12InRunRegex.Matches(digitsOnly))
                TryAddNormalizedUaeMobile(sm.Value, localMobilePrefixes, validNumbers);

            foreach (Match sm in UaeNationalNineInRunRegex.Matches(digitsOnly))
                TryAddNormalizedUaeMobile(sm.Value, localMobilePrefixes, validNumbers);
        }

        /// <summary>
        /// Normalizes international / OCR variants before prefix rules (00971, 971+trunk 0, 571 read as 971).
        /// </summary>
        private static string NormalizeInternationalUaeDigitPrefix(string digits)
        {
            if (digits.StartsWith("00971", StringComparison.Ordinal))
                digits = digits.Substring(2);

            // OCR / typo: +571… when the intent is +971… (local mobile still often shows 05… after the code).
            if (digits.Length > 4
                && digits.StartsWith("571", StringComparison.Ordinal)
                && digits[3] == '0')
                digits = "971" + digits.Substring(3);

            if (digits.Length > 3
                && digits.StartsWith("971", StringComparison.Ordinal)
                && digits[3] == '0')
                digits = "971" + digits.Substring(4);

            return digits;
        }

        /// <summary>
        /// After normalization, only UAE mobile (national operator digit 5) is accepted: 9715 + subscriber digits.
        /// Standard length is 12 (971 + 9-digit national mobile). Allows 11–13 digits for OCR noise.
        /// </summary>
        private static bool IsNormalizedUaeMobileNumber(string normalizedDigits)
        {
            if (normalizedDigits.Length is < 11 or > 13)
                return false;
            if (!normalizedDigits.StartsWith("9715", StringComparison.Ordinal))
                return false;
            for (int i = 4; i < normalizedDigits.Length; i++)
            {
                if (!char.IsDigit(normalizedDigits[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Extracts valid UAE mobile numbers from free text. Multiple numbers may appear (space, comma, words between blocks).
        /// Allowed input shape per block: at least 9 digit positions (e.g. 501234567) or longer local/international forms; optional + and separators space . - / ' between digits;
        /// local forms 050/052/054/055/056/058/059; international +971 / 00971 / 971 with optional trunk 0; OCR +571… as +971 when followed by 0.
        /// Returns comma-separated normalized values (9715…) or empty string.
        /// </summary>
        public static string GetAllValidUaeMobileNumbers(string input)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input))
                    return string.Empty;

                input = NormalizePhoneInputWhitespace(input);

                // Local prefixes that should be normalized to the +971 international format
                string[] localMobilePrefixes = { "050", "052", "054", "055", "056", "058", "059" };

                List<string> validNumbers = [];

                foreach (Match m in UaePhoneDigitBlockRegex.Matches(input))
                {
                    string digitsOnly = Regex.Replace(m.Value, @"\D", "");
                    if (string.IsNullOrEmpty(digitsOnly))
                        continue;

                    ProcessDigitRunForUaeMobiles(digitsOnly, localMobilePrefixes, validNumbers);
                }

                return validNumbers.Count > 0 ? string.Join(",", validNumbers) : string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
        #endregion

        #region Email Validator 
        public static string ExtractValidEmails(string input)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input))
                    return string.Empty;

                var emailRegex = new Regex(
                    @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*" +
                    @"@" +
                    @"(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z]{2,}",
                    RegexOptions.Compiled
                );

                var matches = emailRegex.Matches(input);
                List<string> emails = [];

                foreach (Match match in matches)
                {
                    string[] parts = match.Value.Split('@');
                    string cleanEmail = $"{parts[0]}@{parts[1].ToLower()}";
                    if (cleanEmail.StartsWith("_"))
                    {
                        cleanEmail = cleanEmail.Substring(1);
                    }
                    if (!emails.Contains(cleanEmail))
                        emails.Add(!cleanEmail.StartsWith("/") ? cleanEmail : cleanEmail.Substring(1));
                }

                return emails.Count > 0 ? string.Join(",", emails) : string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
        #endregion
    }

    public static class MaskingMethods
    {
        public static string MaskEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                return email;

            var parts = email.Split('@');
            string username = parts[0];
            string domain = parts[1];

            string maskedUser = username.Length > 4
                ? username.Substring(0, 2) + "**" + username.Substring(username.Length - 2, 2) :
                username.Length > 3 ?
                 username.Substring(0, 2) + "*" + username.Substring(username.Length - 1)
                : username + "**";


            var domainParts = domain.Split('.');
            string domainName = domainParts[0];
            string tld = string.Join(".", domainParts.Skip(1));

            string last3 = domainName.Length >= 3
                ? domainName.Substring(domainName.Length - 3)
                : domainName;

            int maskLength = domainName.Length - last3.Length;
            string maskedDomainPrefix = new string('*', maskLength);

            string finalDomain = maskedDomainPrefix + last3 + "." + tld;

            return maskedUser + "@" + finalDomain;
        }


        public static string MaskUaeNumber(string number)
        {
            number = number.Trim().Replace(" ", "");
            if (string.IsNullOrEmpty(number))
            {
                return "";
            }
            string formatted = number;

            // Convert 05XXXXXXXX to +9715XXXXXXXX
            if (number.StartsWith("05"))
            {
                formatted = string.Concat("+971", number.AsSpan(1));
            }
            if (number.StartsWith("05"))
            {
                formatted = string.Concat("+971", number.AsSpan(1));
            }

            // Already in +971 format
            if (!formatted.StartsWith("971") && !formatted.StartsWith("+971") && !formatted.StartsWith("91") && !formatted.StartsWith("+91"))
            {
                formatted = string.Concat("+971", number.AsSpan(1));
                //return "Invalid UAE number";
            }

            string last3 = formatted.Substring(formatted.Length - 3);

            return formatted.Substring(0, 3) + "******" + last3;
        }

    }


    public static class PdfSharpCoreUtilities
    {
        public static int GetPdfPageCount(byte[] FormFileContent)
        {
            using var stream = new MemoryStream(FormFileContent);
            var pdf = PdfReader.Open(stream, PdfDocumentOpenMode.ReadOnly);
            int pageCount = pdf.PageCount;
            return pageCount;
        }

        public static string GetMimeType(string fileName)
        {
            var ext = Path.GetExtension(fileName)?.ToLower();

            return ext switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".txt" => "text/plain",
                ".csv" => "text/csv",
                _ => "application/octet-stream" // default: unknown file
            };
        }

    }

    public static class QrCodeHelper
    {
        public static Task<byte[]> ConvertStringToQrImageAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Input text cannot be empty.", nameof(text));

            return Task.Run(() =>
            {
                var generator = new QRCodeGenerator();
                var data = generator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);

                var pngQr = new PngByteQRCode(data);
                byte[] pngBytes = pngQr.GetGraphic(20); // 20 = scale

                return pngBytes;
            });
        }

        public static async Task<string> ConvertStringToQrBase64Async(string text)
        {
            var bytes = await ConvertStringToQrImageAsync(text);
            return Convert.ToBase64String(bytes);
        }
    }
}



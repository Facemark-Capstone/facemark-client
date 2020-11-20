// David Wahid
namespace shared.Utilities.Encoding
{
    public static class Convert
    {
        public static byte[] FromUrlSafeBase64String(string str)
        {
            // TODO: exception handling
            return System.Convert.FromBase64String(toBase64(str));
        }

        public static byte[] FromBase64String(string str)
        {
            // TODO: exception handling
            return System.Convert.FromBase64String(str);
        }

        public static string ToUrlSafeBase64String(byte[] bytes)
        {
            return System.Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('/', '_')
                .Replace('+', '-'); ;
        }

        private static string toBase64(string str)
        {
            str = str.Replace('_', '/').Replace('-', '+');
            switch (str.Length % 4)
            {
                case 2: str += "=="; break;
                case 3: str += "="; break;
            }

            return str;
        }
    }
}

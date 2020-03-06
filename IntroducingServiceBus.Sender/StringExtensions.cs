namespace IntroducingServiceBus.Sender
{
    internal static class StringExtensions
    {
        public static string ToUrl(this string connectionString)
        {
            if (connectionString.ToLower().Contains("endpoint=sb://"))
            {
                string result = connectionString.Substring(connectionString.IndexOf('=') + 1);
                result = result.Substring(0, result.IndexOf(';'));
                return result;
            }
            else return string.Empty;
        }
    }
}

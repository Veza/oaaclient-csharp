using System.Globalization;

namespace Veza.OAA
{
    public static class Util
    {
        /// <summary>
        /// Convert a DateTime to an RFC3339 string
        /// </summary>
        /// <param name="date">The DateTime object to convert</param>
        /// <returns>A datetime string formatted for RFC3339</returns>
        public static string ToRFC3339(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Convert a DateTime to an RFC3339 string
        /// 
        /// Returns an empty string if invoked on a null object
        /// </summary>
        /// <param name="date">The DateTime object to convert</param>
        /// <returns>A datetime string formatted for RFC3339 or an empty string</returns>
        public static string ToRFC3339(this DateTime? date)
        {
            if (date != null)
            {
                DateTime dt = (DateTime)date;
                return dt.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture);
            }
            else { return string.Empty; }
        }

        /// <summary>
        /// Convert a DateTime to an RFC3339 string
        /// 
        /// Returns an empty string if `date` is null
        /// </summary>
        /// <param name="date">The DateTime object to convert</param>
        /// <returns>A datetime string formatted for RFC3339 or an empty string</returns>
        public static string DateTimeToRFC3339(DateTime? date)
        {
            if (date != null)
            {
                DateTime dt = (DateTime)date;
                return dt.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture);
            }
            else {  return string.Empty; }
        }

        /// <summary>
        /// Convert an RFC3339 datetime string to a DateTime object
        /// </summary>
        /// <param name="date">The RFC3339 datetime string to convert</param>
        /// <returns>A DateTime object</returns>
        public static DateTime FromRFC3339(this string date)
        {
            return DateTime.ParseExact(date, "yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
        }
    }
}

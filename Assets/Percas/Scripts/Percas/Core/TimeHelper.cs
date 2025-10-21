using System;
using System.Globalization;

namespace Percas
{
    public static class TimeHelper
    {
        /// <summary>
        /// Chuyển DateTime UTC thành chuỗi ISO 8601, lưu trữ an toàn.
        /// </summary>
        public static string ToIsoString(DateTime utcTime)
        {
            return utcTime.ToString("o", CultureInfo.InvariantCulture); // "o" = ISO 8601
        }

        /// <summary>
        /// Lấy thời gian UTC hiện tại và chuyển thành ISO 8601 string.
        /// </summary>
        public static string UtcNowToIsoString()
        {
            return DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Parse string ISO 8601 về DateTime UTC. Nếu sai format, return fallback hoặc DateTime.MinValue.
        /// </summary>
        public static DateTime ParseIsoString(string timeString, DateTime? fallbackUtc = null)
        {
            if (string.IsNullOrEmpty(timeString))
                return fallbackUtc ?? DateTime.MinValue;

            if (DateTime.TryParse(timeString, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var result))
                return result;

            return fallbackUtc ?? DateTime.MinValue;
        }

        /// <summary>
        /// Kiểm tra xem string có đúng định dạng ISO không.
        /// </summary>
        public static bool IsValidIsoString(string timeString)
        {
            return DateTime.TryParse(timeString, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out _);
        }
    }
}

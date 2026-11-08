using System;
using System.Text.RegularExpressions;

namespace RepairCenter.Helpers
{
    /// <summary>Static class: validation of the data entered for a device.</summary>
    public static class DeviceValidator
    {
        // Two capital letters, a dash, six digits. Example: RF-100001
        private static readonly Regex SerialPattern = new Regex(@"^[A-Z]{2}-\d{6}$", RegexOptions.Compiled);

        public static bool IsValidSerialNumber(string serialNumber)
        {
            return !string.IsNullOrWhiteSpace(serialNumber) && SerialPattern.IsMatch(serialNumber);
        }

        public static bool IsValidName(string text)
        {
            return !string.IsNullOrWhiteSpace(text) && text.Trim().Length >= 2;
        }

        /// <summary>The promised delivery date cannot be earlier than the receive date.</summary>
        public static bool IsValidDateRange(DateTime receivedDate, DateTime expectedDeliveryDate)
        {
            return expectedDeliveryDate.Date >= receivedDate.Date;
        }

        public static bool IsPositive(double value)
        {
            return value > 0;
        }
    }
}

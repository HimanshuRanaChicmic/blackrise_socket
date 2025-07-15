using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using BlackRise.Identity.Application.Resources;
using BlackRise.Identity.Application.Utils;

namespace BlackRise.Identity.Application.Helpers
{
    public static class LocalizationHelper
    {
        /// <summary>
        /// Returns the key name for a given constant value in Constants, or the value itself if not found.
        /// </summary>
        public static string GetConstantKey(string constantValue)
        {
            if (string.IsNullOrWhiteSpace(constantValue))
                return constantValue;

            var key = typeof(Constants)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
                .FirstOrDefault(fi => string.Equals((string?)fi.GetRawConstantValue(), constantValue, StringComparison.Ordinal))
                ?.Name;

            if (string.IsNullOrEmpty(key))
            {
                key = typeof(Constants)
                    .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
                    .FirstOrDefault(fi => string.Equals((string?)fi.GetRawConstantValue(), constantValue, StringComparison.OrdinalIgnoreCase))
                    ?.Name;
            }

            return key ?? constantValue;
        }

        /// <summary>
        /// Converts a constant string value to its key name, then fetches the localized message using that key.
        /// </summary>
        public static string GetLocalizedMessageFromConstantValue(string constantValue)
        {
            var key = GetConstantKey(constantValue);
            var localizedMessage = Messages.ResourceManager.GetString(key, CultureInfo.CurrentUICulture);
            return !string.IsNullOrEmpty(localizedMessage) ? localizedMessage : constantValue;
        }

        /// <summary>
        /// Returns the localized message for a given key and culture.
        /// </summary>
        public static string GetLocalizedMessageFromKey(string key, CultureInfo culture)
        {
            var value = Messages.ResourceManager.GetString(key, culture);
            return string.IsNullOrWhiteSpace(value) ? key : value;
        }
    }
}

namespace Microsoft.Telco.DigitalAssistant.Common
{
    using System;

    /// <summary>
    /// Helper class to ensure parameter correctness
    /// </summary>
    public static class Ensure
    {
        /// <summary>
        /// Ensure that the value is not null or empty
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <param name="parameterName">Name of the parameter</param>
        /// <param name="message">Message to throw in case that the value is null or empty</param>
        public static void EnsureIsNotNullOrEmpty(this string value, string parameterName, string message)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(parameterName, message);
            }
        }

        /// <summary>
        /// Ensure that the value is not null
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <param name="parameterName">Name of the parameter</param>
        /// <param name="message">Message to throw in case that the value is null</param>
        public static void EnsureIsNotNull(this object value, string parameterName, string message)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName, message);
            }
        }
    }
}

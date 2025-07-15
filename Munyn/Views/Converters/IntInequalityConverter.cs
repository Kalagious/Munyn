using Avalonia.Data.Converters;
using System;
using System.Globalization; // For CultureInfo

namespace Munyn.Converters // Adjust this namespace to match your project
{
    /// <summary>
    /// Converts an integer value to a boolean, indicating if it matches a target integer parameter.
    /// Returns true if the value equals the parameter, false otherwise.
    /// </summary>
    public class IntInequalityConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            int intValue;
            int intParameter;

            // --- Handle the 'value' input ---
            if (value is int tempIntValue) // If 'value' is already an int
            {
                intValue = tempIntValue;
            }
            else if (value != null) // If not int, but not null, try parsing from string
            {
                if (!int.TryParse(value.ToString(), out intValue))
                {
                    // Value is not a valid integer representation
                    return false;
                }
            }
            else // 'value' is null
            {
                return false;
            }

            // --- Handle the 'parameter' input ---
            if (parameter is int tempIntParameter) // If 'parameter' is already an int
            {
                intParameter = tempIntParameter;
            }
            else if (parameter != null) // If not int, but not null, try parsing from string
            {
                if (!int.TryParse(parameter.ToString(), out intParameter))
                {
                    // Parameter is not a valid integer representation
                    return false;
                }
            }
            else // 'parameter' is null
            {
                return false;
            }

            // Perform the comparison
            return intValue != intParameter;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // One-way conversion: Not typically needed for this converter
            throw new NotImplementedException("IntEqualityConverter is a one-way converter.");
        }
    }
}
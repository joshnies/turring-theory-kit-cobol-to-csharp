// Author: Joshua Nies
//
// Copyright © Turring 2021. All Rights Reserved.
//
using System;
using System.Text;

namespace TheoryKitCOBOL
{
    /// <summary>
    /// COBOL elementary item (variable) representation.
    /// </summary>
    public class COBOLVar
    {
        /// <summary>
        /// Value assigned to this COBOL variable.
        /// </summary>
        public dynamic value;

        /// <summary>
        /// Allocated (maximum) size of the value.
        /// Usage depends on the data type of "value".
        /// <para>If integer: maximum number of digits.</para>
        /// <para>
        /// If float: maximum number of digits + 1 for decimal point.
        /// </para>
        /// <para>If string: maxium string length.</para>
        /// <para>Unused in all other cases.</para>
        /// </summary>
        public int size;

        /// <summary>
        /// Allocated range this variable takes for its group.
        /// </summary>
        public int startIndex;

        /// <summary>
        /// Number of occurences when converted to a string.
        /// </summary>
        public int occurs;

        /// <summary>
        /// Variable used for the condition function.
        /// </summary>
        private COBOLVar conditionVar;

        /// <summary>
        /// Function ran for boolean variables to determine its value.
        /// </summary>
        private Func<COBOLVar, bool> conditionFunc;

        /// <summary>
        /// Whether this variable has a boolean value.
        /// </summary>
        public bool isBool
        {
            get
            {
                return conditionFunc != null;
            }
        }

        /// <summary>
        /// Whether this variable has a string value.
        /// </summary>
        public bool isString
        {
            get
            {
                return value is string;
            }
        }

        /// <summary>
        /// Whether this variable has an integer value.
        /// </summary>
        public bool isInt
        {
            get
            {
                return value is int;
            }
        }

        /// <summary>
        /// Whether this variable has a float value.
        /// </summary>
        public bool isFloat
        {
            get
            {
                return value is float;
            }
        }

        /// <summary>
        /// Create a new COBOL variable item.
        /// </summary>
        /// <param name="value">Initial/default value.</param>
        /// <param name="size">Allocated (maximum) size of the value.</param>
        /// <param name="occurs">
        /// Number of times this variable is repeated during output.
        /// </param>
        /// <param name="conditionVar">
        /// Variable referenced in the condition function.
        /// Only used for boolean variables.
        /// </param>
        /// <param name="conditionFunc">
        /// Function used to determine the value.
        /// Only used for boolean variables.
        /// </param>
        public COBOLVar(
            dynamic value,
            int size,
            int? occurs = null,
            COBOLVar conditionVar = null,
            Func<COBOLVar, bool> conditionFunc = null
        )
        {
            // Assign default value
            if (conditionVar != null && conditionFunc != null)
            {
                // If bool variable, get default value from condition function
                this.value = conditionFunc(conditionVar);
            }
            else
            {
                this.value = value;
            }

            this.size = size;
            this.occurs = occurs ?? 1;
            this.conditionVar = conditionVar;
            this.conditionFunc = conditionFunc;
        }

        /// <summary>
        /// Set value.
        /// </summary>
        /// <param name="newValue">New value.</param>
        /// <param name="fill">
        /// Whether to fill all variable items with the new value.
        /// </param>
        public void Set(dynamic newValue = null, bool fill = false)
        {
            // Set boolean value based on condition
            if (isBool)
            {
                try
                {
                    value = conditionFunc(conditionVar);
                    return;
                }
                catch
                {
                    Console.Error.WriteLine(
                        "[Theory Kit (COBOL)] ERROR: Failed to set COBOL " +
                        "variable to new condition-based bool value."
                    );
                }
            }
            // Fill alphanumeric or alphabetic values with new value based on
            // this variable's size
            else if (fill && value is string)
            {
                try
                {
                    value = new string(char.Parse(newValue), size);
                    return;
                }
                catch
                {
                    Console.Error.WriteLine(
                        "[Theory Kit (COBOL)] ERROR: Failed to set COBOL " +
                        "variable to filled value."
                    );
                }
            }
            else if (newValue is COBOLVar)
            {
                // Assign value of COBOL var
                value = newValue.value;
            }
            else
            {
                // Assign new value
                value = newValue;
            }
        }

        /// <summary>
        /// Add to the current value.
        /// <para>Only supports numeric types.</para>
        /// </summary>
        /// <param name="added">Value to add.</param>
        /// <returns>New value.</returns>
        public dynamic Add(dynamic added)
        {
            if (!COBOLUtils.IsNumeric(value) || !COBOLUtils.IsNumeric(added))
            {
                throw new Exception("Only numeric types can be added.");
            }

            value += added;
            return value;
        }

        /// <summary>
        /// Subtract from the current value.
        /// <para>Only supports numeric types.</para>
        /// </summary>
        /// <param name="subtracted">Value to subtract by.</param>
        /// <returns>New value.</returns>
        public dynamic Subtract(dynamic subtracted)
        {
            if (!COBOLUtils.IsNumeric(value) || !COBOLUtils.IsNumeric(subtracted))
            {
                throw new Exception("Only numeric types can be subtracted.");
            }

            value -= subtracted;
            return value;
        }

        /// <summary>
        /// Multiply the current value by a given number.
        /// <para>Only supports numeric types.</para>
        /// </summary>
        /// <param name="multiplier">Multiplier.</param>
        /// <returns>New value.</returns>
        public dynamic MultiplyBy(dynamic multiplier)
        {
            if (!COBOLUtils.IsNumeric(value) || !COBOLUtils.IsNumeric(multiplier))
            {
                throw new Exception("Only numeric types can be multiplied.");
            }

            value *= multiplier;
            return value;
        }

        /// <summary>
        /// Divide the current value by a given number.
        /// <para>Only supports numeric types.</para>
        /// </summary>
        /// <param name="divisor">Divisor.</param>
        /// <returns>New value.</returns>
        public dynamic DivideBy(dynamic divisor)
        {
            if (!COBOLUtils.IsNumeric(value) || !COBOLUtils.IsNumeric(divisor))
            {
                throw new Exception("Only numeric types can be divided.");
            }

            value /= divisor;
            return value;
        }

        /// <summary>
        /// Get whether the value has a numeric type.
        /// </summary>
        /// <returns>Whether the value has a numeric type.</returns>
        public dynamic IsNumeric()
        {
            return COBOLUtils.IsNumeric(value);
        }

        /// <summary>
        /// Get whether the value is an alphabetic string.
        /// </summary>
        /// <returns>Whether the value is an alphabetic string.</returns>
        public dynamic IsAlphabetic()
        {
            return COBOLUtils.IsAlphabetic(value);
        }

        /// <summary>
        /// Get whether the value is an all-uppercase alphabetic string.
        /// </summary>
        /// <returns>
        /// Whether the value is an all-uppercase alphabetic string.
        /// </returns>
        public dynamic IsAlphabeticUpper()
        {
            return COBOLUtils.IsAlphabeticUpper(value);
        }

        /// <summary>
        /// Get whether the value is an all-lowercase alphabetic string.
        /// </summary>
        /// <returns>
        /// Whether the value is an all-lowercase alphabetic string.
        /// </returns>
        public dynamic IsAlphabeticLower()
        {
            return COBOLUtils.IsAlphabeticLower(value);
        }

        /// <summary>
        /// Get whether the value is an alphanumeric string.
        /// </summary>
        /// <returns>Whether the value is an alphanumeric string.</returns>
        public dynamic IsAlphanumeric()
        {
            return COBOLUtils.IsAlphanumeric(value);
        }

        /// <summary>
        /// Get subvalue.
        /// </summary>
        /// <param name="start">
        /// Subvalue starting byte number (starts at 1).
        /// </param>
        /// <param name="length">Subvalue length.</param>
        /// <returns>Subvalue of same type.</returns>
        public dynamic GetSubvalue(int start, int? length)
        {
            if (isBool)
            {
                throw new Exception("Boolean variables cannot contain a subvalue.");
            }

            int startIndex = start - 1;

            if (isString)
            {
                if (length is int substrLength)
                {
                    return value.Substring(startIndex, substrLength);
                }
                else
                {
                    return value.Substring(startIndex);
                }
            }

            if (IsNumeric())
            {
                string valueStr = value.ToString();

                if (length is int substrLength)
                {
                    valueStr = valueStr.Substring(startIndex, substrLength);
                }
                else
                {
                    valueStr = valueStr.Substring(startIndex);
                }

                try
                {
                    if (isInt)
                    {
                        return int.Parse(valueStr);
                    }
                    else
                    {
                        return float.Parse(valueStr);
                    }
                }
                catch
                {
                    throw new Exception($"Failed to parse subvalue" +
                        " \"{valueStr}\" (start: {start} : length: {length}).");
                }
            }

            throw new Exception($"Subvalue not supported for type " +
                "\"{value.GetType()}\".");
        }

        /// <summary>
        /// Set subvalue.
        /// </summary>
        /// <param name="start">
        /// Subvalue starting byte number (starts at 1).
        /// </param>
        /// <param name="length">Subvalue length.</param>
        /// <param name="newValue">New value.</param>
        public void SetSubvalue(int start, int? length, dynamic newValue)
        {
            if (isBool)
            {
                throw new Exception("Boolean variables cannot contain a subvalue.");
            }

            int startIndex = start - 1;
            string newValueStr = newValue.ToString();

            if (isString)
            {
                var sb = new StringBuilder(value);

                if (length is int substrLength)
                {
                    sb.Remove(startIndex, substrLength);
                }
                else
                {
                    sb.Remove(startIndex, (value as string).Length - start);
                }

                sb.Insert(startIndex, newValueStr);
                value = sb.ToString();
                return;
            }

            if (IsNumeric())
            {
                string valueStr = value.ToString();
                var sb = new StringBuilder(valueStr);

                if (length is int substrLength)
                {
                    sb.Remove(startIndex, substrLength);
                }
                else
                {
                    sb.Remove(startIndex, (value as string).Length - start);
                }

                sb.Insert(startIndex, newValueStr);
                valueStr = sb.ToString();

                try
                {
                    if (isInt)
                    {
                        value = int.Parse(valueStr);
                        return;
                    }
                    else
                    {
                        value = float.Parse(valueStr);
                        return;
                    }
                }
                catch
                {
                    throw new Exception($"Failed to parse subvalue" +
                        " \"{valueStr}\" (start: {start} : length: {length}).");
                }
            }

            throw new Exception($"Subvalue not supported for type " +
                "\"{value.GetType()}\".");
        }

        public override string ToString()
        {
            if (value == null)
            {
                return "null";
            }
            else if (isBool)
            {
                // Return empty string for boolean items to omit for group item
                // strings.
                return "";
            }

            // Create string based on occurence count
            var baseVal = value.ToString();
            var result = baseVal;

            for (int i = 1; i < occurs; i++)
            {
                result += " " + baseVal;
            }

            return result;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(dynamic obj)
        {
            if (obj is COBOLGroup group) return value == group.value;

            if (obj is COBOLVar cvar) return value == cvar.value;

            return value == obj;
        }

        public static bool operator ==(COBOLVar cvar, dynamic val)
        {
            // Check null equality
            if (ReferenceEquals(cvar, null))
            {
                return ReferenceEquals(val, null);
            }

            // Check other equality
            return cvar.Equals(val);
        }

        public static bool operator !=(COBOLVar cvar, dynamic val)
        {
            // Check null equality
            if (ReferenceEquals(cvar, null))
            {
                return !ReferenceEquals(val, null);
            }

            // Check other equality
            return !cvar.Equals(val);
        }
    }
}

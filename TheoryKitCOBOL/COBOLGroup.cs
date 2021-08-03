// Author: Joshua Nies
//
// Copyright © Turring 2021. All Rights Reserved.
//
using System;
using System.Collections.Generic;
using System.Linq;

namespace TheoryKitCOBOL
{
    /// <summary>
    /// COBOL group item representation.
    /// </summary>
    public class COBOLGroup
    {
        /// <summary>
        /// COBOL variable items for this group.
        /// </summary>
        public List<dynamic> items;

        /// <summary>
        /// String value formed from this group's combined items.
        /// </summary>
        public string value
        {
            get
            {
                // Return appended item values as string
                string val = "";

                foreach (var item in items)
                {
                    if (item.value == null)
                    {
                        continue;
                    }

                    val += item.value.ToString();
                }

                return val;
            }
        }

        public int size
        {
            get
            {
                int newSize = 0;
                foreach (dynamic item in items)
                {
                    newSize += item.size;
                }
                return newSize;
            }
        }

        /// <summary>
        /// Create a new COBOL group item.
        /// </summary>
        /// <param name="items">COBOL variable items for this group.</param>
        public COBOLGroup(params dynamic[] items)
        {
            this.items = new List<dynamic>(items);

            // Set starting indices
            var nextStartIndex = 0;

            foreach (var item in this.items)
            {
                if (item is COBOLGroup) {
                    nextStartIndex = 0;
                    continue;
                }

                COBOLVar cvar = item;
                cvar.startIndex = nextStartIndex;
                nextStartIndex = cvar.startIndex + cvar.size;
            }
        }

        /// <summary>
        /// Set value for the entire COBOL group item.
        /// The value is spread among all variable items.
        /// </summary>
        /// <param name="value">New value.</param>
        /// <param name="fill">
        /// Whether to fill all variable items with the new value.
        /// </param>
        public void Set(dynamic value, bool fill = false)
        {
            var strVal = value.ToString();
            int lastIndex = 0;

            // Add padding for FILLERs
            if (size > strVal.Length)
            {
                string padding = new string(' ', Math.Max(size - strVal.Length, 0));
                strVal += padding;
            }

            foreach (var item in items)
            {
                if (lastIndex >= strVal.Length) return;

                // Evaluate item boolean value if applicable.
                if (item is COBOLVar && item.isBool) {
                    item.Set();
                    return;
                }

                // Get value substring based on item size
                int itemSize = item.size;
                string substr = strVal.Substring(lastIndex, itemSize);
                lastIndex += itemSize;
               
                // Assign item based on item value data type
                if (item.value is string)
                {
                    item.Set(substr, fill);
                }
                else if (item.value is int)
                {
                    // Parse new value to int, default to 0
                    int.TryParse(substr, out int val);
                    item.Set(val, fill);
                }
                else if (item.value is float)
                {
                    // Parse new value to float, default to 0
                    float.TryParse(substr, out float val);
                    item.Set(val, fill);
                }
            }
        }

        /// <summary>
        /// Get a COBOL variable item within this group by index.
        /// </summary>
        /// <param name="index">Index.</param>
        /// <returns>COBOL variable item.</returns>
        public COBOLVar GetItem(int index)
        {
            return items[index];
        }

        public override string ToString()
        {
            var strVals = items.Select(x => x.ToString()).ToList();
            return string.Join("", strVals);
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

        public static bool operator ==(COBOLGroup group, dynamic val)
        {
            // Check null equality
            if (ReferenceEquals(group, null))
            {
                return ReferenceEquals(val, null);
            }

            // Check other equality
            return group.Equals(val);
        }

        public static bool operator !=(COBOLGroup group, dynamic val)
        {
            // Check null equality
            if (ReferenceEquals(group, null))
            {
                return !ReferenceEquals(val, null);
            }

            // Check other equality
            return !group.Equals(val);
        }
    }
}

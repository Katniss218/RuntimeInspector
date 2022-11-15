using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeInspector.UI.FixedValuesDropdown
{
    /// <summary>
    /// A fixed list of values that can be submitted when the user clicks on them.
    /// </summary>
    public class FixedValuesDropdownWindow : MonoBehaviour
    {
        /// <summary>
        /// Set this to a function that returns the list of all valid values to display in the dropdown.
        /// </summary>
        public Func<List<object>> ValidValues { get; set; }

        public Type Type { get; set; }

        /// <summary>
        /// Creates a fixed values dropdown that returns values of a specific type and sets the method for getting the values.
        /// </summary>
        /// <param name="type">The type of the values that this dropdown will return.</param>
        /// <param name="valuesFunc"></param>
        public static void Create( Type type, Func<List<object>> valuesFunc )
        {

        }
    }
}
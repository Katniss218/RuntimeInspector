using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RuntimeEditor.UI.ValueSelection
{
    /// <summary>
    /// Stores a specific value for assignment.
    /// </summary>
    public sealed class ValueSelectionElement : MonoBehaviour, IPointerClickHandler
    {
        /// <summary>
        /// The value that this element holds.
        /// </summary>
        public object Value { get; internal set; }

        /// <summary>
        /// The window that this element is a part of.
        /// </summary>
        public ValueSelectionWindow Window { get; internal set; }

        public void SelectMe()
        {
            Window.Submit( this.Value );
        }

        public void OnPointerClick( PointerEventData e )
        {
            SelectMe();
        }
    }
}
using RuntimeInspector.Core;
using RuntimeInspector.UI.GUIUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeInspector.UI
{
    /// <summary>
    /// Used to match a binding to a specific UI element.
    /// </summary>
    public class UIObjectGraphBinding : MonoBehaviour
    {
        static List<UIObjectGraphBinding> uiBindings = new List<UIObjectGraphBinding>();

        /// <summary>
        /// Finds a UIBinding for a graphnode based on hierarchy and name.
        /// </summary>
        public static UIObjectGraphBinding Find( ObjectGraphNode node )
        {
            foreach( var uiBinding in uiBindings )
            {
                if( uiBinding.Node.Equals( node ) )
                {
                    return uiBinding;
                }
            }
            return null;
        }

        [field: SerializeField]
        public ObjectGraphNode Node
        {
            get;
            private set;
        }

        /// <summary>
        /// Sets the UI graph binding to point at the new graph node.
        /// </summary>
        public void UpdateGraphNode( ObjectGraphNode node )
        {
            Node = node;
            if( Node.CanRead )
            {
                CurrentValue = Node.GetValue();
            }
        }

#warning kinda ugly and requires it to be a string input field.
        // What we need:
        // - string input fields.
        // - object (reference and value type) input fields.

        // - null drawn as value - empty, no fields
        // - null drawn as reference - reference input field with nothing assigned.


        [field: SerializeField]
        public TMPro.TMP_InputField InputField { get; set; }

        /// <summary>
        /// Describes the root element for this drawn binding.
        /// </summary>
        /// <remarks>
        /// The root has a property of "if you delete it, it's equivalent to never drawing this binding in the first place".
        /// </remarks>
        [field: SerializeField]
        public RectTransform Root { get; set; }

        [field: SerializeField]
        public object CurrentValue { get; private set; }

        public bool IsEditing()
        {
            if( InputField != null && InputField.isFocused )
            {
                return true;
            }
            return false;
        }

        public void SetValueText( string userProvidedValue )
        {
            SetValue( userProvidedValue );
        }

        public void SetValue( object userProvidedValue )
        {
            if( InputConverterProvider.TryConvertForward( Node.Type, userProvidedValue, out object converted ) )
            {
                Node.SetValue( converted );

                if( !Node.CanRead )
                {
                    InputField.text = InspectorTextInputField.READONLY_PLACEHOLDER;
                }
            }
            else
            {

            }
        }

        void Awake()
        {
            uiBindings.Add( this );
        }

        void OnDestroy()
        {
            uiBindings.Remove( this );
        }
    }
}
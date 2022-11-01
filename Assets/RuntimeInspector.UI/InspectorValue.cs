using RuntimeInspector.Core;
using RuntimeInspector.UI.GUIUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeInspector.UI
{
    /// <summary>
    /// Binds a user input element to a field/property. Prevents the value shown from being updated if the user is inputting something.
    /// </summary>
    public class InspectorValue : MonoBehaviour
    {
        // TODO - in the future this should interlock with the Drawer system and prevent a drawer from redrawing if one of the fields is being edited.
        public MemberBinding Binding { get; set; }

        public TMPro.TMP_InputField InputField { get; set; } // kinda ugly and requires it to be an input field.

        //public RectTransform Parent { get; set; }
        //public RectTransform Root { get; set; }
        //public InspectorStyle Style { get; set; }

        public void UpdateValue( string value )
        {
            InputConverterProvider.AssignValue( Binding, value );
        }/*

        int timer = -10;

        void Update()
        {
            timer++;
            if( timer > 50 )
            {
                timer = 0;

                if( InputField.isFocused )
                {
                    return;
                }

                MemberBinding binding = Binding;
                RectTransform parent = Parent;
                RectTransform root = Root;
                InspectorStyle style = Style;

                Type type = Binding.Binding.GetInstanceType();
                Drawer drawer = DrawerProvider.GetDrawerOfType( type );

                Destroy( root.gameObject );

                RectTransform redrawnRoot = drawer.Draw( parent, binding, style );
            }
        }*/
    }
}
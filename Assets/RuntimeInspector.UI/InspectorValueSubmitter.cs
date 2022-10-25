using RuntimeInspector.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeInspector.UI
{
    /// <summary>
    /// Binds a user input element to a field/property. Prevents the value shown from being updated if the user is inputting something.
    /// </summary>
    public class InspectorValueSubmitter : MonoBehaviour
    {
        // TODO - in the future this should interlock with the Drawer system and prevent a drawer from redrawing if one of the fields is being edited.
        public MemberBinding Binding { get; set; }

        public TMPro.TMP_InputField InputField { get; set; } // kinda ugly and requires it to be an input field.

        public void UpdateValue( string value )
        {
            Drawer drawer = DrawerManager.GetDrawerOfType( Binding.Binding.GetInstanceType() );

            Binding.Binding.SetValue( drawer.InputToValueGeneral( value ) );
        }
    }
}
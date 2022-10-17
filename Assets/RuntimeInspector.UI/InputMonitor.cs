using RuntimeInspector.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeInspector.UI
{
    public class InputMonitor : MonoBehaviour
    {
        public IMemberBinding Binding { get; set; }

        public TMPro.TMP_InputField InputField { get; set; } // kinda ugly and requires it to be an input field.

        public void UpdateValue( string value )
        {
            IDrawer drawer = DrawerManager.GetDrawerOfType( Binding.Type );

            Binding.SetValue( drawer.InputToValueGeneral( value ) );
            // we need something to define how to set the value.
        }
    }
}
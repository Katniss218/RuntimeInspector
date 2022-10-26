using RuntimeInspector.Core;
using RuntimeInspector.UI.GUIUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeInspector.UI
{
    /// <summary>
    /// Used to inspect objects.
    /// </summary>
    public class Viewer : MonoBehaviour
    {
        [field: SerializeField]
        public RectTransform ViewerPanel { get; set; }

        [field: SerializeField]
        public Component DrawnObj { get; set; }

        public void Show( InspectorStyle style )
        {
            if( DrawnObj is null )
            {
                return;
            }

            Drawer drawer = DrawerProvider.GetDrawerOfType( DrawnObj.GetType() );

            MemberBinding binding = BindingUtils.GetBinding( () => DrawnObj, ( o ) => DrawnObj = (Component)o );

            drawer.Draw( ViewerPanel, binding, style );
        }

        void Start()
        {
            Show( InspectorStyle.Default );
        }

    }
}
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
        public Component DrawnObj { get; private set; }

        MemberBinding? binding = null;

        public void Show( InspectorStyle style )
        {
            if( DrawnObj is null )
            {
                return;
            }

            if( binding is null )
            {
                binding = BindingUtils.GetBinding( () => DrawnObj, ( o ) => DrawnObj = (Component)o );
            }
            if( !binding.Value.Binding.HasChangedValue(out _ ) )
            {
                return;
            }

            Drawer drawer = DrawerProvider.GetDrawerOfType( DrawnObj.GetType() );
            drawer.Draw( ViewerPanel, binding.Value, style );
        }

        void Start()
        {
            Show( InspectorStyle.Default );
        }
        

        int timer = -10;

        void Update()
        {
            timer++;
            if( timer > 50 )
            {
                timer = 0;

                Show( InspectorStyle.Default );
            }
        }
    }
}
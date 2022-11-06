using RuntimeInspector.Core;
using RuntimeInspector.UI.Drawers;
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

        public void Show( InspectorStyle style )
        {
            if( DrawnObj is null )
            {
                return;
            }

            string b = DrawnObj.ToString();

            ObjectGraphNode rootGraphNode = ObjectGraphNode.CreateGraph( () => DrawnObj, ( o ) => DrawnObj = (Component)o );

            Drawer drawer = DrawerProvider.GetDrawerOfType( rootGraphNode.GetInstanceType() );
            drawer.Draw( ViewerPanel, rootGraphNode, style );
        }

        void Start()
        {
            Show( InspectorStyle.Default );
        }


        int timer = 0;

        const int UPDATE_DELAY = 50;

        void Update()
        {
            timer++;

            if( timer > UPDATE_DELAY )
            {
                timer = 0;

                Show( InspectorStyle.Default );
            }
        }
    }
}
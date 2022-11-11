using RuntimeInspector.Core;
using RuntimeInspector.UI.Drawers;
using RuntimeInspector.UI.GUIUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RuntimeInspector.UI
{
    /// <summary>
    /// Used to inspect objects.
    /// </summary>
    public class Viewer : MonoBehaviour
    {
        /// <summary>
        /// Holds all of the graph node UIs associated with this viewer.
        /// </summary>
        public List<ObjectGraphNodeUI> GraphNodeUIs { get; } = new List<ObjectGraphNodeUI>();

        /// <summary>
        /// Finds a UIBinding for a graphnode based on hierarchy and name.
        /// </summary>
        public ObjectGraphNodeUI Find( ObjectGraphNode node )
        {
            foreach( var uiBinding in GraphNodeUIs )
            {
                if( uiBinding.GraphNode.Equals( node ) )
                {
                    return uiBinding;
                }
            }
            return null;
        }

        [field: SerializeField]
        public RectTransform ViewerPanel { get; set; }

        [field: SerializeField]
        public RectTransform MouseCanvas { get; set; }

        [field: SerializeField]
        public Component DrawnObj { get; private set; }

        public void Show( InspectorStyle style )
        {
            if( DrawnObj is null )
            {
                return;
            }

            ObjectGraphNode rootGraphNode = ObjectGraphNode.CreateGraph( null, () => DrawnObj, ( o ) => DrawnObj = (Component)o );

            Drawer drawer = DrawerProvider.GetDrawerOfType( rootGraphNode.GetInstanceType() );
            drawer.Draw( ViewerPanel, rootGraphNode, style );

            LayoutRebuilder.MarkLayoutForRebuild( ViewerPanel ); // Force layout to update to reflect the now potentially changed visuals.     TODO - kinda flicker'y.
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
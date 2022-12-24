using RuntimeEditor.Core;
using RuntimeEditor.UI.Inspector.Drawers;
using RuntimeEditor.UI.GUIUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RuntimeEditor.UI.Utils;

namespace RuntimeEditor.UI.Inspector
{
    /// <summary>
    /// Used to inspect objects.
    /// </summary>
    public class InspectorWindow : MonoBehaviour
    {
        /// <summary>
        /// Holds all of the graph node UIs associated with this viewer.
        /// </summary>
        public List<GraphNodeUI> GraphNodeUIs { get; } = new List<GraphNodeUI>();

        /// <summary>
        /// Finds a UIBinding for a graphnode based on hierarchy and name.
        /// </summary>
        public GraphNodeUI Find( ObjectGraphNode node )
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

        /// <summary>
        /// The parent object for the graph nodes.
        /// </summary>
        [field: SerializeField]
        public RectTransform ViewerPanel { get; set; }

        /// <summary>
        /// Gets the inspected object.
        /// </summary>
        [field: SerializeField]
        public Component DrawnObj { get; private set; }

        public object InspectedObject { get; private set; }

        public void InspectObject( object obj )
        {
            this.InspectedObject = obj;

            this.RedrawInspectedObject( InspectorStyle.Default );
        }

        public void RedrawInspectedObject( InspectorStyle style )
        {
            if( InspectedObject.IsUnityNull() )
            {
                return;
            }

            ObjectGraphNode rootGraphNode = ObjectGraphNode.CreateGraph( null, () => InspectedObject, ( o ) => InspectedObject = (Component)o );

            Drawer drawer = DrawerProvider.GetDrawerOfType( rootGraphNode.GetInstanceType() );
            drawer.Draw( ViewerPanel, rootGraphNode, style );

#warning TODO - Layout update is kinda flicker'y and takes a long time with nested objects.
            LayoutRebuilder.ForceRebuildLayoutImmediate( ViewerPanel );
        }

        void Start()
        {
            InspectObject( DrawnObj );
        }

        int timer = 0;
        const int UPDATE_DELAY = 50;

        void Update()
        {
            timer++;

            if( timer > UPDATE_DELAY )
            {
                timer = 0;

                RedrawInspectedObject( InspectorStyle.Default );
            }
        }
    }
}
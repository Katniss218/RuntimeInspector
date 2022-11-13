using RuntimeInspector.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RuntimeInspector.UI.Inspector
{
    /// <summary>
    /// Used when dragging values from one object input field into another.
    /// </summary>
    public class GraphNodeUIDrag : MonoBehaviour
    {
        public static GraphNodeUIDrag CurrentlyDragged { get; private set; }

        public ObjectGraphNodeUI Node { get; set; }

        public void Awake()
        {
            if( CurrentlyDragged != null )
            {
                throw new Exception( "Tried to create a GraphNodeUIDrag when another one already exists" );
            }
        }

        public static void StartDragging( ObjectGraphNodeUI sourceNodeUI )
        {
            if( CurrentlyDragged != null )
            {
                throw new Exception( "Tried to begin drag when something is already being dragged." );
            }

            GameObject go = new GameObject( "Drag" );

            GraphNodeUIDrag drag = go.AddComponent<GraphNodeUIDrag>();
            drag.Node = sourceNodeUI;

            CurrentlyDragged = drag;

#warning TODO - remove this delegate on end dragging because it throws errors later??.
            sourceNodeUI.onDestroy += () => Destroy(drag.gameObject);
        }

        public static void EndDragging( ObjectGraphNodeUI targetNodeUI )
        {
            if( CurrentlyDragged == null )
            {
                throw new Exception( "Tried to end drag when nothing was being dragged." );
            }

            targetNodeUI.SetValue( CurrentlyDragged.Node.GraphNode.GetInstanceType(), CurrentlyDragged.Node.CurrentValue );

            Destroy( CurrentlyDragged.gameObject );
            CurrentlyDragged = null;
        }
    }
}
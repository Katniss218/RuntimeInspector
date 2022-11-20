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
    public class GraphNodeDrag : MonoBehaviour
    {
        public static GraphNodeDrag CurrentlyDragged { get; private set; }

        public GraphNodeUI Node { get; set; }

        public void Awake()
        {
            if( CurrentlyDragged != null )
            {
                throw new Exception( "Tried to create a GraphNodeUIDrag when another one already exists" );
            }
        }

        public static void StartDragging( GraphNodeUI sourceNodeUI )
        {
            if( CurrentlyDragged != null )
            {
                throw new Exception( "Tried to begin drag when something is already being dragged." );
            }

            GameObject go = new GameObject( "Drag" );

            GraphNodeDrag drag = go.AddComponent<GraphNodeDrag>();
            drag.Node = sourceNodeUI;

            CurrentlyDragged = drag;

#warning TODO - remove this delegate on end dragging because it throws errors later??.
            sourceNodeUI.onValueChanged += () => Destroy(drag.gameObject);
        }

        public static void EndDragging( GraphNodeUI targetNodeUI )
        {
            if( CurrentlyDragged == null )
            {
                throw new Exception( "Tried to end drag when nothing was being dragged." );
            }

            targetNodeUI.SetValue( CurrentlyDragged.Node );

            Destroy( CurrentlyDragged.gameObject );
            CurrentlyDragged = null;
        }
    }
}
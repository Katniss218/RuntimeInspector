using RuntimeInspector.Core;
using RuntimeInspector.UI.Attributes;
using RuntimeInspector.UI.Drawers;
using RuntimeInspector.UI.GUIUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace RuntimeInspector.UI
{
    public abstract class Drawer
    {
        private struct RedrawDataInternal
        {
            public bool DestroyOld { get; set; }
            public bool CreateNew { get; set; }
            public ObjectGraphNodeUI GraphUI { get; set; }

            public bool Hidden { get; set; }

            public RedrawDataInternal( bool destroyOld, bool createNew, ObjectGraphNodeUI binding, bool hidden )
            {
                this.DestroyOld = destroyOld;
                this.CreateNew = createNew;
                this.GraphUI = binding;
                this.Hidden = hidden;
            }

            /// <summary>
            /// Calculates what should be done with a given binding. Whether to draw it all over, just update, or do nothing.
            /// </summary>
            public static RedrawDataInternal GetRedrawData( ObjectGraphNode node )
            {
                if( node.GetAttributes<HideAttribute>().FirstOrDefault() != null )
                {
                    return new RedrawDataInternal( false, false, null, true );
                }

                bool destroyOld = false;
                bool createNew = false;

                ObjectGraphNodeUI drawnBinding = ObjectGraphNodeUI.Find( node );

                // we should redraw if the value changed, or if the value isn't drawn at all.
                // we should remove the previous value if it changed, and is drawn.

                bool isDisplayedValueStale = false;
                if( drawnBinding != null )
                {
                    if( node.CanRead )
                    {
                        object displayedValue = drawnBinding.CurrentValue;
                        object newValue = node.GetValue();
                        isDisplayedValueStale = !displayedValue?.Equals( newValue ) ?? newValue != null;
                    }
                    else
                    {
                        isDisplayedValueStale = false;
                    }

                    if( isDisplayedValueStale )
                    {
                        if( drawnBinding.Root == null )
                        {
                            Debug.LogWarning( $"UIBinding.Root for Binding '{node.Name}' was null." );
                        }

                        destroyOld = true;
                    }
                }

                if( drawnBinding == null || isDisplayedValueStale )
                {
                    createNew = true;
                }

                return new RedrawDataInternal( destroyOld, createNew, drawnBinding, false );
            }
        }

        protected struct RedrawData
        {
            /// <summary>
            /// Whether or not the node should be drawn.
            /// </summary>
            /// <remarks>
            /// This can be set to false, in such case, make sure to call Draw on the child nodes.
            /// </remarks>
            public bool CreateNew { get; set; }

            /// <summary>
            /// The UI component representing the value of this graph node.
            /// </summary>
            public ObjectGraphNodeUI ObjectGraphNodeUI { get; set; }
        }

        /// <summary>
        /// Draws a graph node using the drawer.
        /// </summary>
        /// <param name="parent">The root of the graph node will be drawn as a child of this object.</param>
        /// <param name="binding">The graph node to draw.</param>
        public ObjectGraphNodeUI Draw( RectTransform parent, ObjectGraphNode binding, InspectorStyle style )
        {
            RedrawDataInternal redrawData = RedrawDataInternal.GetRedrawData( binding );
            if( redrawData.Hidden ) // this for some reason prevents the null list and whatnot. (week later --idk what that means anymore kek)
            {
                return null;
            }
            if( redrawData.GraphUI == null )
            {
                (_, redrawData.GraphUI) = UINode.Create( parent, binding, style );
            }

            if( redrawData.DestroyOld )
            {
                Transform root = redrawData.GraphUI.Root;
                for( int i = 0; i < root.childCount; i++ )
                {
                    Object.Destroy( root.GetChild( i ).gameObject );
                }
            }

            redrawData.GraphUI.UpdateGraphNode( binding ); // point at the updated node and cache the new value.

            RedrawData redrawDataActual = new RedrawData()
            {
                CreateNew = redrawData.CreateNew,
                ObjectGraphNodeUI = redrawData.GraphUI
            };

            DrawInternal( redrawDataActual, binding, style );

            return redrawData.GraphUI;
        }

        /// <summary>
        /// Override this method in a derived drawer to implement the drawing functionality.
        /// </summary>
        protected abstract void DrawInternal( RedrawData redrawData, ObjectGraphNode binding, InspectorStyle style );
    }
}
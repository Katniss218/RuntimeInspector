﻿using RuntimeEditor.Core;
using RuntimeEditor.UI.Inspector.Attributes;
using RuntimeEditor.UI.GUIUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RuntimeEditor.UI.Inspector
{
    public abstract class Drawer
    {
        private class RedrawDataPrivate
        {
            public bool DestroyOld { get; private set; }
            public bool CreateNew { get; private set; }
            public GraphNodeUI GraphUI { get; set; }

            public bool Hidden { get; private set; }

            public RedrawDataPrivate( bool destroyOld, bool createNew, GraphNodeUI graphNodeUI, bool hidden )
            {
                this.DestroyOld = destroyOld;
                this.CreateNew = createNew;
                this.GraphUI = graphNodeUI;
                this.Hidden = hidden;
            }

            /// <summary>
            /// Calculates what should be done with a given binding. Whether to draw it all over, just update, or do nothing.
            /// </summary>
            public static RedrawDataPrivate GetRedrawData( InspectorWindow viewer, ObjectGraphNode graphNode )
            {
                if( graphNode.GetAttributes<HideAttribute>().FirstOrDefault() != null )
                {
                    return new RedrawDataPrivate( false, false, null, true );
                }

                bool destroyOld = false;
                bool createNew = false;

                GraphNodeUI drawnBinding = viewer.Find( graphNode );

                // we should redraw if the value changed, or if the value isn't drawn at all.
                // we should remove the previous value if it changed, and is drawn.

                bool isDisplayedValueStale = false;
                if( drawnBinding != null )
                {
                    if( graphNode.CanRead )
                    {
                        object displayedValue = drawnBinding.CurrentValue;
                        object newValue = graphNode.GetValue();
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
                            Debug.LogWarning( $"UIBinding.Root for Binding '{graphNode.Name}' was null." );
                        }

                        destroyOld = true;
                    }
                }

                if( drawnBinding == null || isDisplayedValueStale )
                {
                    createNew = true;
                }

                return new RedrawDataPrivate( destroyOld, createNew, drawnBinding, false );
            }
        }

        protected struct RedrawDataInternal
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
            /// <remarks>
            /// Use <see cref="GraphNodeUI.Root"/> to get the root UI object matching this particular graph node (guaranteed to not be null inside <see cref="DrawInternal(RedrawDataInternal, ObjectGraphNode, InspectorStyle)"/> method.
            /// </remarks>
            public GraphNodeUI ObjectGraphNodeUI { get; set; }
        }

        /// <summary>
        /// Draws a graph node using the drawer.
        /// </summary>
        /// <param name="parent">The root of the graph node will be drawn as a child of this object.</param>
        /// <param name="graphNode">The graph node to draw.</param>
        public GraphNodeUI Draw( RectTransform parent, ObjectGraphNode graphNode, InspectorStyle style )
        {
            if( parent == null )
            {
                throw new ArgumentNullException( nameof( parent ), "Parent transform must be set to a child of a valid viewer rect transform." );
            }
            if( graphNode == null )
            {
                throw new ArgumentNullException( nameof( graphNode ), "Graph node must be set." );
            }
            if( style == null )
            {
                throw new ArgumentNullException( nameof( style ), "Style must be set." );
            }
            InspectorWindow viewer = parent.GetComponentInParent<InspectorWindow>( false );
            if( viewer == null )
            {
                throw new InvalidOperationException( $"The parent object for drawing must be a child of an object with a Viewer component." );
            }

            RedrawDataPrivate redrawData = RedrawDataPrivate.GetRedrawData( viewer, graphNode );
            if( redrawData.Hidden ) // this for some reason prevents the null list and whatnot. (week later --idk what that means anymore kek)
            {
                return null;
            }
            if( redrawData.GraphUI == null )
            {
                (_, redrawData.GraphUI) = InspectorGraphNodeUI.Create( parent, graphNode, style );
            }

            if( redrawData.DestroyOld )
            {
                Transform root = redrawData.GraphUI.Root;
                for( int i = 0; i < root.childCount; i++ )
                {
                    Object.Destroy( root.GetChild( i ).gameObject );
                }
            }

            redrawData.GraphUI.SetGraphNode( graphNode ); // point at the updated node and cache the new value.

            RedrawDataInternal redrawDataActual = new RedrawDataInternal()
            {
                CreateNew = redrawData.CreateNew,
                ObjectGraphNodeUI = redrawData.GraphUI
            };

            DrawInternal( redrawDataActual, graphNode, style );

            return redrawData.GraphUI;
        }

        /// <summary>
        /// Override this method in a derived drawer to implement the drawing functionality.
        /// </summary>
        /// <remarks>
        /// This method must handle drawing a fresh given member, and updating the child members, if the given member doesn't need to be updated.
        /// </remarks>
        protected abstract void DrawInternal( RedrawDataInternal info, ObjectGraphNode graphNode, InspectorStyle style );
    }
}
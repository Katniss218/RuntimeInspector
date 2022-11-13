using RuntimeInspector.Core;
using RuntimeInspector.Core.AssetManagement;
using RuntimeInspector.UI.GUIUtils;
using System;
using System.Linq;
using UnityEngine;

namespace RuntimeInspector.UI.Inspector.Drawers
{
    /// <summary>
    /// Draws instances of objects (not references).
    /// </summary>
    [DrawerOf( typeof( Array ) )]
    public sealed class ArrayDrawer : Drawer
    {
        protected override void DrawInternal( RedrawDataInternal redrawData, ObjectGraphNode node, InspectorStyle style )
        {
            bool isNull = false;
            Array value = null;
            if( node.CanRead )
            {
                value = (Array)node.GetValue();
            }

            RectTransform list = null;
            RectTransform group;
            if( redrawData.CreateNew )
            {
                group = InspectorVerticalList.Create( "group", redrawData.ObjectGraphNodeUI.Root, style, new InspectorVerticalList.Params() { IncludeMargin = false } );

                RectTransform label = InspectorLabel.Create( group, AssetRegistry<Sprite>.GetAsset( "RuntimeInspector/Sprites/icon_object" ), $"{node.Name} >", style );

                if( node.CanRead && !isNull )
                {
                    list = InspectorVerticalList.Create( "list", group, style, new InspectorVerticalList.Params() { IncludeMargin = true } );
                }
            }
            else
            {
                if( node.CanRead && !isNull )
                {
                    group = InspectorVerticalList.Find( "group", redrawData.ObjectGraphNodeUI?.Root );

                    list = InspectorVerticalList.Find( "list", group );
                }
            }

            // We have to ALWAYS ping the child objects, or their displayed values will get stale when their parent is not updated.

            var children = node.Children;
            if( node.CanRead && !isNull )
            {
#warning TODO - Drawing array elements is harder than I thought. This will fail with multiple arrays of the same name.
                // dynamically (or statically) adding children to the graph node could work.

                //ObjectGraphNode indexer = children.FirstOrDefault( n => n is ObjectGraphNodeProperty p && p.IndexParameters != null && p.IndexParameters.SequenceEqual( new[] { typeof( int ) } ) );

                for( int i = 0; i < value.Length; i++ )
                {
                    int index = i;
                    ObjectGraphNode graphNode = ObjectGraphNode.CreateGraph( $"{node.Name}[{i}]", () => value.GetValue( index ), ( o ) => value.SetValue( o, index ) );

                    Drawer drawer = DrawerProvider.GetDrawerOfType( graphNode.GetInstanceType() );
                    drawer.Draw( list, graphNode, style );
                }
            }
        }
    }
}
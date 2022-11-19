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
        protected override void DrawInternal( RedrawDataInternal redrawData, ObjectGraphNode graphNode, InspectorStyle style )
        {
            bool isNull = false;
            RectTransform group;
            RectTransform list = null;
            if( redrawData.CreateNew )
            {
                group = InspectorVerticalList.Create( "group", redrawData.ObjectGraphNodeUI.Root, style, new InspectorVerticalList.Params() { IncludeMargin = false } );

                RectTransform label = InspectorLabel.Create( group, AssetRegistry<Sprite>.GetAsset( "RuntimeInspector/Sprites/icon_object" ), $"{graphNode.GetDisplayName()} >", style );

                if( graphNode.CanRead && !isNull )
                {
                    list = InspectorVerticalList.Create( "list", group, style, new InspectorVerticalList.Params() { IncludeMargin = true } );
                }
            }
            else
            {
                if( graphNode.CanRead && !isNull )
                {
                    group = InspectorVerticalList.Find( "group", redrawData.ObjectGraphNodeUI?.Root );

                    list = InspectorVerticalList.Find( "list", group );
                }
            }

            // We have to ALWAYS ping the child objects, or their displayed values will get stale when their parent is not updated.

            Array value = null;
            if( graphNode.CanRead )
            {
                value = (Array)graphNode.GetValue();
            }

            var children = graphNode.GetChildren();
            if( graphNode.CanRead && !isNull )
            {
                for( int i = 0; i < value.Length; i++ )
                {
                    int index = i;

                    ObjectGraphNode elementNode = ObjectGraphNode.CreateNode( graphNode, $"[{i}]", null, () => value.GetValue( index ), ( o ) => value.SetValue( o, index ) );

                    Drawer drawer = DrawerProvider.GetDrawerOfType( elementNode.GetInstanceType() );
                    drawer.Draw( list, elementNode, style );
                }
            }
        }
    }
}
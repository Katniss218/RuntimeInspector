using RuntimeInspector.Core;
using RuntimeInspector.Core.AssetManagement;
using RuntimeInspector.UI.GUIUtils;
using RuntimeInspector.UI.Inspector.Attributes;
using System;
using System.Linq;
using UnityEngine;

namespace RuntimeInspector.UI.Inspector.Drawers
{
    /// <summary>
    /// Draws instances of objects (not references).
    /// </summary>
    [DrawerOf( typeof( Vector3 ) )]
    public sealed class Vector3Drawer : Drawer
    {
        protected override void DrawInternal( RedrawDataInternal redrawData, ObjectGraphNode graphNode, InspectorStyle style )
        {
            bool isNull = false;
            if( graphNode.CanRead )
            {
                isNull = IsUnityNull( graphNode.GetValue() );
            }

            RectTransform list = null;
            RectTransform group;
            if( redrawData.CreateNew )
            {
                group = InspectorVerticalList.Create( "group", redrawData.ObjectGraphNodeUI.Root, style, new InspectorVerticalList.Params() { IncludeMargin = false } );

                RectTransform label = InspectorLabel.Create( group, AssetRegistry<Sprite>.GetAsset( "RuntimeInspector/Sprites/icon_object" ), $"{graphNode.Name} >", style );

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

            if( graphNode.CanRead && !isNull )
            {
#warning TODO - this *works* but is kinda ugly.

                // method to get child by some predicate.


                ObjectGraphNode nXs = graphNode.GetChildren().FirstOrDefault( n => n.Name == "x" );
                ObjectGraphNode nYs = graphNode.GetChildren().FirstOrDefault( n => n.Name == "y" );
                ObjectGraphNode nZs = graphNode.GetChildren().FirstOrDefault( n => n.Name == "z" );

                ObjectGraphNode nx = ObjectGraphNode.CreateGraph( $"x", nXs.GetValue, ( o ) => graphNode.SetValue( new Vector3(
                   (float)o,
                   (float)nYs.GetValue(),
                   (float)nZs.GetValue()
                   ) ) );
                nx.SetParent( graphNode );

                ObjectGraphNode ny = ObjectGraphNode.CreateGraph( $"y", nYs.GetValue, ( o ) => graphNode.SetValue( new Vector3(
                   (float)nXs.GetValue(),
                   (float)o,
                   (float)nZs.GetValue()
                   ) ) );
                ny.SetParent( graphNode );

                ObjectGraphNode nz = ObjectGraphNode.CreateGraph( $"z", nZs.GetValue, ( o ) => graphNode.SetValue( new Vector3(
                   (float)nZs.GetValue(),
                   (float)nYs.GetValue(),
                   (float)o
                   ) ) );
                nz.SetParent( graphNode );

                Drawer drawer = DrawerProvider.GetDrawerOfType( nx.GetInstanceType() );
                drawer.Draw( list, nx, style );

                drawer = DrawerProvider.GetDrawerOfType( ny.GetInstanceType() );
                drawer.Draw( list, ny, style );

                drawer = DrawerProvider.GetDrawerOfType( nz.GetInstanceType() );
                drawer.Draw( list, nz, style );
            }
        }
    }
}
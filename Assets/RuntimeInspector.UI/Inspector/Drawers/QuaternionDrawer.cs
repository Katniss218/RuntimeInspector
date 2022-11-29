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
    [DrawerOf( typeof( Quaternion ) )]
    public sealed class QuaternionDrawer : Drawer
    {
        protected override void DrawInternal( RedrawDataInternal redrawData, ObjectGraphNode graphNode, InspectorStyle style )
        {
            bool isNull = false;
            if( graphNode.CanRead )
            {
                isNull = Utils.UnityUtils.IsUnityNull( graphNode.GetValue() );
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
                Vector3 euler = ((Quaternion)graphNode.GetValue()).eulerAngles;

                ObjectGraphNode nx = ObjectGraphNode.CreateNode( graphNode, "x", null, () => euler.x, ( o ) => graphNode.SetValue( Quaternion.Euler(
                   (float)o,
                   euler.y,
                   euler.z
                   ) ) );

                ObjectGraphNode ny = ObjectGraphNode.CreateNode( graphNode, "y", null, () => euler.y, ( o ) => graphNode.SetValue( Quaternion.Euler(
                   euler.x,
                   (float)o,
                   euler.z
                   ) ) );

                ObjectGraphNode nz = ObjectGraphNode.CreateNode( graphNode, "z", null, () => euler.z, ( o ) => graphNode.SetValue( Quaternion.Euler(
                   euler.x,
                   euler.y,
                   (float)o
                   ) ) );

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
using RuntimeEditor.Core;
using UnityPlus.AssetManagement;
using RuntimeEditor.UI.GUIUtils;
using RuntimeEditor.UI.Inspector.Attributes;
using RuntimeEditor.UI.ValueSelection;
using System;
using System.Linq;
using UnityEngine;

namespace RuntimeEditor.UI.Inspector.Drawers
{
    /// <summary>
    /// Draws instances of objects (not references).
    /// </summary>
    [DrawerOf( typeof( GameObject ) )]
    public sealed class GameObjectDrawer : Drawer
    {
        protected override void DrawInternal( RedrawDataInternal redrawData, ObjectGraphNode graphNode, InspectorStyle style )
        {
            // GameObject members of other classes can be drawn either as references, or assets.
            if( !graphNode.IsRoot )
            {
                if( graphNode.GetAttributes<AssetAttribute>().Any() )
                {
                    if( graphNode.CanRead && redrawData.CreateNew )
                    {
                        InspectorStandardFieldOrProperty.Create( redrawData.ObjectGraphNodeUI.Root, AssetRegistry<Sprite>.GetAsset( "RuntimeEditor/Sprites/icon_objectreference" ), graphNode, new AssetEntryProvider(), style );
                    }

                    return;
                }

                if( graphNode.CanRead && redrawData.CreateNew )
                {
                    InspectorStandardFieldOrProperty.Create( redrawData.ObjectGraphNodeUI.Root, AssetRegistry<Sprite>.GetAsset( "RuntimeEditor/Sprites/icon_objectreference" ), graphNode, new ObjectEntryProvider(), style );
                }

                return;
            }

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

                RectTransform label = InspectorLabel.Create( group, AssetRegistry<Sprite>.GetAsset( "RuntimeEditor/Sprites/icon_object" ), $"{graphNode.Name} >", style );

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
                int i = 0;
                foreach( var component in ((GameObject)graphNode.GetValue()).GetComponents<Component>() )
                {
                    ObjectGraphNode compNode = graphNode.AddNode( $"component[{i}]", typeof( GameObject ), () => component, null );

                    Drawer drawer = DrawerProvider.GetDrawerOfType( component.GetType() );
                    drawer.Draw( list, compNode, style );
                    i++;
                }
            }
        }
    }
}
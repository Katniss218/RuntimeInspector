using RuntimeInspector.Core;
using RuntimeInspector.Core.AssetManagement;
using RuntimeInspector.UI.GUIUtils;
using System;
using UnityEngine;

namespace RuntimeInspector.UI.Inspector.Drawers
{
    /// <summary>
    /// Draws instances of objects (not references).
    /// </summary>
    [DrawerOf( typeof( object ) )]
    public class ObjectDrawer : Drawer
    {
        protected override void DrawInternal( RedrawDataInternal redrawData, ObjectGraphNode node, InspectorStyle style )
        {
            bool isNull = false;
            if( node.CanRead )
            {
                isNull = IsUnityNull( node.GetValue() );
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

            if( node.CanRead && !isNull )
            {
                foreach( var memberBinding in node.Children )
                {
                    // skip over indexers.
                    if( memberBinding is ObjectGraphNodeProperty p && p.IndexParameters != null && p.IndexParameters.Length != 0 )
                    {
                        continue;
                    }
                    // Hide members defined by Unity component classes.
                    // - Component, MonoBehaviour and others have a bunch of internal Unity garbage that doesn't need to be shown.

                    Type declaringType = memberBinding.DeclaringType;
                    if(
                        declaringType == typeof( UnityEngine.Object )
                     || declaringType == typeof( UnityEngine.Component )
                     || declaringType == typeof( UnityEngine.Behaviour )
                     || declaringType == typeof( UnityEngine.MonoBehaviour ) )
                    {
                        continue;
                    }

                    Drawer drawer = DrawerProvider.GetDrawerOfType( memberBinding.GetInstanceType() );
                    drawer.Draw( list, memberBinding, style );
                }
            }
        }
    }
}
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
    [DrawerOf( typeof( object ) )]
    public sealed class ObjectDrawer : Drawer
    {
        protected override void DrawInternal( RedrawDataInternal redrawData, ObjectGraphNode graphNode, InspectorStyle style )
        {
            // not root
            // not a struct
            // not draw as value
            // - we can draw as reference.
            if( !graphNode.IsRoot && !graphNode.GetAttributes<DrawAsValueAttribute>().Any() && !graphNode.GetAttributes<AssetAttribute>().Any() )
            {
                if( graphNode.CanRead && redrawData.CreateNew )
                {
                    GameObject root = new GameObject( $"{graphNode.Name} ({graphNode.GetInstanceType().FullName})" );
                    root.layer = 5;

                    RectTransform rootTransform = root.AddComponent<RectTransform>();
                    rootTransform.SetParent( redrawData.ObjectGraphNodeUI.Root );
                    rootTransform.anchorMin = new Vector2( 0.0f, 0.5f );
                    rootTransform.anchorMax = new Vector2( 1.0f, 0.5f );
                    rootTransform.pivot = new Vector2( 0.5f, 0.5f );
                    rootTransform.anchoredPosition = new Vector2( 0.0f, 0.0f );
                    rootTransform.sizeDelta = new Vector2( 0.0f, style.FieldHeight );

                    RectTransform label = InspectorLabel.Create( rootTransform, AssetRegistry<Sprite>.GetAsset( "RuntimeInspector/Sprites/icon_objectreference" ), graphNode.Name, style );

                    GraphNodeUI graphNodeUI = redrawData.ObjectGraphNodeUI;

                    RectTransform value = InspectorReferenceInputField.Create( rootTransform, graphNodeUI, graphNode, style );

                    value.anchorMin = new Vector2( 0.5f, 0.0f );
                    value.anchorMax = new Vector2( 1.0f, 1.0f );
                    value.pivot = new Vector2( 1.0f, 0.5f );
                    value.anchoredPosition = new Vector2( 0.0f, 0.0f );
                    value.sizeDelta = new Vector2( 0.0f, 0.0f );
                }

                return;
            }

            if( graphNode.GetAttributes<AssetAttribute>().Any() )
            {
                if( graphNode.CanRead && redrawData.CreateNew )
                {
                    GameObject root = new GameObject( $"{graphNode.Name} ({graphNode.GetInstanceType().FullName})" );
                    root.layer = 5;

                    RectTransform rootTransform = root.AddComponent<RectTransform>();
                    rootTransform.SetParent( redrawData.ObjectGraphNodeUI.Root );
                    rootTransform.anchorMin = new Vector2( 0.0f, 0.5f );
                    rootTransform.anchorMax = new Vector2( 1.0f, 0.5f );
                    rootTransform.pivot = new Vector2( 0.5f, 0.5f );
                    rootTransform.anchoredPosition = new Vector2( 0.0f, 0.0f );
                    rootTransform.sizeDelta = new Vector2( 0.0f, style.FieldHeight );

                    RectTransform label = InspectorLabel.Create( rootTransform, AssetRegistry<Sprite>.GetAsset( "RuntimeInspector/Sprites/icon_objectreference" ), graphNode.Name, style );

                    GraphNodeUI graphNodeUI = redrawData.ObjectGraphNodeUI;

                    RectTransform value = InspectorAssetInputField.Create( rootTransform, graphNodeUI, graphNode, style );

                    value.anchorMin = new Vector2( 0.5f, 0.0f );
                    value.anchorMax = new Vector2( 1.0f, 1.0f );
                    value.pivot = new Vector2( 1.0f, 0.5f );
                    value.anchoredPosition = new Vector2( 0.0f, 0.0f );
                    value.sizeDelta = new Vector2( 0.0f, 0.0f );
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
                foreach( var memberBinding in graphNode.GetChildren() )
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
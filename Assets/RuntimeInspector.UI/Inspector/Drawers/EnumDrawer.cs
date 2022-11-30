using RuntimeInspector.Core;
using RuntimeInspector.Core.AssetManagement;
using RuntimeInspector.UI.GUIUtils;
using RuntimeInspector.UI.ValueSelection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RuntimeInspector.UI.Inspector.Drawers
{
    [DrawerOf( typeof( Enum ) )]
    public sealed class EnumDrawer : Drawer
    {
        protected override void DrawInternal( RedrawDataInternal redrawData, ObjectGraphNode graphNode, InspectorStyle style )
        {
            if( redrawData.CreateNew )
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

                RectTransform value = InspectorValueSelectionInputField.Create( rootTransform, graphNodeUI, graphNode, new EnumEntryProvider(), style );

                value.anchorMin = new Vector2( 0.5f, 0.0f );
                value.anchorMax = new Vector2( 1.0f, 1.0f );
                value.pivot = new Vector2( 1.0f, 0.5f );
                value.anchoredPosition = new Vector2( 0.0f, 0.0f );
                value.sizeDelta = new Vector2( 0.0f, 0.0f );

                //InspectorStandardFieldOrProperty.Create( redrawData.ObjectGraphNodeUI.Root, AssetRegistry<Sprite>.GetAsset( "RuntimeInspector/Sprites/icon_enum" ), graphNode, style );
            }
        }
    }
}
using RuntimeInspector.Core;
using UnityPlus.AssetManagement;
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
                InspectorStandardFieldOrProperty.Create( redrawData.ObjectGraphNodeUI.Root, AssetRegistry<Sprite>.GetAsset( "RuntimeInspector/Sprites/icon_enum" ), graphNode, new EnumEntryProvider(), style );
            }
        }
    }
}
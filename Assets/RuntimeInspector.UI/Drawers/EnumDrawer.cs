using RuntimeInspector.Core;
using RuntimeInspector.Core.AssetManagement;
using RuntimeInspector.UI.GUIUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RuntimeInspector.UI.Drawers
{
    [DrawerOf( typeof( Enum ) )]
    public class EnumDrawer : Drawer
    {
        protected override void DrawInternal( RedrawDataInternal redrawData, ObjectGraphNode binding, InspectorStyle style )
        {
            if( redrawData.CreateNew )
            {
                InspectorStandardFieldOrProperty.Create( redrawData.ObjectGraphNodeUI.Root, AssetRegistry<Sprite>.GetAsset( "RuntimeInspector/Sprites/icon_enum" ), binding, style );
            }
        }
    }
}
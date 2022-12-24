using RuntimeEditor.Core;
using UnityPlus.AssetManagement;
using RuntimeEditor.UI.GUIUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RuntimeEditor.UI.Inspector.Drawers
{
    [DrawerOf( typeof( string ) )]
    public sealed class StringDrawer : Drawer
    {
        protected override void DrawInternal( RedrawDataInternal redrawData, ObjectGraphNode binding, InspectorStyle style )
        {
            if( redrawData.CreateNew )
            {
                InspectorStandardFieldOrProperty.Create( redrawData.ObjectGraphNodeUI.Root, AssetRegistry<Sprite>.GetAsset( "RuntimeEditor/Sprites/icon_string16" ), binding, style );
            }
        }
    }
}
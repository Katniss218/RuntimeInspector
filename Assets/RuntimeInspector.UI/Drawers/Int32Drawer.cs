using RuntimeInspector.Core;
using RuntimeInspector.Core.AssetManagement;
using RuntimeInspector.UI.GUIUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RuntimeInspector.UI.Drawers
{
    [DrawerOf( typeof( int ) )]
    public class Int32Drawer : Drawer
    {
        public override RectTransform Draw( RectTransform parent, MemberBinding binding, InspectorStyle style )
        {
            RectTransform root = InspectorFieldOrProperty.Create( parent, AssetRegistry<Sprite>.GetAsset( "RuntimeInspector/Sprites/icon_int32" ), binding, style );

            return root;
        }
    }
}
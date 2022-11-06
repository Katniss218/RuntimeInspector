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
    [DrawerOf( typeof( float ) )]
    public class SingleDrawer : Drawer
    {
        public override (RectTransform, UIObjectGraphBinding) Draw( RectTransform parent, ObjectGraphNode binding, InspectorStyle style )
        {
            RedrawData redrawData = RedrawData.GetRedrawData( binding );

            int siblingIndex = -2;
            if( redrawData.DestroyOld )
            {
                siblingIndex = redrawData.Binding.Root.GetSiblingIndex();
                Object.Destroy( redrawData.Binding.Root.gameObject );
            }
            if( redrawData.CreateNew )
            {
                (RectTransform, UIObjectGraphBinding) obj = InspectorFieldOrProperty.Create( parent, AssetRegistry<Sprite>.GetAsset( "RuntimeInspector/Sprites/icon_binary32" ), binding, style );

                if( siblingIndex != -2 )
                {
                    obj.Item1.SetSiblingIndex( siblingIndex );
                }

                return obj;
            }

            return (redrawData.Binding?.Root, redrawData.Binding);
        }
    }
}
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
    [DrawerOf( typeof( bool ) )]
    public class BooleanDrawer : Drawer
    {
        public override (RectTransform, UIBinding) Draw( RectTransform parent, MemberBinding binding, InspectorStyle style )
        {
            (bool destroyOld, bool createNew, UIBinding uiBinding) = GetRedrawMode( binding );

            int siblingIndex = -2;
            if( destroyOld )
            {
                siblingIndex = uiBinding.Root.GetSiblingIndex();
                GameObject.Destroy( uiBinding.Root.gameObject );
            }
            if( createNew )
            {
                (RectTransform, UIBinding) obj = InspectorFieldOrProperty.Create( parent, AssetRegistry<Sprite>.GetAsset( "RuntimeInspector/Sprites/icon_boolean" ), binding, style );

                if( siblingIndex != -2 )
                {
                    obj.Item1.SetSiblingIndex( siblingIndex );
                }

                return obj;
            }

#warning TODO - the return value seems to not be used or useful.
            return (uiBinding.Root, uiBinding);
        }
    }
}
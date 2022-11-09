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
    [DrawerOf( typeof( bool ) )]
    public class BooleanDrawer : Drawer
    {
        public override (RectTransform, UIObjectGraphBinding) Draw( RectTransform parent, ObjectGraphNode binding, InspectorStyle style )
        {
            RedrawData redrawData = RedrawData.GetRedrawData( binding );
            if( redrawData.Hidden ) // this for some reason prevents the null list and whatnot.
            {
                return (null, null);
            }
            if( redrawData.Binding == null )
            {
                (_, redrawData.Binding) = UINode.Create( parent, binding, style );
            }

            if( redrawData.DestroyOld )
            {
                Object.Destroy( redrawData.Binding.Root.GetChild( 0 ).gameObject );
            }
            if( redrawData.CreateNew )
            {
                (RectTransform, UIObjectGraphBinding) obj = InspectorFieldOrProperty.Create( redrawData.Binding.Root, AssetRegistry<Sprite>.GetAsset( "RuntimeInspector/Sprites/icon_boolean" ), binding, style );

                return obj;
            }

            return (redrawData.Binding?.Root, redrawData.Binding);
        }
    }
}
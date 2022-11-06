using RuntimeInspector.Core;
using RuntimeInspector.Core.AssetManagement;
using RuntimeInspector.UI.GUIUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace RuntimeInspector.UI.Drawers
{
    /// <summary>
    /// Draws instances of objects (not references).
    /// </summary>
    public class ObjectDrawer : Drawer
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

            bool isNullOrWriteOnly = true;
            if( binding.CanRead )
            {
                // UnityObjects are not truly null, UnityObject overrides the `==` operator to make empty references equal to null.
                object value = binding.GetValue();
                isNullOrWriteOnly = value == null;
                if( value is Object unityobject )
                {
                    isNullOrWriteOnly = unityobject == null;
                }
            }

            if( redrawData.DestroyOld )
            {
                Object.Destroy( redrawData.Binding.Root.GetChild( 0 ).gameObject );
            }

            RectTransform list = null;
            if( redrawData.CreateNew )
            {
                RectTransform group = InspectorVerticalList.Create( "group", redrawData.Binding.Root, style, new InspectorVerticalList.Params() { IncludeMargin = false } );

                RectTransform label = InspectorLabel.Create( group, AssetRegistry<Sprite>.GetAsset( "RuntimeInspector/Sprites/icon_object" ), $"{binding.Name} >", style );

                if( !isNullOrWriteOnly )
                {
                    list = InspectorVerticalList.Create( "list", group, style, new InspectorVerticalList.Params() { IncludeMargin = true } );
                }
            }
            else
            {
                if( !isNullOrWriteOnly )
                {
                    list = InspectorVerticalList.Find( "list", redrawData.Binding?.Root );
                }
            }

            // Set up the UI elements that will be shown/updated.
            if( !isNullOrWriteOnly )
            {
                foreach( var memberBinding in binding.Children )
                {
                    // Don't list complete inheritance tree of certain types.
                    // - Component and MonoBehaviour have a bunch of internal Unity garbage.

                    Type declaringType = memberBinding.DeclaringType;
                    if(
                        declaringType == typeof( Object )
                     || declaringType == typeof( Component )
                     || declaringType == typeof( Behaviour )
                     || declaringType == typeof( MonoBehaviour ) )
                    {
                        continue;
                    }

                    Drawer drawer = DrawerProvider.GetDrawerOfType( memberBinding.GetInstanceType() );
                    drawer.Draw( list, memberBinding, style );
                }
            }
#warning TODO - 'list' is not the root.
            return (redrawData.Binding.Root, redrawData.Binding);
        }
    }
}
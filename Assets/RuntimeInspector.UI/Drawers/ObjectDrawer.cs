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
        protected override (RectTransform, UIObjectGraphBinding) DrawInternal( RedrawData redrawData, ObjectGraphNode binding, InspectorStyle style )
        {
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

            RectTransform list = null;
            if( redrawData.CreateNew )
            {
                RectTransform group = InspectorVerticalList.Create( "group", redrawData.GraphUI.Root, style, new InspectorVerticalList.Params() { IncludeMargin = false } );

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
                    list = InspectorVerticalList.Find( "list", redrawData.GraphUI?.Root );
                }
            }

            // regardless whether the node was created or not, we should ping the child objects.

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
            return (redrawData.GraphUI.Root, redrawData.GraphUI);
        }
    }
}
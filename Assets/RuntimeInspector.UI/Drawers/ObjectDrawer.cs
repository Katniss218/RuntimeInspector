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
        protected override void DrawInternal( RedrawDataInternal redrawData, ObjectGraphNode binding, InspectorStyle style )
        {
            bool isNull = false;
            if( binding.CanRead )
            {
                // UnityObjects are not truly null, UnityObject overrides the `==` operator to make empty references equal to null.
                object value = binding.GetValue();
                if( value is Object unityobject )
                {
                    isNull = unityobject == null;
                }
                else
                {
                    isNull = value == null;
                }
            }

            RectTransform list = null;
            RectTransform group;
            if( redrawData.CreateNew )
            {
                group = InspectorVerticalList.Create( "group", redrawData.ObjectGraphNodeUI.Root, style, new InspectorVerticalList.Params() { IncludeMargin = false } );

                RectTransform label = InspectorLabel.Create( group, AssetRegistry<Sprite>.GetAsset( "RuntimeInspector/Sprites/icon_object" ), $"{binding.Name} >", style );

                if( binding.CanRead && !isNull )
                {
                    list = InspectorVerticalList.Create( "list", group, style, new InspectorVerticalList.Params() { IncludeMargin = true } );
                }
            }
            else
            {
                if( binding.CanRead && !isNull )
                {
                    group = InspectorVerticalList.Find( "group", redrawData.ObjectGraphNodeUI?.Root );

                    list = InspectorVerticalList.Find( "list", group );
                }
            }

            // regardless whether the node was created or not, we should ping the child objects.

            // Set up the UI elements that will be shown/updated.
            if( binding.CanRead && !isNull )
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
        }
    }
}
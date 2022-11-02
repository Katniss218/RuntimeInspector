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
        public override (RectTransform, UIBinding) Draw( RectTransform parent, MemberBinding binding, InspectorStyle style )
        {
            (bool destroyOld, bool createNew, UIBinding uiBinding) = GetRedrawMode( binding );

            int siblingIndex = -2;
            if( destroyOld )
            {
                siblingIndex = uiBinding.Root.GetSiblingIndex();
                Object.Destroy( uiBinding.Root.gameObject );
            }

            RectTransform list;
            if( createNew )
            {
#warning TODO - move this and the new list to a separate helper method.
                GameObject gameObject = new GameObject( "group" );
                gameObject.layer = 5;

                RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
                rectTransform.SetParent( parent );

                if( siblingIndex != -2 )
                {
                    rectTransform.SetSiblingIndex( siblingIndex );
                }

                rectTransform.anchorMin = new Vector2( 0.0f, 1.0f );
                rectTransform.anchorMax = new Vector2( 1.0f, 1.0f );
                rectTransform.pivot = new Vector2( 0.0f, 0.5f );
                rectTransform.anchoredPosition = new Vector2( 0.0f, 0.0f );
                rectTransform.sizeDelta = new Vector2( 0.0f, 0.0f );

                VerticalLayoutGroup layoutGroup = gameObject.AddComponent<VerticalLayoutGroup>();
                layoutGroup.padding = new RectOffset( 0, 0, 0, 0 );
                layoutGroup.childControlWidth = true;
                layoutGroup.childControlHeight = false;
                layoutGroup.childScaleWidth = false;
                layoutGroup.childScaleHeight = false;
                layoutGroup.childForceExpandWidth = true;
                layoutGroup.childForceExpandHeight = false;

                ContentSizeFitter fitter = gameObject.AddComponent<ContentSizeFitter>();

                fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

                UIBinding insBi = gameObject.AddComponent<UIBinding>();
                insBi.Root = rectTransform;
                insBi.Binding = binding;

                RectTransform label = InspectorLabel.Create( rectTransform, AssetRegistry<Sprite>.GetAsset( "RuntimeInspector/Sprites/icon_object" ), $"{binding.Metadata.Name} >", style );

                list = InspectorVerticalList.Create( "list", rectTransform, style );
            }
            else
            {
                list = InspectorVerticalList.Find( "list", uiBinding.Root );
            }

            // Set up the UI elements that will be shown/updated.
#warning TODO - read-only objects.
            if( binding.Binding.GetValue() != null )
            {
                foreach( var memberBinding in binding.Binding.InstanceMembers )
                {
                    // Don't list complete inheritance tree of certain types.
                    // - Component and MonoBehaviour have a bunch of internal Unity garbage.

                    Type declaringType = memberBinding.Metadata.DeclaringType;
                    if(
                        declaringType == typeof( Object )
                     || declaringType == typeof( Component )
                     || declaringType == typeof( Behaviour )
                     || declaringType == typeof( MonoBehaviour ) )
                    {
                        continue;
                    }

#warning TODO - the property of type object is not drawn?
                    Drawer drawer = DrawerProvider.GetDrawerOfType( memberBinding.GetDrawnType() );
                    drawer.Draw( list, memberBinding, style );
                }
            }
            return (list, null);
        }
    }
}
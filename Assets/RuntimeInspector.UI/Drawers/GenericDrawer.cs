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

namespace RuntimeInspector.UI.Drawers
{
    public class GenericDrawer : Drawer
    {
        public override RectTransform Draw( RectTransform parent, MemberBinding binding, InspectorStyle style )
        {
            for( int i = 0; i < parent.childCount; i++ )
            {
                UnityEngine.Object.Destroy( parent.GetChild( i ).gameObject );
            }
            /*
            GameObject go = new GameObject();
            go.layer = 5;

            RectTransform rectTransform = go.AddComponent<RectTransform>();
            rectTransform.SetParent( parent );

            rectTransform.anchorMin = new Vector2( 0.0f, 1.0f );
            rectTransform.anchorMax = new Vector2( 1.0f, 1.0f );
            rectTransform.pivot = new Vector2( 0.0f, 0.5f );
            rectTransform.anchoredPosition = new Vector2( 0.0f, 0.0f );
            rectTransform.sizeDelta = new Vector2( 0.0f, 100.0f );

            VerticalLayoutGroup layoutGroup = go.AddComponent<VerticalLayoutGroup>();
            layoutGroup.padding = new RectOffset( 0, 0, 0, 0 );
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = false;
            layoutGroup.childScaleWidth = false;
            layoutGroup.childScaleHeight = false;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = false;


            ContentSizeFitter fitter = go.AddComponent<ContentSizeFitter>();

            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            */
            RectTransform label = InspectorLabel.Create( parent, AssetRegistry<Sprite>.GetAsset( "RuntimeInspector/Sprites/icon_object" ), $"{binding.Metadata.Name} >", style );

            RectTransform list = InspectorVerticalList.Create( parent, style );

            // Set up the UI elements that will be shown/updated.
            var members = binding.Binding.InstanceMembers;
            foreach( var memberBinding in members )
            {
                // Don't list complete inheritance tree of certain types.
                // - Component and MonoBehaviour have a bunch of internal Unity garbage.
                if( memberBinding.Metadata.DeclaringType == typeof( Component ) || memberBinding.Metadata.DeclaringType == typeof( MonoBehaviour ) )
                {
                    continue;
                }

                if( !memberBinding.Metadata.CanRead )
                {
                    // draw as reference field.
                    continue;
                }
                if( !memberBinding.Binding.HasChangedValue( out _ ) )
                {
                    continue;
                }
                try
                {
                    Type type = memberBinding.Binding.GetInstanceType();
                    Drawer drawer = DrawerProvider.GetDrawerOfType( type );

                    RectTransform rt = drawer.Draw( list, memberBinding, style );
                }
                catch( Exception ex )
                {
                    Debug.LogWarning( $"EXCEPTION while trying to get value of: {ex}" );
                    // temporary.
                }
            }

            return list;
        }
    }
}
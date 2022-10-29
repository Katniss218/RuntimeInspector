using RuntimeInspector.Core;
using RuntimeInspector.UI.GUIUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RuntimeInspector.UI.Drawers
{
    public class GenericDrawer : Drawer
    {
        public override RectTransform Draw( RectTransform parent, MemberBinding binding, InspectorStyle style )
        {
            /*for( int i = 0; i < parent.childCount; i++ )
            {
                UnityEngine.Object.Destroy( parent.GetChild( i ).gameObject );
            }*/

            RectTransform label = InspectorLabel.Create( parent, $"{binding.Metadata.Name} >", style );

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
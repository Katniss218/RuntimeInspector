using RuntimeInspector.Core;
using RuntimeInspector.UI.GUIUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RuntimeInspector.UI
{
    public abstract class Drawer
    {
        public List<Drawer> ChildDrawers { get; set; }

        public abstract RectTransform Draw( RectTransform parent, MemberBinding binding, InspectorStyle style );

        /// <summary>
        /// Inverse of draw. Converts user input to a value.
        /// </summary>
        public abstract object InputToValueGeneral( object input ); // doesn't necessarily have to be a string.
    }

    public abstract class TypedDrawer<T> : Drawer
    {
        protected static Type UnderlyingType = typeof( T );

        public override object InputToValueGeneral( object input )
        {
            return InputToValue( input );
        }

        /// <summary>
        /// Converts user input (input field) into the value that will be assigned.
        /// </summary>
        public abstract T InputToValue( object input );
    }

    public class GenericDrawer : Drawer
    {
        public override RectTransform Draw( RectTransform parent, MemberBinding binding, InspectorStyle style )
        {
            for( int i = 0; i < parent.childCount; i++ )
            {
                UnityEngine.Object.Destroy( parent.GetChild( i ).gameObject );
            }

            RectTransform list = InspectorVerticalList.Create( parent, style );

            // Set up the UI elements that will be shown/updated.
            var members = binding.Binding.GetInstanceMembers();
            foreach( var memberBinding in members )
            {
                // Don't list complete inheritance tree of certain types.
                // - Component and MonoBehaviour have a bunch of internal Unity garbage.
                if( memberBinding.Metadata.DeclaringType == typeof( Component ) || memberBinding.Metadata.DeclaringType == typeof( MonoBehaviour ) )
                {
                    continue;
                }

                GameObject root = new GameObject( "_binding" );
                root.layer = 5;
                RectTransform rootTransform = root.AddComponent<RectTransform>();

                rootTransform.SetParent( list );
                rootTransform.sizeDelta = new Vector2( 0.0f, style.FieldHeight );

                InspectorValueSubmitter submitter = root.AddComponent<InspectorValueSubmitter>();

                if( !memberBinding.Metadata.CanRead )
                {
                    // draw as reference field.
                    continue;
                }
                try
                {
                    Type type = memberBinding.Binding.GetInstanceType();
                    Drawer drawer = DrawerManager.GetDrawerOfType( type );
                    if( drawer is GenericDrawer )
                    {
#error TODO - without this there is an infinite stack look somewhere.
                        continue;
                    }
                    RectTransform rt = drawer.Draw( rootTransform, memberBinding, style );
                }
                catch( Exception ex )
                {
                    Debug.LogWarning( $"EXCEPTION while trying to get value of: {ex}" );
                    // temporary.
                }
            }

            return list;
        }

        public override object InputToValueGeneral( object input )
        {
            throw new InvalidOperationException( "Can't use generic drawer to convert string into object" );
        }
    }
}
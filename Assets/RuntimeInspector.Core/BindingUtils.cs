using RuntimeInspector.Core.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeInspector.Core
{
    /// <summary>
    /// Public methods to create bindings to objects.
    /// </summary>
    public static class BindingUtils
    {
        internal static List<MemberBinding> GetMembersOf( object instance )
        {
#warning TODO - can be cached.
            List<MemberBinding> bindings = new List<MemberBinding>();

            Type instanceType = instance.GetType();

            Type currentDeclaringType = instanceType;

            while( true )
            {
                FieldInfo[] fields = currentDeclaringType.GetFields( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly );
                PropertyInfo[] properties = currentDeclaringType.GetProperties( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly );

                foreach( var field in fields )
                {
                    IObjectBinding binding = FieldBinding.Create( field, instance );
                    MemberMetadata metadata = MemberMetadata.FromField( field );

                    bindings.Add( new MemberBinding()
                    {
                        Metadata = metadata,
                        Binding = binding
                    } );
                }

                foreach( var property in properties )
                {
                    IObjectBinding binding = PropertyBinding.Create( property, instance );
                    MemberMetadata metadata = MemberMetadata.FromProperty( property );

                    bindings.Add( new MemberBinding()
                    {
                        Metadata = metadata,
                        Binding = binding
                    } );
                }

                if( currentDeclaringType.BaseType == null )
                {
                    break;
                }

                currentDeclaringType = currentDeclaringType.BaseType;
            }

            return bindings;
        }

        /// Returns a binding to an arbitrary object.
        public static MemberBinding GetBinding( Func<object> objGetter, Action<object> objSetter )
        {
            // just create the binding for this object. Getting members is implementation-specific.
            return new MemberBinding()
            {
                Metadata = new MemberMetadata(),
                Binding = new ArbitraryBinding( objGetter, objSetter )
            };
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeInspector.UI
{
    public static class DrawerManager
    {
        private static Dictionary<Type, IDrawer> cachedDrawers = new Dictionary<Type, IDrawer>();

        public static IDrawer GetDrawerOfType( Type type )
        {
            if( cachedDrawers.TryGetValue( type, out IDrawer cd ) )
            {
                return cd;
            }

            Type drawerIType = typeof( IDrawer );
            Type drawerType = typeof( TypedDrawer<> );

            Type targetType = type; // the type we want to get the drawer for.

            // if the type is generic, we want to first check if a specific drawer for this specific type exists
            // if not, get the drawer for the definition of the generic type.

            IEnumerable<Type> drawers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany( a => a.GetTypes() )
                .Where( t => t != drawerIType && t != drawerType )// not the generic drawer itself or the interface itself
                .Where( t => drawerIType.IsAssignableFrom( t ) );

            // specific drawer.
            Type drawer = drawers.FirstOrDefault( t => t.BaseType.GenericTypeArguments.SequenceEqual( new[] { targetType } ) );

            if( drawer == null && type.IsGenericType ) // if there is no drawer for a specific generic type, get for the unspecified generic type.
            {
                targetType = type.GetGenericTypeDefinition();

                foreach( var d in drawers )
                {
                    // d.BaseType = Drawer<Func<T, TResult>>
                    // genericTypes = [] { Func<T, TResult> }
                    // drawerGenericType = Func<T, TResult> -- but without calling getgenerictypedefinition, the type is fucky.
                    Type[] genericTypes = d.BaseType.GetGenericArguments();

                    // current drawer is not generic at all, thus can't match a generic target type.
                    if( !genericTypes.Any() || !genericTypes[0].IsGenericType )
                    {
                        continue;
                    }

                    Type drawerGenericType = genericTypes[0].GetGenericTypeDefinition(); // drawer<T>, get the 'T'.

                    if( drawerGenericType == targetType )
                    {
                        drawer = d.MakeGenericType( type.GetGenericArguments() ); // get a drawer for the specific arguments of target.
                        break;
                    }
                }
            }

            IDrawer foundDrawer = null;

            if( drawer == null )
            {
                foundDrawer = new GenericDrawer();
                cachedDrawers.Add( type, foundDrawer );

                return foundDrawer;
            }

            foundDrawer = (IDrawer)Activator.CreateInstance( drawer );
            cachedDrawers.Add( type, foundDrawer );

            return foundDrawer;
        }
    }
}
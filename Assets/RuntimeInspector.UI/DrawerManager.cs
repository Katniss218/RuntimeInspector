using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeInspector.UI
{
    public static class DrawerManager
    {
        private static Dictionary<Type, Drawer> cachedDrawers = new Dictionary<Type, Drawer>();

        public static Drawer GetDrawerOfType( Type type )
        {
            if( cachedDrawers.TryGetValue( type, out Drawer cd ) )
            {
                return cd;
            }

            Type baseDrawerType = typeof( Drawer );

            Type drawerType = typeof( TypedDrawer<> );

            Type targetType = type; // the type we want to get the drawer for.

            // if the type is generic, we want to first check if a specific drawer for this specific type exists
            // if not, get the drawer for the definition of the generic type.

            IEnumerable<Type> drawers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany( a => a.GetTypes() )
                .Where( t => !t.IsAbstract && t != drawerType && t != baseDrawerType )// not the generic drawer itself or the interface itself
                .Where( t => baseDrawerType.IsAssignableFrom( t ) );

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

            Drawer foundDrawer = null;

#warning TODO - temporarily don't add to dict because we're testing whether I can hold the drawers nested in each other as a representation of the structure of the drawn object.
            // this is for persistence and so I don't have to redraw the entire thing??

            if( drawer == null )
            {
                foundDrawer = new GenericDrawer();
            //    cachedDrawers.Add( type, foundDrawer );

                return foundDrawer;
            }

            foundDrawer = (Drawer)Activator.CreateInstance( drawer );
        //    cachedDrawers.Add( type, foundDrawer );

            return foundDrawer;
        }
    }
}
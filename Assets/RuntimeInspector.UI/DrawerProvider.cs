using RuntimeInspector.UI.Drawers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RuntimeInspector.UI
{
    public static class DrawerProvider
    {
        /// <summary>
        /// Maps the DrawerOf.DrawnType to the drawer type.
        /// </summary>
        private static Dictionary<Type, Type> availableDrawers = new Dictionary<Type, Type>();

        private static readonly Type baseDrawerType = typeof( Drawer );

        private static readonly Type genericDrawerType = typeof( ObjectDrawer );

        private static readonly Type drawerOfAttributeType = typeof( DrawerOfAttribute );

        public static void ReloadDrawers()
        {
            availableDrawers = new Dictionary<Type, Type>();

            List<Type> availableTypedDrawers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany( a => a.GetTypes() )
                .Where( 
                    dt => !dt.IsAbstract
                 && dt != baseDrawerType
                 && dt != genericDrawerType
                 && baseDrawerType.IsAssignableFrom( dt ) )
                .ToList();

            foreach( var typedDrawerType in availableTypedDrawers )
            {
                object[] drawerOfAttributes = typedDrawerType.GetCustomAttributes( drawerOfAttributeType, false );
                if( drawerOfAttributes.Length == 0 )
                {
                    continue;
                }
                foreach( var attribute in drawerOfAttributes )
                {
                    DrawerOfAttribute drawerOfAttribute = (DrawerOfAttribute)attribute;

                    Type drawnType = drawerOfAttribute.DrawnType;

                    if( availableDrawers.ContainsKey( drawnType ) )
                    {
                        Debug.LogWarning( $"Drawer for type '{drawnType}' is already registerd." );
                        continue;
                    }
                    availableDrawers.Add( drawnType, typedDrawerType );
                }
            }
        }

        public static Drawer GetDrawerOfType( Type type )
        {
            if( !availableDrawers.Any() )
            {
                ReloadDrawers();
            }

            Type targetType = type; // the type we want to get the drawer for.

            availableDrawers.TryGetValue( targetType, out Type drawer );

            if( drawer == null )
            {
                if( type.IsGenericType ) // if there is no drawer for a specific generic type, get for the unspecified generic type.
                {
                    targetType = type.GetGenericTypeDefinition();

                    availableDrawers.TryGetValue( targetType, out drawer );
                }
                else // drawer for the base type of the type we want.
                {
                    targetType = type.BaseType;

                    while( true )
                    {
                        availableDrawers.TryGetValue( targetType, out drawer );

                        if( drawer != null )
                        {
                            break;
                        }
                        if( targetType.BaseType == null )
                        {
                            break;
                        }

                        targetType = targetType.BaseType;
                    }
                }
            }

            Drawer foundDrawer;

            if( drawer == null )
            {
                foundDrawer = new ObjectDrawer();
                return foundDrawer;
            }

            foundDrawer = (Drawer)Activator.CreateInstance( drawer );
            return foundDrawer;
        }
    }
}
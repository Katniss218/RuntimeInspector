using RuntimeInspector.Core.AssetManagement.Providers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RuntimeInspector.Core.AssetManagement
{
    /// <summary>
    /// A self contained registry for one type of asset.
    /// </summary>
    public static class AssetRegistry<T>
    {
        private static IAssetProvider<T>[] providers;

        private static IDictionary<string, T> registry = new Dictionary<string, T>();
        private static IDictionary<T, string> reverseRegistry = new Dictionary<T, string>();

        private static bool hasLazyLoaded = false;

        static AssetRegistry()
        {
            Type providerType = typeof( IAssetProvider<T> );

            List<Type> prov = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany( a => a.GetTypes() )
                .Where( t => t != providerType )// not the generic provider interface itself
                .Where( t => providerType.IsAssignableFrom( t ) )
                .ToList();

            providers = new IAssetProvider<T>[prov.Count];
            for( int i = 0; i < providers.Length; i++ )
            {
                providers[i] = (IAssetProvider<T>)Activator.CreateInstance( prov[i] );
            }
        }

        private static void TryLazyLoad()
        {
            // Already loaded and wasn't cleared - return.
            if( hasLazyLoaded )
            {
                return;
            }

            // Ask every provider for their assets.
            foreach( var provider in providers )
            {
                IEnumerable<(string assetID, T obj)> objs = provider.GetAll();

                foreach( var (assetID, obj) in objs )
                {
                    registry.Add( assetID, obj );
                    reverseRegistry.Add( obj, assetID );
                }
            }

            hasLazyLoaded = true;
        }

        private static void TryLazyLoadOne( string assetID )
        {
            // Ask every provider for their asset.
            foreach( var provider in providers )
            {
                if( provider.Get( assetID, out T obj ) )
                {
                    registry.Add( assetID, obj );
                    reverseRegistry.Add( obj, assetID );
                }
            }
        }

        private static void TryLazyLoadOne( T obj )
        {
            // Ask every provider for their asset.
            foreach( var provider in providers )
            {
                if( provider.GetAssetID( obj, out string assetID ) )
                {
                    registry.Add( assetID, obj );
                    reverseRegistry.Add( obj, assetID );
                }
            }
        }

        /// <summary>
        /// Clears the registry.
        /// </summary>
        public static void ClearRegistry()
        {
            registry.Clear();
            reverseRegistry.Clear();
            hasLazyLoaded = false;
        }

        /// <summary>
        /// Registers an asset with the specified assetID manually.
        /// </summary>
        public static void Register( string assetID, T obj )
        {
            if( registry.ContainsKey( assetID ) )
            {
                throw new InvalidOperationException( $"A '{typeof( T ).FullName}' asset with assetID '{assetID}' is already registered." );
            }

            registry.Add( assetID, obj );
            reverseRegistry.Add( obj, assetID );
        }

        /// <summary>
        /// Checks whether an asset with the specified assetID exists.
        /// </summary>
        public static bool Exists( string assetID )
        {
            if( registry.Count == 0 )
            {
                TryLazyLoad();
            }

            if( registry.ContainsKey( assetID ) )
            {
                return true;
            }

            // Sometimes assets can't be loaded all at once (because searching all paths is unavailable).
            TryLazyLoadOne( assetID );
            return registry.ContainsKey( assetID );
        }

        /// <summary>
        /// Returns an asset with the specified assetID. Throws an exception if none are found.
        /// </summary>
        /// <remarks>
        /// After the asset has been loaded, this is an O(1) dictionary lookup.
        /// </remarks>
        public static T GetAsset( string assetID )
        {
            if( registry.Count == 0 )
            {
                TryLazyLoad();
            }

            if( registry.TryGetValue( assetID, out T val ) )
            {
                return val;
            }

            // Sometimes assets can't be loaded all at once (because searching all paths is unavailable).
            TryLazyLoadOne( assetID );
            if( registry.TryGetValue( assetID, out val ) )
            {
                return val;
            }

            throw new InvalidOperationException( $"A '{typeof( T ).FullName}' asset with assetID '{assetID}' is not registered." );
        }

        public static List<(string assetID, T obj)> GetAll()
        {
            if( registry.Count == 0 )
            {
                TryLazyLoad();
            }

            List<(string assetID, T obj)> all = new List<(string assetID, T obj)>();

            foreach( var (key, obj) in registry )
            {
                all.Add( (key, obj) );
            }

            return all;
        }

        /// <summary>
        /// This can be used with reflection.
        /// </summary>
        public static List<(string assetID, object obj)> GetAllReflection()
        {
            if( registry.Count == 0 )
            {
                TryLazyLoad();
            }

            List<(string assetID, object obj)> all = new List<(string assetID, object obj)>();

            foreach( var (key, obj) in registry )
            {
                all.Add( (key, obj) );
            }

            return all;
        }

        public static string GetAssetID( T obj )
        {
            if( registry.Count == 0 )
            {
                TryLazyLoad();
            }

            if( reverseRegistry.TryGetValue( obj, out string assetID ) )
            {
                return assetID;
            }

            // Sometimes assets can't be loaded all at once (because searching all paths is unavailable).
            TryLazyLoadOne( obj );
            if( reverseRegistry.TryGetValue( obj, out assetID ) )
            {
                return assetID;
            }

            throw new InvalidOperationException( $"A '{typeof( T ).FullName}' asset is not registered. Can't find an assetID." );
        }
    }
}
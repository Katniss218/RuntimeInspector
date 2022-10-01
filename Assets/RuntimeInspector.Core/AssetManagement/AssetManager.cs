using RuntimeInspector.Core.AssetManagement.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RuntimeInspector.Core.AssetManagement
{
    public static class AssetManager
    {
        /// <summary>
        /// Provides meshes.
        /// </summary>
        public static AssetRegistry<Mesh> Meshes { get; private set; } = new AssetRegistry<Mesh>( new MeshProviderResources() );

        public static string GetAssetID( object asset )
        {
            if( asset is Mesh )
            {
                return Meshes.GetAssetID( (Mesh)asset );
            }
            throw new ArgumentException( $"The type '{asset.GetType().FullName}' is not a supported asset type." );
        }

        public static T GetAsset<T>( string assetID )
        {
            if( typeof( T ) == typeof( Mesh ) )
            {
                return (T)(object)Meshes.Get( assetID ); // TODO - ugly and probs slow.
            }
            throw new ArgumentException( $"The type '{typeof( T ).FullName}' is not a supported asset type." );
        }
    }
}

using UnityPlus.AssetManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RuntimeEditor.UI.ValueSelection
{
    public class AssetEntryProvider : IEntryProvider
    {
        public Entry[] GetAllEntries( Type type )
        {
            Type assetRegistryType = typeof( AssetRegistry<> );
            assetRegistryType = assetRegistryType.MakeGenericType( type );

            MethodInfo method = assetRegistryType.GetMethod( nameof( AssetRegistry<object>.GetAllReflection ) );
            try
            {
                List<(string assetID, object obj)> foundAssets = (List<(string, object)>)method.Invoke( null, null );

                Entry[] entries = new Entry[foundAssets.Count + 1];

                entries[0] = Entry.Default;

                int i = 0;
                foreach( var obj in foundAssets )
                {
                    entries[i + 1] = new Entry()
                    {
                        Identifier = obj.assetID,
                        Obj = obj.obj
                    };

                    i++;
                }

                return entries;
            }
            catch( InvalidOperationException ex )
            {
                Debug.LogException( ex );

                return new Entry[]
                {
                    Entry.Default
                };
            }
        }

        /*
        
        private Entry? FindExact( string assetID )
        {
            Type assetRegistryType = typeof( AssetRegistry<> );
            assetRegistryType = assetRegistryType.MakeGenericType( Type );

            MethodInfo checkMethod = assetRegistryType.GetMethod( nameof( AssetRegistry<object>.Exists ) );
            MethodInfo getMethod = assetRegistryType.GetMethod( nameof( AssetRegistry<object>.GetAsset ) );

            if( (bool)checkMethod.Invoke( null, new[] { assetID } ) )
            {
                object asset = getMethod.Invoke( null, new[] { assetID } );

                return new Entry() { AssetID = assetID, Obj = asset };
            }
            return null;
        }

        */
    }
}
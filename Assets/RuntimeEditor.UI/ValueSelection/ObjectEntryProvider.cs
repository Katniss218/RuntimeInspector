using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RuntimeEditor.UI.ValueSelection
{
    public class ObjectEntryProvider : IEntryProvider
    {
        public Entry[] GetAllEntries( Type type )
        {
            Object[] allObjects = Object.FindObjectsOfType( type );

            Entry[] entries = new Entry[allObjects.Length + 1];
            entries[0] = Entry.Default;

            for( int i = 0; i < allObjects.Length; i++ )
            {
                Object obj = allObjects[i];

                entries[i + 1] = new Entry() { Identifier = obj.name, Obj = obj };
            }

            return entries.ToArray();
        }
    }
}
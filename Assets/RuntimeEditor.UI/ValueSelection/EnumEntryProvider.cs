using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RuntimeEditor.UI.ValueSelection
{
    public class EnumEntryProvider : IEntryProvider
    {
        public Entry[] GetAllEntries( Type type )
        {
            //Enum.GetValues( graphNode.GetInstanceType() ).Cast<object>()
            Array values = Enum.GetValues( type );

            Entry[] entries = new Entry[values.Length];
            int i = 0;
            foreach( var value in values )
            {
                entries[i] = new Entry()
                {
                    Identifier = Enum.GetName( type, value ),
                    Obj = value
                };
                i++;
            }

            return entries;
        }
    }
}
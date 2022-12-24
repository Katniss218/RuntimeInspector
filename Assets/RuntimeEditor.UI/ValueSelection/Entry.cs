using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeEditor.UI.ValueSelection
{
    public struct Entry
    {
        public string Identifier { get; set; }
        public object Obj { get; set; }

        public bool IsDefault => Identifier == null;

        /// <summary>
        /// A special case when the entry represents the default value.
        /// </summary>
        public static Entry Default => new Entry() { Identifier = null, Obj = null };
    }
}
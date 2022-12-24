using RuntimeEditor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeEditor.UI
{
    public static class ObjectGraphNode_Ex
    {
        public static string GetDisplayName( this ObjectGraphNode graphNode )
        {
            return graphNode.Name;
        }
    }
}
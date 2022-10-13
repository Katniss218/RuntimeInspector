using RuntimeInspector.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RuntimeInspector.UI.Drawers
{
    public class Int32Drawer : TypedDrawer<int>
    {
        public override RectTransform Draw( int binding )
        {
            //throw new NotImplementedException();
            Debug.Log( $"Int Drawer - {binding}" );
            return null;
        }
    }
}

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
        public override RectTransform Draw( RectTransform parent, IMemberBinding<int> binding )
        {
            (RectTransform root, _, _) = DrawerUtils.MakeInputField( parent, binding );

            return root;
        }

        public override int InputToValue( object input )
        {
            return int.Parse( (string)input );
        }
    }
}
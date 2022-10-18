using RuntimeInspector.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RuntimeInspector.UI.Drawers
{
    public class StringDrawer : TypedDrawer<string>
    {
        public override RectTransform Draw( RectTransform parent, IMemberBinding<string> binding )
        {
            (RectTransform root, _, _) = DrawerUtils.MakeInputField( parent, binding );

            return root;
        }

        public override string InputToValue( object input )
        {
            return (string)input;
        }
    }
}
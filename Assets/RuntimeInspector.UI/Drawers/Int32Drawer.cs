using RuntimeInspector.Core;
using RuntimeInspector.UI.GUIUtils;
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
        public override RectTransform Draw( RectTransform parent, MemberBinding binding, InspectorStyle style )
        {
            (RectTransform root, _, _) = InspectorInputField.Create( parent, binding, style );

            return root;
        }

        public override int InputToValue( object input )
        {
            return int.Parse( (string)input );
        }
    }
}
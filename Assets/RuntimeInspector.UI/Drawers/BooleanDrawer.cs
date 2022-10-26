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
    [DrawerOf( typeof( bool ) )]
    public class BooleanDrawer : Drawer
    {
        public override RectTransform Draw( RectTransform parent, MemberBinding binding, InspectorStyle style )
        {
            RectTransform root = InspectorInputField.Create( parent, binding, style );

            return root;
        }
    }
}
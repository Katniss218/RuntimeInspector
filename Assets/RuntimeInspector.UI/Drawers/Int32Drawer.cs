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
        public override RectTransform Draw( RectTransform parent, string name, int value )
        {
            (RectTransform root, TMPro.TextMeshProUGUI labelText, TMPro.TextMeshProUGUI valueText) = DrawerUtils.MakeInputField( name, UnderlyingType.FullName, parent, $"{value}" );

            return root;
        }
    }
}
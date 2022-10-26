using RuntimeInspector.Core;
using RuntimeInspector.UI.GUIUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace RuntimeInspector.UI
{
    public abstract class Drawer
    {
        public List<Drawer> ChildDrawers { get; set; }

        public abstract RectTransform Draw( RectTransform parent, MemberBinding binding, InspectorStyle style );
    }
}
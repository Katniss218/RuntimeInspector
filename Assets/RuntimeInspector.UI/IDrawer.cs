using RuntimeInspector.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RuntimeInspector.UI
{
    public interface IDrawer
    {
        RectTransform Draw( RectTransform parent, IMemberBinding binding );
    }

    public abstract class TypedDrawer<T> : IDrawer
    {
        protected static Type UnderlyingType = typeof( T );

        public RectTransform Draw( RectTransform parent, IMemberBinding binding )
        {
            RectTransform uiObj = Draw( parent, binding.DisplayName, (T)binding.GetValue() );

            return uiObj;
        }

        public abstract RectTransform Draw( RectTransform parent, string name, T value );
    }

    public class GenericDrawer : IDrawer
    {
        public RectTransform Draw( RectTransform parent, IMemberBinding binding )
        {
            (RectTransform root, TMPro.TextMeshProUGUI labelText, TMPro.TextMeshProUGUI valueText) = DrawerUtils.MakeInputField( binding.DisplayName, binding.Type.FullName, parent, $"{binding.GetValue()}" );

            return root;
        }
    }
}
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
        RectTransform Draw( IMemberBinding binding );
    }

    public abstract class TypedDrawer<T> : IDrawer
    {
        public RectTransform Draw( IMemberBinding binding )
        {
            RectTransform uiObj = Draw( (T)binding.GetValue() );

            return uiObj;
        }

        public abstract RectTransform Draw( T value );
    }

    public class GenericDrawer : IDrawer
    {
        public RectTransform Draw( IMemberBinding binding )
        {
            //throw new NotImplementedException();
            Debug.Log( $"Generic Drawer - {binding}" );
            return null;
        }
    }
}
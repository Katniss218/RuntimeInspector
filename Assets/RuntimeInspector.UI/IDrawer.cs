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

        /// <summary>
        /// Inverse of draw. Converts user input to a value.
        /// </summary>
        object InputToValueGeneral( string input ); // doesn't necessarily have to be a string.
    }

    public abstract class TypedDrawer<T> : IDrawer
    {
        protected static Type UnderlyingType = typeof( T );

        public RectTransform Draw( RectTransform parent, IMemberBinding binding )
        {
            RectTransform uiObj = Draw( parent, (IMemberBinding<T>)binding );

            return uiObj;
        }

        public object InputToValueGeneral( string input )
        {
            return InputToValue( input );
        }

        public abstract RectTransform Draw( RectTransform parent, IMemberBinding<T> binding );

        /// <summary>
        /// Converts user input (input field) into the value that will be assigned.
        /// </summary>
        public abstract T InputToValue( string input );
    }

    public class GenericDrawer : IDrawer
    {
        public RectTransform Draw( RectTransform parent, IMemberBinding binding )
        {
            (RectTransform root, _, _) = DrawerUtils.MakeInputField( parent, binding );

            return root;
        }

        public object InputToValueGeneral( string input )
        {
            throw new InvalidOperationException( "Can't use generic drawer to convert string into object" );
        }
    }
}
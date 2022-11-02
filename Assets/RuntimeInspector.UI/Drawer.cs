using RuntimeInspector.Core;
using RuntimeInspector.UI.Drawers;
using RuntimeInspector.UI.GUIUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace RuntimeInspector.UI
{
    public abstract class Drawer
    {
        public abstract (RectTransform, UIBinding) Draw( RectTransform parent, MemberBinding binding, InspectorStyle style );
  
        public static (bool destroyOld, bool createNew, UIBinding uiBinding) GetRedrawMode( MemberBinding binding )
        {
            bool destroyOld = false;
            bool createNew = false;

            UIBinding drawnBinding = UIBinding.Find( binding );

            if( drawnBinding != null && drawnBinding.IsStale )
            {
                if( drawnBinding.Root == null )
                {
                    Debug.LogWarning( $"UIBinding.Root for Binding '{binding.Metadata.Name}' was null." );
                }

                destroyOld = true;
            }
            if( drawnBinding == null || drawnBinding.IsStale )
            {
                createNew = true;
            }

            return (destroyOld, createNew, drawnBinding);
        }
    }
}
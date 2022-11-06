using RuntimeInspector.Core;
using RuntimeInspector.UI.Attributes;
using RuntimeInspector.UI.Drawers;
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
    public struct RedrawData
    {
        public bool DestroyOld { get; set; }
        public bool CreateNew { get; set; }
        public UIObjectGraphBinding Binding { get; set; }

        public bool Hidden { get; set; }

        public RedrawData( bool destroyOld, bool createNew, UIObjectGraphBinding binding, bool hidden )
        {
            this.DestroyOld = destroyOld;
            this.CreateNew = createNew;
            this.Binding = binding;
            this.Hidden = hidden;
        }

        public static RedrawData GetRedrawData( ObjectGraphNode node )
        {
            if( node.GetAttributes<HideAttribute>().FirstOrDefault() != null )
            {
                return new RedrawData( false, false, null, true );
            }

            bool destroyOld = false;
            bool createNew = false;

            UIObjectGraphBinding drawnBinding = UIObjectGraphBinding.Find( node );

            // we should redraw if the value changed, or if the value isn't drawn at all.
            // we should remove the previous value if it changed, and is drawn.

            bool isDisplayedValueStale = false;
            if( drawnBinding != null )
            {
                if( node.CanRead )
                {
                    object displayedValue = drawnBinding.CurrentValue;
                    object newValue = node.GetValue();
                    isDisplayedValueStale = !displayedValue?.Equals( newValue ) ?? newValue == null;
                }
                else
                {
                    isDisplayedValueStale = false;
                }

                if( isDisplayedValueStale )
                {
                    if( drawnBinding.Root == null )
                    {
                        Debug.LogWarning( $"UIBinding.Root for Binding '{node.Name}' was null." );
                    }

                    destroyOld = true;
                }
            }

            if( drawnBinding == null || isDisplayedValueStale )
            {
                createNew = true;
            }

            return new RedrawData( destroyOld, createNew, drawnBinding, false );
        }
    }

    public abstract class Drawer
    {
        public abstract (RectTransform root, UIObjectGraphBinding) Draw( RectTransform parent, ObjectGraphNode binding, InspectorStyle style );
    }
}
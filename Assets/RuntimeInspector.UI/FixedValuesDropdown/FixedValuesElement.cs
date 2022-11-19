using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace RuntimeInspector.UI.FixedValuesDropdown
{
    public class FixedValuesElement : MonoBehaviour, IPointerClickHandler
    {
        public object Value { get; internal set; }

        public FixedValuesDropdownWindow Window { get; internal set; }

        public void SelectMe()
        {
            Window.Submit( this.Value );
        }

        public void OnPointerClick( PointerEventData e )
        {
            SelectMe();
        }
    }
}
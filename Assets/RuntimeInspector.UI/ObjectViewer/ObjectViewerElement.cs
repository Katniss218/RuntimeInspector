using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace RuntimeInspector.UI.ObjectViewer
{
    /// <summary>
    /// Represents a single element that shows up in the results list of the <see cref="ObjectViewerWindow"/>
    /// </summary>
    public class ObjectViewerElement : MonoBehaviour, IPointerClickHandler
    {
        public Object Obj { get; internal set; }

        public ObjectViewerWindow Window { get; internal set; }

        public void SelectMe()
        {
            Window.Select( this.Obj );
        }

        public void OnPointerClick( PointerEventData e )
        {
            SelectMe();
        }
    }
}
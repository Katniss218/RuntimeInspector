using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace RuntimeInspector.UI.AssetViewer
{
    /// <summary>
    /// Represents a single element that shows up in the results list of the <see cref="ObjectViewerWindow"/>
    /// </summary>
    public class AssetViewerElement : MonoBehaviour, IPointerClickHandler
    {
        public object Value { get; internal set; }

        public AssetViewerWindow Window { get; internal set; }

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
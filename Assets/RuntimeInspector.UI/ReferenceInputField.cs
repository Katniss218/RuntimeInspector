using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RuntimeInspector.UI
{
    public class ReferenceInputField : MonoBehaviour, IPointerClickHandler
    {
        public event Action<object> onSubmit;

        public Type Type { get; set; }

        public void OnSubmit( Type type, object obj )
        {
            onSubmit?.Invoke( obj );
        }

        public void OnPointerClick( PointerEventData eventData )
        {
            ObjectViewer.ObjectViewerWindow v = ObjectViewer.ObjectViewerWindow.Create( GameObject.Find( "ModalCanvas" ).transform, Type );
            v.onSubmit += this.OnSubmit;
        }
    }
}

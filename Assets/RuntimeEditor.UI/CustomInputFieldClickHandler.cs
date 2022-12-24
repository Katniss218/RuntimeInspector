using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RuntimeEditor.UI
{
    public class CustomInputFieldClickHandler : MonoBehaviour, IPointerClickHandler
    {
        public event Action<object> onSubmit;

        public Action<PointerEventData> OnClickFunc;

        public Type Type { get; set; }

        public void OnSubmit( Type type, object obj )
        {
            onSubmit?.Invoke( obj );
        }

        public void OnPointerClick( PointerEventData eventData )
        {
            OnClickFunc?.Invoke(eventData);
        }
    }
}
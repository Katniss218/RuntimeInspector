using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RuntimeEditor.UI
{
    public class PointerClickHandler : MonoBehaviour, IPointerClickHandler
    {
        public Action<PointerEventData> OnClickFunc;

        public void OnPointerClick( PointerEventData eventData )
        {
            OnClickFunc?.Invoke(eventData);
        }
    }
}
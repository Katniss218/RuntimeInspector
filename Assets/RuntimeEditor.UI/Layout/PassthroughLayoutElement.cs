using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RuntimeEditor.UI.Layout
{
    public class PassthroughLayoutElement : MonoBehaviour, ILayoutElement
    {
        private RectTransform _childTransform;
        private ILayoutElement _child;

        public ILayoutElement GetChild()
        {
            if( _childTransform == null || _childTransform.parent != this.transform )
            {
                ResetChild();
            }

            return _child;
        }

        private void ResetChild()
        {
            RectTransform childTransform = (RectTransform)this.transform.GetChild( 0 );

            this._childTransform = childTransform;
            this._child = childTransform.GetComponent<ILayoutElement>();
        }

        public float minWidth => GetChild()?.minWidth ?? 0;

        public float preferredWidth => GetChild()?.preferredWidth ?? 0;

        public float flexibleWidth => GetChild()?.flexibleWidth ?? 0;

        public float minHeight => GetChild()?.minHeight ?? 0;

        public float preferredHeight => GetChild()?.preferredHeight ?? 0;

        public float flexibleHeight => GetChild()?.flexibleHeight ?? 0;

        public int layoutPriority => GetChild()?.layoutPriority ?? 0;

        public void CalculateLayoutInputHorizontal()
        {
            GetChild()?.CalculateLayoutInputHorizontal();
        }

        public void CalculateLayoutInputVertical()
        {
            GetChild()?.CalculateLayoutInputVertical();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace RuntimeInspector.UI.GUIUtils
{
    public static class InspectorVerticalList
    {
        public static RectTransform Create( RectTransform parent, InspectorStyle style )
        {
            GameObject gameObject = new GameObject( $"_list" );
            gameObject.layer = 5;

            RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.SetParent( parent );
            rectTransform.anchorMin = new Vector2( 0.0f, 0.5f );
            rectTransform.anchorMax = new Vector2( 1.0f, 0.5f );
            rectTransform.pivot = new Vector2( 0.5f, 0.5f );
            rectTransform.anchoredPosition = new Vector2( 0.0f, 0.0f );
            rectTransform.sizeDelta = new Vector2( 0.0f, 200.0f );

            VerticalLayoutGroup layoutGroup = gameObject.AddComponent<VerticalLayoutGroup>();
            layoutGroup.padding = new RectOffset( style.IndentWidth, 0, style.IndentMargin, style.IndentMargin );
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = false; 
            layoutGroup.childScaleWidth = false;
            layoutGroup.childScaleHeight = false;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = false;

            ContentSizeFitter contentSizeFitter = gameObject.AddComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            return rectTransform;
        }
    }
}

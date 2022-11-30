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
        public struct Params
        {
            public bool IncludeMargin { get; set; }
        }

        public static RectTransform Find( string identifier, RectTransform parent )
        {
#warning TODO - finding stuff is kinda ugly. And it's not available in other helper methods.
            if( parent == null )
            {
                return null;
            }

            for( int i = 0; i < parent.childCount; i++ )
            {
                Transform transform = parent.GetChild( i );
                if( transform.gameObject.name == $"${identifier}" )
                {
                    return (RectTransform)transform;
                }
            }
            return null;
        }

        public static RectTransform Create( string identifier, RectTransform parent, InspectorStyle style, Params param )
        {
            GameObject gameObject = new GameObject( $"${identifier}" );
            gameObject.layer = 5;

            RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.SetParent( parent );
            rectTransform.anchorMin = new Vector2( 0.0f, 0.5f );
            rectTransform.anchorMax = new Vector2( 1.0f, 0.5f );
            rectTransform.pivot = new Vector2( 0.5f, 0.5f );
            rectTransform.anchoredPosition = new Vector2( 0.0f, 0.0f );
            rectTransform.sizeDelta = new Vector2( 0.0f, 200.0f );

            VerticalLayoutGroup layoutGroup = gameObject.AddComponent<VerticalLayoutGroup>();
            if( param.IncludeMargin )
            {
                layoutGroup.padding = new RectOffset( style.IndentWidth, 0, style.IndentMargin, style.IndentMargin );
            }
            else
            {
                layoutGroup.padding = new RectOffset( 0, 0, 0, 0 );
            }
            layoutGroup.spacing = style.Spacing;
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

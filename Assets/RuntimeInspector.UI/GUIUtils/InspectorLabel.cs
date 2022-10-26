using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RuntimeInspector.UI.GUIUtils
{
    public static class InspectorLabel
    {
        public static RectTransform Create( RectTransform parent, string text, InspectorStyle style )
        {
            GameObject gameObject = new GameObject( $"_label" );
            gameObject.layer = 5;

            RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.SetParent( parent );

            rectTransform.anchorMin = new Vector2( 0.0f, 0.5f );
            rectTransform.anchorMax = new Vector2( 0.0f, 0.5f );
            rectTransform.pivot = new Vector2( 0.0f, 0.5f );
            rectTransform.anchoredPosition = new Vector2( 0.0f, 0.0f );
            rectTransform.sizeDelta = new Vector2( 200.0f, style.FieldHeight );

            TMPro.TextMeshProUGUI labelText = gameObject.AddComponent<TMPro.TextMeshProUGUI>();
            labelText.fontSize = style.FontSize;
            labelText.alignment = TMPro.TextAlignmentOptions.Left;
            labelText.overflowMode = TMPro.TextOverflowModes.Overflow;
            labelText.color = style.LabelTextColor;

            labelText.text = text;

            return rectTransform;
        }
    }
}
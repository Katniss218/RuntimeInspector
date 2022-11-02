using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

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

        public static RectTransform Create( RectTransform parent, Sprite typeIcon, string text, InspectorStyle style )
        {
            GameObject gameObject = new GameObject( $"_label" );
            gameObject.layer = 5;

            RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.SetParent( parent );

            rectTransform.anchorMin = new Vector2( 0.0f, 0.0f );
            rectTransform.anchorMax = new Vector2( 0.5f, 1.0f );
            rectTransform.pivot = new Vector2( 0.0f, 0.5f );
            rectTransform.anchoredPosition = new Vector2( 0.0f, 0.0f );
            rectTransform.sizeDelta = new Vector2( 0.0f, style.FieldHeight );

            GameObject iconGameObject = new GameObject( $"icon" );
            iconGameObject.layer = 5;

            RectTransform iconTransform = iconGameObject.AddComponent<RectTransform>();
            iconTransform.SetParent( rectTransform );

            iconTransform.anchorMin = new Vector2( 0.0f, 0.5f );
            iconTransform.anchorMax = new Vector2( 0.0f, 0.5f );
            iconTransform.pivot = new Vector2( 0.0f, 0.5f );
            iconTransform.anchoredPosition = new Vector2( 0.0f, 0.0f );
            iconTransform.sizeDelta = new Vector2( style.TypeIconSize, style.TypeIconSize );

            Image typeImage = iconGameObject.AddComponent<Image>();
            typeImage.sprite = typeIcon;

            GameObject textGameObject = new GameObject( $"text" );
            textGameObject.layer = 5;

            RectTransform textTransform = textGameObject.AddComponent<RectTransform>();
            textTransform.SetParent( rectTransform );

            textTransform.anchorMin = new Vector2( 0.0f, 0.0f );
            textTransform.anchorMax = new Vector2( 1.0f, 1.0f );
            textTransform.pivot = new Vector2( 0.5f, 0.5f );
            textTransform.anchoredPosition = new Vector2( (style.TypeIconSize + style.TypeIconMargin) / 2, 0.0f );
            textTransform.sizeDelta = new Vector2( -(style.TypeIconSize + style.TypeIconMargin), 0.0f );

            TMPro.TextMeshProUGUI labelText = textGameObject.AddComponent<TMPro.TextMeshProUGUI>();
            labelText.fontSize = style.FontSize;
            labelText.alignment = TMPro.TextAlignmentOptions.Left;
            labelText.overflowMode = TMPro.TextOverflowModes.Overflow;
            labelText.color = style.LabelTextColor;

            labelText.text = text;

            return rectTransform;
        }
    }
}
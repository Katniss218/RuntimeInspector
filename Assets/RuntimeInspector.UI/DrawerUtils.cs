using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace RuntimeInspector.UI
{
    public static class DrawerUtils
    {
        const float FONT_SIZE = 12.0f;
        const float FIELD_HEIGHT = 24.0f;

        static readonly Color INPUT_FIELD_COLOR = new Color( 0.5f, 0.5f, 0.5f );
        static readonly Color LABEL_TEXT_COLOR = new Color( 1.0f, 1.0f, 1.0f );
        static readonly Color VALUE_TEXT_COLOR = new Color( 1.0f, 1.0f, 1.0f );

        public static (RectTransform root, TMPro.TextMeshProUGUI label, TMPro.TextMeshProUGUI value) MakeInputField( string name, string typeName, RectTransform parent, string value )
        {
            GameObject root = new GameObject( $"{name} ({typeName})" );
            root.layer = 5;
            RectTransform rootTransform = root.AddComponent<RectTransform>();

            rootTransform.SetParent( parent );
            rootTransform.sizeDelta = new Vector2( 0.0f, FIELD_HEIGHT );


            GameObject labelGO = new GameObject( $"_label" );
            labelGO.layer = 5;
            RectTransform labelTransform = labelGO.AddComponent<RectTransform>();

            labelTransform.SetParent( rootTransform );

            labelTransform.anchorMin = new Vector2( 0.0f, 0.5f );
            labelTransform.anchorMax = new Vector2( 0.0f, 0.5f );
            labelTransform.pivot = new Vector2( 0.0f, 0.5f );
            labelTransform.anchoredPosition = new Vector2( 0.0f, 0.0f );
            labelTransform.sizeDelta = new Vector2( 200.0f, FIELD_HEIGHT );

            TMPro.TextMeshProUGUI labelText = labelGO.AddComponent<TMPro.TextMeshProUGUI>();
            labelText.fontSize = FONT_SIZE;
            labelText.alignment = TMPro.TextAlignmentOptions.Left;
            labelText.overflowMode = TMPro.TextOverflowModes.Overflow;
            labelText.color = LABEL_TEXT_COLOR;


            GameObject valueGO = new GameObject( $"_value" );
            valueGO.layer = 5;
            RectTransform valueTransform = valueGO.AddComponent<RectTransform>();

            valueTransform.SetParent( rootTransform );

            valueTransform.anchorMin = new Vector2( 1.0f, 0.5f );
            valueTransform.anchorMax = new Vector2( 1.0f, 0.5f );
            valueTransform.pivot = new Vector2( 1.0f, 0.5f );
            valueTransform.anchoredPosition = new Vector2( 0.0f, 0.0f );
            valueTransform.sizeDelta = new Vector2( 200.0f, FIELD_HEIGHT );

            Image image = valueGO.AddComponent<Image>();
            image.color = INPUT_FIELD_COLOR;


            GameObject valueTextAreaGO = new GameObject( "_textarea" );
            valueTextAreaGO.layer = 5;
            RectTransform valueTextAreaTransform = valueTextAreaGO.AddComponent<RectTransform>();

            valueTextAreaTransform.SetParent( valueTransform );
            valueTextAreaTransform.anchorMin = new Vector2( 0.0f, 0.0f );
            valueTextAreaTransform.anchorMax = new Vector2( 1.0f, 1.0f );
            valueTextAreaTransform.pivot = new Vector2( 0.5f, 0.5f );
            valueTextAreaTransform.anchoredPosition = new Vector2( 0.0f, 0.0f );
            valueTextAreaTransform.sizeDelta = new Vector2( 0.0f, 0.0f );

            RectMask2D mask = valueTextAreaGO.AddComponent<RectMask2D>();
            mask.padding = new Vector4( -8, -5, -8, -5 );


            GameObject valueTextGO = new GameObject( "_text" );
            valueTextGO.layer = 5;
            RectTransform valueTextTransform = valueTextGO.AddComponent<RectTransform>();

            valueTextTransform.SetParent( valueTextAreaTransform );

            valueTextTransform.anchorMin = new Vector2( 0.0f, 0.0f );
            valueTextTransform.anchorMax = new Vector2( 1.0f, 1.0f );
            valueTextTransform.pivot = new Vector2( 0.5f, 0.5f );
            valueTextTransform.anchoredPosition = new Vector2( 0.0f, 0.0f );
            valueTextTransform.sizeDelta = new Vector2( 0.0f, 0.0f );

            TMPro.TextMeshProUGUI valueText = valueTextGO.AddComponent<TMPro.TextMeshProUGUI>();
            valueText.fontSize = FONT_SIZE;
            valueText.alignment = TMPro.TextAlignmentOptions.Right;
            valueText.overflowMode = TMPro.TextOverflowModes.Overflow;
            valueText.color = VALUE_TEXT_COLOR;


            labelText.text = name;

            valueText.text = value;

            InputSubmitter submitter = valueGO.AddComponent<InputSubmitter>();
            //submitter

            TMPro.TMP_InputField valueInput = valueGO.AddComponent<TMPro.TMP_InputField>();

            valueInput.textViewport = valueTextAreaTransform;
            valueInput.textComponent = valueText;
            valueInput.fontAsset = valueText.font;
            valueInput.pointSize = FONT_SIZE;

            valueInput.text = value;

            valueInput.enabled = false;
            valueInput.enabled = true; // regenerate the caret.

            return (rootTransform, labelText, valueText);
        }
    }
}

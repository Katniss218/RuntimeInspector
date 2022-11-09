﻿using RuntimeInspector.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace RuntimeInspector.UI.GUIUtils
{
    /// <summary>
    /// Helper class to create GUI input fields.
    /// </summary>
    public static class InspectorTextInputField
    {
        public static readonly string READONLY_PLACEHOLDER = string.Empty;

        public static RectTransform Create( RectTransform parent, ObjectGraphNodeUI uiBinding, ObjectGraphNode binding, InspectorStyle style )
        {
            GameObject valueGO = new GameObject( $"_value" );
            valueGO.layer = 5;
            RectTransform valueTransform = valueGO.AddComponent<RectTransform>();

            valueTransform.SetParent( parent );

            valueTransform.anchorMin = new Vector2( 1.0f, 0.5f );
            valueTransform.anchorMax = new Vector2( 1.0f, 0.5f );
            valueTransform.pivot = new Vector2( 1.0f, 0.5f );
            valueTransform.anchoredPosition = new Vector2( 0.0f, 0.0f );
            valueTransform.sizeDelta = new Vector2( 200.0f, style.FieldHeight );

            Image image = valueGO.AddComponent<Image>();
            image.color = style.InputFieldColor;


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
            valueTextTransform.sizeDelta = new Vector2( style.InputFieldMargin * -2.0f, style.InputFieldMargin * -2.0f );

            TMPro.TextMeshProUGUI valueText = valueTextGO.AddComponent<TMPro.TextMeshProUGUI>();
            valueText.fontSize = style.FontSize;
            valueText.alignment = TMPro.TextAlignmentOptions.Right;
            valueText.overflowMode = TMPro.TextOverflowModes.Overflow;
            valueText.color = style.ValueTextColor;

            if( binding.CanRead )
            {
                valueText.text = binding.GetValue().ToString();
            }
            else
            {
                valueText.text = READONLY_PLACEHOLDER;
            }

            if( binding.CanWrite )
            {
                TMPro.TMP_InputField valueInput = valueGO.AddComponent<TMPro.TMP_InputField>();

                valueInput.textViewport = valueTextAreaTransform;
                valueInput.textComponent = valueText;
                valueInput.fontAsset = valueText.font;
                valueInput.pointSize = style.FontSize;
                valueInput.restoreOriginalTextOnEscape = true;

                if( binding.CanRead )
                {
                    valueInput.text = binding.GetValue().ToString();
                }
                else
                {
                    valueInput.text = READONLY_PLACEHOLDER;
                }

                valueInput.RegenerateCaret();

                uiBinding.InputField = valueInput;
                valueInput.onSubmit.AddListener( uiBinding.SetValueText );
            }
            if( !binding.CanWrite && binding.CanRead )
            {
                image.color = style.InputFieldColorReadonly;
            }
            if( binding.CanWrite && !binding.CanRead )
            {
                image.color = style.InputFieldColorWriteonly;
            }

            return valueTransform;
        }
    }
}

using RuntimeInspector.Core;
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
    public static class InspectorInputField
    {
        public static RectTransform Create( RectTransform parent, MemberBinding binding, InspectorStyle style )
        {
            GameObject root = new GameObject( $"{binding.Metadata.Name} ({binding.Binding.GetInstanceType().FullName})" );
            root.layer = 5;
            RectTransform rootTransform = root.AddComponent<RectTransform>();

            rootTransform.SetParent( parent );
            rootTransform.anchorMin = new Vector2( 0.0f, 0.5f );
            rootTransform.anchorMax = new Vector2( 1.0f, 0.5f );
            rootTransform.pivot = new Vector2( 0.5f, 0.5f );
            rootTransform.anchoredPosition = new Vector2( 0.0f, 0.0f );
            rootTransform.sizeDelta = new Vector2( 0.0f, style.FieldHeight );

            GameObject labelGO = new GameObject( $"_label" );
            labelGO.layer = 5;
            RectTransform labelTransform = labelGO.AddComponent<RectTransform>();

            labelTransform.SetParent( rootTransform );

            labelTransform.anchorMin = new Vector2( 0.0f, 0.5f );
            labelTransform.anchorMax = new Vector2( 0.0f, 0.5f );
            labelTransform.pivot = new Vector2( 0.0f, 0.5f );
            labelTransform.anchoredPosition = new Vector2( 0.0f, 0.0f );
            labelTransform.sizeDelta = new Vector2( 200.0f, style.FieldHeight );

            TMPro.TextMeshProUGUI labelText = labelGO.AddComponent<TMPro.TextMeshProUGUI>();
            labelText.fontSize = style.FontSize;
            labelText.alignment = TMPro.TextAlignmentOptions.Left;
            labelText.overflowMode = TMPro.TextOverflowModes.Overflow;
            labelText.color = style.LabelTextColor;


            GameObject valueGO = new GameObject( $"_value" );
            valueGO.layer = 5;
            RectTransform valueTransform = valueGO.AddComponent<RectTransform>();

            valueTransform.SetParent( rootTransform );

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
            valueTextTransform.sizeDelta = new Vector2( 0.0f, 0.0f );

            TMPro.TextMeshProUGUI valueText = valueTextGO.AddComponent<TMPro.TextMeshProUGUI>();
            valueText.fontSize = style.FontSize;
            valueText.alignment = TMPro.TextAlignmentOptions.Right;
            valueText.overflowMode = TMPro.TextOverflowModes.Overflow;
            valueText.color = style.ValueTextColor;


            labelText.text = binding.Metadata.Name;

            if( binding.Metadata.CanRead )
            {
                valueText.text = binding.Binding.GetValue().ToString();
            }
            else
            {
                valueText.text = "<Can't read>";
            }

            if( binding.Metadata.CanWrite )
            {
                TMPro.TMP_InputField valueInput = valueGO.AddComponent<TMPro.TMP_InputField>();

                valueInput.textViewport = valueTextAreaTransform;
                valueInput.textComponent = valueText;
                valueInput.fontAsset = valueText.font;
                valueInput.pointSize = style.FontSize;

                if( binding.Metadata.CanRead )
                {
                    valueInput.text = binding.Binding.GetValue().ToString();
                }

                valueInput.enabled = false;
                valueInput.enabled = true; // regenerate the caret.
                
                InspectorValue submitter = root.AddComponent<InspectorValue>();
                submitter.InputField = valueInput;
                submitter.Binding = binding;
                submitter.Parent = parent;
                submitter.Style = style;
                submitter.Root = rootTransform;
                valueInput.onSubmit.AddListener( submitter.UpdateValue );
            }
            else
            {
                image.color = style.InputFieldColorReadonly;
            }

            return rootTransform;
        }
    }
}

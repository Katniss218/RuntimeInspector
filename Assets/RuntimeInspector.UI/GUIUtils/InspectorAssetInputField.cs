using RuntimeInspector.Core;
using RuntimeInspector.UI.Inspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace RuntimeInspector.UI.GUIUtils
{
    /// <summary>
    /// Helper class to create GUI input fields.
    /// </summary>
    public static class InspectorAssetInputField
    {
        /// <summary>
        /// The placeholder display to be used with write-only properties.
        /// </summary>
        public static readonly string WRITEONLY_PLACEHOLDER = string.Empty;

        /// <summary>
        /// Creates a text input field and binds it to a graph node UI.
        /// </summary>
        public static RectTransform Create( RectTransform parent, GraphNodeUI existingGraphNodeUI, ObjectGraphNode graphNode, InspectorStyle style )
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

            TMPro.TextMeshProUGUI text = valueTextGO.AddComponent<TMPro.TextMeshProUGUI>();
            text.fontSize = style.FontSize;
            text.alignment = TMPro.TextAlignmentOptions.Right;
            text.overflowMode = TMPro.TextOverflowModes.Overflow;
            text.color = style.ValueTextColor;
            text.font = style.Font;

            if( graphNode.CanRead )
            {
                object value = graphNode.GetValue();
                if( Utils.UnityUtils.IsUnityNull( value ) )
                {
                    text.text = "< null >";
                }
                else
                {
                    text.text = value.ToString();
                }
            }
            else
            {
                text.text = WRITEONLY_PLACEHOLDER;
            }

            if( graphNode.CanWrite )
            {
                // string cachedText = text.text;

                GenericClickHandler inputField = valueGO.AddComponent<GenericClickHandler>();
                inputField.Type = graphNode.Type;
                inputField.onSubmit += existingGraphNodeUI.SetValue;
                inputField.OnClickFunc = ( eventData ) =>
                {
                    AssetViewer.AssetViewerWindow window = AssetViewer.AssetViewerWindow.Create( GameObject.Find( "ModalCanvas" ).transform, inputField.Type );
                    window.onSubmit += inputField.OnSubmit;
                    // window.CloseOnSubmit = true;
                    existingGraphNodeUI.onSetterInvalidated += window.Close;
                    window.onSubmit += ( t, o ) =>
                    {
                        existingGraphNodeUI.onSetterInvalidated -= window.Close;
                    };
                    /* window.onSubmit += inputField.OnSubmit;
                     window.CloseOnSubmit = true;
                     existingGraphNodeUI.onDestroy += window.Close;
                     window.onSubmit += ( t, o ) => { existingGraphNodeUI.onDestroy -= window.Close; };*/
                };

                // technically unneeded because a reference is assigned instantaneously.
                //inputField.onSelect.AddListener( ( e ) => existingGraphNodeUI.SetSelected() );
                //inputField.onDeselect.AddListener( ( e ) => existingGraphNodeUI.SetSelected() );
            }
            if( !graphNode.CanWrite && graphNode.CanRead )
            {
                image.color = style.InputFieldColorReadonly;
            }
            if( graphNode.CanWrite && !graphNode.CanRead )
            {
                image.color = style.InputFieldColorWriteonly;
            }

            return valueTransform;
        }
    }
}
using RuntimeInspector.Core;
using RuntimeInspector.UI.Inspector;
using RuntimeInspector.UI.ValueSelection;
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
    /// A helper class to create GUI input fields.
    /// </summary>
    public static class InspectorInputField
    {
        /// <summary>
        /// The placeholder display to be used with write-only properties.
        /// </summary>
        public const string WRITEONLY_PLACEHOLDER = "";

        /// <summary>
        /// Creates a text input field, and binds it to a graph node UI.
        /// </summary>
        public static RectTransform Create( RectTransform parent, GraphNodeUI existingGraphNodeUI, ObjectGraphNode graphNode, InspectorStyle style )
        {
            return CreateInternal( parent, existingGraphNodeUI, graphNode, style, null );
        }

        /// <summary>
        /// Creates an input field that will open a value selection window, and binds it to a graph node UI.
        /// </summary>
        public static RectTransform Create( RectTransform parent, GraphNodeUI existingGraphNodeUI, ObjectGraphNode graphNode, IEntryProvider entryProvider, InspectorStyle style )
        {
            return CreateInternal( parent, existingGraphNodeUI, graphNode, style, entryProvider );
        }

        /// <param name="entryProvider">If this is set, it will create an input field that opens a value selection window on click.</param>
        private static RectTransform CreateInternal( RectTransform parent, GraphNodeUI existingGraphNodeUI, ObjectGraphNode graphNode, InspectorStyle style, IEntryProvider entryProvider )
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
            text.raycastTarget = false;

            if( graphNode.CanRead )
            {
                object value = graphNode.GetValue();
                if( entryProvider == null )
                {
                    if( Converter.TryConvertReverse( graphNode.GetInstanceType(), typeof( string ), value, out object converted ) )
                    {
                        text.text = (string)converted;
                    }
                    else
                    {
                        text.text = "$FAIL";
                        Debug.LogWarning( $"Couldn't convert value '{value}' of type '{graphNode.Type.FullName}' into type '{typeof( string ).FullName}'" );
                    }
                }
                else
                {
                    if( Utils.UnityUtils.IsUnityNull( value ) )
                    {
                        text.text = "< null >";
                    }
                    else
                    {
                        text.text = value.ToString();
                    }
                }
            }
            else
            {
                text.text = WRITEONLY_PLACEHOLDER;
            }

            if( graphNode.CanWrite )
            {
                if( entryProvider == null )
                {
                    string cachedText = text.text; // Temporary variable because adding TMPro.TMP_InputField clears the 'valueText.text'.

                    TMPro.TMP_InputField inputField = valueGO.AddComponent<TMPro.TMP_InputField>();
                    inputField.textViewport = valueTextAreaTransform;
                    inputField.textComponent = text;
                    inputField.fontAsset = text.font;
                    inputField.pointSize = style.FontSize;
                    inputField.restoreOriginalTextOnEscape = true;
                    inputField.fontAsset = style.Font;

                    inputField.text = cachedText;

                    inputField.RegenerateCaret();

                    inputField.onSubmit.AddListener( existingGraphNodeUI.SetValue );
                    inputField.onSelect.AddListener( ( e ) => existingGraphNodeUI.SetSelected() );
                    inputField.onDeselect.AddListener( ( e ) => existingGraphNodeUI.SetSelected() );
                }
                else
                {
                    CustomInputFieldClickHandler inputField = valueGO.AddComponent<CustomInputFieldClickHandler>();
                    inputField.Type = graphNode.Type;
                    inputField.onSubmit += existingGraphNodeUI.SetValue;
                    inputField.OnClickFunc = ( eventData ) =>
                    {
                        ValueSelectionWindow window = ValueSelectionWindow.Create( GameObject.Find( "ModalCanvas" ).transform, inputField.Type, entryProvider );
                        window.onSubmit += inputField.OnSubmit;
                        existingGraphNodeUI.onSetterInvalidated += window.Close;
                        window.onClose += () =>
                        {
                            existingGraphNodeUI.onSetterInvalidated -= window.Close;
                        };
                    };
                    // technically unneeded because a reference is assigned instantaneously.
                    //inputField.onSelect.AddListener( ( e ) => existingGraphNodeUI.SetSelected() );
                    //inputField.onDeselect.AddListener( ( e ) => existingGraphNodeUI.SetSelected() );
                }
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
using RuntimeInspector.Core.AssetManagement;
using RuntimeInspector.UI.GUIUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace RuntimeInspector.UI.FixedValuesDropdown
{
    /// <summary>
    /// A fixed list of values that can be submitted when the user clicks on them.
    /// </summary>
    public class FixedValuesDropdownWindow : MonoBehaviour, IInspectorWindow
    {
        private Type _type;
        public Type Type
        {
            get => _type;
            set
            {
                _type = value;
                SetValidValues( null );
            }
        }

        /// <summary>
        /// The method to call when the user selects a specific object from the list.
        /// </summary>
        public Action<Type, object> onSubmit { get; set; }

        [SerializeField]
        private RectTransform _list;

        InspectorStyle style;

        void Awake()
        {
            this.style = InspectorStyle.Default;
        }

        public void Close()
        {
            Destroy( this.gameObject );
        }

        public void SetValidValues( IEnumerable<object> values )
        {
            for( int i = 0; i < _list.childCount; i++ )
            {
                Destroy( _list.GetChild( i ).gameObject );
            }

            if( values == null )
            {
                return;
            }

            foreach( var value in values )
            {
                if( !this.Type.IsAssignableFrom( value.GetType() ) )
                {
                    throw new InvalidOperationException( $"Invalid value type '{value.GetType().FullName}'. The values must be derived from or of the type '{Type.FullName}'." );
                }

                CreateEntry( value );
            }
        }

        internal void Submit( object value )
        {
            if( !Type.IsAssignableFrom( value.GetType() ) )
            {
                throw new InvalidOperationException( $"Invalid value type '{value.GetType().FullName}'. The value must be derived from or of the type '{Type.FullName}'." );
            }

            onSubmit?.Invoke( Type, value );
        }

        /// <summary>
        /// Creates a fixed values dropdown that returns values of a specific type and sets the method for getting the values.
        /// </summary>
        /// <param name="type">The type of the values that this dropdown will return.</param>
        /// <param name="valuesFunc"></param>
        public static FixedValuesDropdownWindow Create( Transform modalCanvas, Type type, IEnumerable<object> values )
        {
            GameObject prefab = AssetRegistry<GameObject>.GetAsset( "RuntimeInspector/Prefabs/FixedValuesWindow" );

            GameObject windowGO = Instantiate( prefab, modalCanvas );
            FixedValuesDropdownWindow window = windowGO.GetComponent<FixedValuesDropdownWindow>();
            window.Type = type;
            window.SetValidValues( values );

            return window;
        }

        private RectTransform CreateEntry( object value )
        {
            GameObject gameObject = new GameObject( $"_label" );
            gameObject.layer = 5;

            RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.SetParent( _list );

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

            labelText.text = value.ToString();
            labelText.font = style.Font;

            FixedValuesElement elem = gameObject.AddComponent<FixedValuesElement>();
            elem.Window = this;
            elem.Value = value;

            return rectTransform;
        }
    }
}
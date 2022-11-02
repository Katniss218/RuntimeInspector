using RuntimeInspector.Core;
using RuntimeInspector.UI.GUIUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeInspector.UI
{
    /// <summary>
    /// Used to match a binding to a specific UI element.
    /// </summary>
    public class UIBinding : MonoBehaviour
    {
        static List<UIBinding> uiBindings = new List<UIBinding>();

        public static UIBinding Find( MemberBinding binding )
        {
            foreach( var uiBinding in uiBindings )
            {
                if( uiBinding.Binding.Metadata.Equals( binding.Metadata ) && ReferenceEquals( uiBinding.Binding.Binding.Parent, binding.Binding.Parent ) )
                {
                    return uiBinding;
                }
            }
            return null;
        }

        [SerializeField]
        private MemberBinding binding;
        public MemberBinding Binding
        {
            get => binding;
            set
            {
                binding = value;
                if( binding.Metadata.CanRead )
                {
                    currentValue = binding.Binding.GetValue();
                }
            }
        }

        [field: SerializeField]
        public TMPro.TMP_InputField InputField { get; set; } // kinda ugly and requires it to be an input field.

        /// <summary>
        /// Describes the root element for this drawn binding.
        /// </summary>
        /// <remarks>
        /// The root has a property of "if you delete it, it's equivalent to never drawing this binding in the first place".
        /// </remarks>
        [field: SerializeField]
        public RectTransform Root { get; set; }

        [SerializeField]
        private object currentValue;
        [SerializeField]
        private bool isStale;

        /// <summary>
        /// Describes whether the value that was drawn is different to the current value.
        /// </summary>
        public bool IsStale { get => isStale; }

        public bool IsEditing()
        {
            if( InputField != null && InputField.isFocused )
            {
                return true;
            }
            return false;
        }

        public void SetValue( string userProvidedValue )
        {
            InputConverterProvider.AssignValue( Binding, userProvidedValue );
            if( !binding.Metadata.CanRead )
            {
                InputField.text = InspectorInputField.READONLY_PLACEHOLDER;
            }
        }

        int counter = 0;
        const int UPDATE_INTERVAL = 50;

        void Update()
        {
            if( isStale ) // no need to bother checking if we already know the value to be stale (old / incorrect).
            {
                return;
            }
            if( !binding.Metadata.CanRead ) // if the value can't be read, we don't know whether or not it's stale.
            {
                return;
            }

            counter++;

            if( counter >= UPDATE_INTERVAL )
            {
                counter = 0;
                object newValue = Binding.Binding.GetValue();

                if( IObjectBinding_Ex.IsDifferent( currentValue, newValue ) )
                {
                    isStale = true;
                }

                currentValue = newValue;
            }
        }

        void Awake()
        {
            uiBindings.Add( this );
        }

        void OnDestroy()
        {
            uiBindings.Remove( this );
        }
    }
}
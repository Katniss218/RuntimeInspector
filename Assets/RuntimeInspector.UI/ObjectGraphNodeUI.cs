using RuntimeInspector.Core;
using RuntimeInspector.UI.GUIUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RuntimeInspector.UI
{
    /// <summary>
    /// Binds a specific UI element to a graph node.
    /// </summary>
    public class ObjectGraphNodeUI : MonoBehaviour, IPointerClickHandler //IPointerDownHandler, IPointerUpHandler
    {
#warning TODO - these should really be per viewer.
        static List<ObjectGraphNodeUI> uiBindings = new List<ObjectGraphNodeUI>();

        /// <summary>
        /// Finds a UIBinding for a graphnode based on hierarchy and name.
        /// </summary>
        public static ObjectGraphNodeUI Find( ObjectGraphNode node )
        {
            foreach( var uiBinding in uiBindings )
            {
                if( uiBinding.Node.Equals( node ) )
                {
                    return uiBinding;
                }
            }
            return null;
        }

        [field: SerializeField]
        public ObjectGraphNode Node
        {
            get;
            private set;
        }

        /// <summary>
        /// Sets the UI graph binding to point at the new graph node.
        /// </summary>
        public void UpdateGraphNode( ObjectGraphNode node )
        {
            Node = node;
            if( Node.CanRead )
            {
                CurrentValue = Node.GetValue();
            }
        }

        [field: SerializeField]
        public TMPro.TMP_InputField InputField { get; set; }

        /// <summary>
        /// Describes the root element for this drawn binding.
        /// </summary>
        /// <remarks>
        /// The root has a property of "if you delete it, it's equivalent to never drawing this binding in the first place".
        /// </remarks>
        [field: SerializeField]
        public RectTransform Root { get; set; }

        /// <summary>
        /// The value of the currently viewed graph node.
        /// </summary>
        [field: SerializeField]
        public object CurrentValue { get; private set; }

        public GraphNodeUIDrag Drag { get; set; }

        public bool IsEditing()
        {
            if( InputField != null && InputField.isFocused )
            {
                return true;
            }
            return false;
        }

        public void SetValueText( string userProvidedValue )
        {
            SetValue( typeof(string), userProvidedValue );
        }

        public void SetValue( Type inType, object userProvidedValue )
        {
            if( InputConverterProvider.TryConvertForward( Node.Type, inType, userProvidedValue, out object converted ) )
            {
                Node.SetValue( converted );

                if( !Node.CanRead )
                {
                    InputField.text = InspectorTextInputField.READONLY_PLACEHOLDER;
                }
            }
            else
            {
                Debug.LogWarning( $"Couldn't convert value '{userProvidedValue}'" );
            }
        }

        void Awake()
        {
            uiBindings.Add( this );
        }

        void OnDestroy()
        {
            uiBindings.Remove( this );
            if( Drag  != null )
            {
                Destroy( Drag.gameObject );
            }
        }

        private void HandleStartDrag()
        {
            if( GraphNodeUIDrag.CurrentlyDragged != null )
            {
                return;
            }
            GraphNodeUIDrag.StartDragging( this );
            // guard against already dragging something.
            // create the drag object, populate it to reflect this object.
        }

        private void HandleEndDrag()
        {
            if( GraphNodeUIDrag.CurrentlyDragged == null )
            {
                return;
            }
            GraphNodeUIDrag.EndDragging( this );
            // check if drag object exists.
            // - if it does - assign its value (if possible), destroy it.
        }

        public void OnPointerClick( PointerEventData e )
        {
            Debug.Log( "pointer click " + this.gameObject.name );
            if( GraphNodeUIDrag.CurrentlyDragged == null )
            {
                HandleStartDrag();
            }
            else
            {
                HandleEndDrag();
            }
        }
        /*
        public void OnPointerDown( PointerEventData e )
        {
            Debug.Log( "pointer down " + this.gameObject.name );
            HandleStartDrag();
        }

        public void OnPointerUp( PointerEventData e )
        {
            Debug.Log( "pointer up " + this.gameObject.name );
            HandleEndDrag();
        }*/
    }
}
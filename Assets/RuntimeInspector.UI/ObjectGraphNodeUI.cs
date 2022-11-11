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
    /// Represents a specific graph node as a UI element.
    /// </summary>
    public class ObjectGraphNodeUI : MonoBehaviour, IPointerClickHandler
    {
        /// <summary>
        /// Gets the actual graph node associated with this graph node UI element.
        /// </summary>
        public ObjectGraphNode GraphNode { get; private set; }

        /// <summary>
        /// Sets the UI graph binding to point at the new graph node.
        /// </summary>
        public void SetGraphNode( ObjectGraphNode node )
        {
            GraphNode = node;
            if( GraphNode.CanRead )
            {
                CurrentValue = GraphNode.GetValue();
            }
        }

        /// <summary>
        /// The (string) input field associated with this graph node UI. Can be null.
        /// </summary>
        public TMPro.TMP_InputField InputField { get; set; }

        /// <summary>
        /// Describes the root UI object for this drawn binding.
        /// </summary>
        public RectTransform Root { get; set; }

        private Viewer _viewer;
        /// <summary>
        /// Gets the viewer that this graph node UI is associated with.
        /// </summary>
        public Viewer Viewer
        {
            get
            {
                if( _viewer == null )
                {
                    _viewer = this.GetComponentInParent<Viewer>( false );
                }
                if( _viewer == null )
                {
                    throw new Exception( "Couldn't find the viewer component." );
                }

                return _viewer;
            }
        }

        /// <summary>
        /// The value of the currently viewed graph node.
        /// </summary>
        [field: SerializeField]
        public object CurrentValue { get; private set; }

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
            SetValue( typeof( string ), userProvidedValue );
        }

        public void SetValue( Type inType, object userProvidedValue )
        {
            if( InputConverterProvider.TryConvertForward( GraphNode.Type, inType, userProvidedValue, out object converted ) )
            {
                GraphNode.SetValue( converted );

                if( !GraphNode.CanRead )
                {
                    InputField.text = InspectorTextInputField.WRITEONLY_PLACEHOLDER;
                }
            }
            else
            {
                Debug.LogWarning( $"Couldn't convert value '{userProvidedValue}' of type '{inType.FullName}' into type '{GraphNode.Type.FullName}'" );
            }
        }

        public event Action onDestroy;

        void Awake()
        {
            Viewer.GraphNodeUIs.Add( this );
        }

        void OnDestroy()
        {
            this.onDestroy?.Invoke();
            Viewer.GraphNodeUIs.Remove( this );
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
    }
}
using RuntimeInspector.Core;
using RuntimeInspector.UI.GUIUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RuntimeInspector.UI.Inspector
{
    /// <summary>
    /// Represents a specific graph node as a UI element.
    /// </summary>
    public class GraphNodeUI : MonoBehaviour, IPointerClickHandler
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
       // public TMPro.TMP_InputField InputField { get; set; }

        /// <summary>
        /// Describes the root UI object for this drawn binding.
        /// </summary>
        public RectTransform Root { get; set; }

        private InspectorWindow _viewer;
        /// <summary>
        /// Gets the viewer that this graph node UI is associated with.
        /// </summary>
        public InspectorWindow Viewer
        {
            get
            {
                if( _viewer == null )
                {
                    _viewer = this.GetComponentInParent<InspectorWindow>( false );
                }
                if( _viewer == null )
                {
                    throw new Exception( "Couldn't find the viewer component." );
                }

                return _viewer;
            }
        }

        /// <summary>
        /// Cached currently viewed value.
        /// </summary>
        [field: SerializeField]
        public object CurrentValue { get; private set; }

        public bool IsSelected { get; private set; }

        public void SetSelected()
        {
            IsSelected = true;
        }

        public void SetDeselected()
        {
            IsSelected = false;
        }

        /// <summary>
        /// Sets the value of the underlying graph node (and thus the inspected object) to a value converted from an arbitrary value.
        /// </summary>
        /// <remarks>
        /// The type for conversion will be inferred from the type of the inputValue.
        /// </remarks>
        public void SetValue( object inputValue )
        {
            if( inputValue == null )
            {
                throw new ArgumentNullException( nameof( inputValue ), "Input Value can't be null or else the type inferrence can't be performed" );
            }
            SetValueInternal( inputValue.GetType(), GraphNode.Type, inputValue );
        }

        /// <summary>
        /// Sets the value of the underlying graph node (and thus the inspected object) to a value of a different graph node UI.
        /// </summary>
        public void SetValue( GraphNodeUI graphNodeUI )
        {
            SetValueInternal( graphNodeUI.GraphNode.GetInstanceType(), this.GraphNode.Type, graphNodeUI.CurrentValue );
        }

        /// <summary>
        /// Sets the value of the underlying graph node (and thus the inspected object) to a value converted from an arbitrary value.
        /// </summary>
        /// <remarks>
        /// inputValue can be null.
        /// </remarks>
        private void SetValueInternal( Type inputType, Type outputType, object inputValue )
        {
            if( Converter.TryConvertForward( outputType, inputType, inputValue, out object converted ) )
            {
                GraphNode.SetValue( converted );

                if( !GraphNode.CanRead )
                {
#warning TODo - add an event to tell the input field to set the displayed value.
                    // InputField.text = InspectorTextInputField.WRITEONLY_PLACEHOLDER;
                }
            }
            else
            {
                Debug.LogWarning( $"Couldn't convert value '{inputValue}' of type '{inputType.FullName}' into type '{GraphNode.Type.FullName}'" );
            }
        }

        /// <summary>
        /// Checks whether or not this graph node UI is currently being edited by the user and shouldn't be updated.
        /// </summary>
        /// <returns>True if the graph node UI is being edited, otherwise false.</returns>
        public bool IsEditing()
        {
            return IsSelected;
            /*if( InputField != null && InputField.isFocused )
            {
                return true;
            }
            return false;*/
        }

        /// <summary>
        /// An event that is called when the object is destroyed.
        /// </summary>
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
            if( GraphNodeDrag.CurrentlyDragged == null )
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
            if( GraphNodeDrag.CurrentlyDragged != null )
            {
                return;
            }
            GraphNodeDrag.StartDragging( this );
            // guard against already dragging something.
            // create the drag object, populate it to reflect this object.
        }

        private void HandleEndDrag()
        {
            if( GraphNodeDrag.CurrentlyDragged == null )
            {
                return;
            }
            GraphNodeDrag.EndDragging( this );
            // check if drag object exists.
            // - if it does - assign its value (if possible), destroy it.
        }
    }
}
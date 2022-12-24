using RuntimeInspector.Core;
using RuntimeInspector.UI.GUIUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityPlus.InputSystem;

namespace RuntimeInspector.UI.Inspector
{
    /// <summary>
    /// Represents a specific graph node as a UI element.
    /// </summary>
    public class GraphNodeUI : MonoBehaviour, IInputHandler_MouseDragBegin, IInputHandler_MouseDragEnd
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
        /// An event that's called whenever the external objects using SetValue method need to remove their listeners, because they would point to an old/invalid node.
        /// </summary>
        public event Action onSetterInvalidated;

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
            if( Utils.UnityUtils.IsUnityNull( inputValue ) )
            {
                SetValueInternal( GraphNode.Type, GraphNode.Type, null );
                return;
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
                this.onSetterInvalidated?.Invoke();

                if( !GraphNode.CanRead )
                {
#warning TODO - The displayed value should be the "default" value.
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
        }

        void Awake()
        {
            Viewer.GraphNodeUIs.Add( this );
        }

        void OnDestroy()
        {
            this.onSetterInvalidated?.Invoke();
            Viewer.GraphNodeUIs.Remove( this );
        }

        public void BeginDrag( PointerEventData e )
        {
            Debug.Log( $"Begin drag onaaaaa {this.gameObject.name}" );
            if( GraphNodeDrag.CurrentlyDragged != null )
            {
                return;
            }
            GraphNodeDrag.StartDragging( this );
            // guard against already dragging something.
            // create the drag object, populate it to reflect this object.
        }

        public void EndDrag( PointerEventData e )
        {
            Debug.Log( $"End drag onaaaaa {this.gameObject.name}" );
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
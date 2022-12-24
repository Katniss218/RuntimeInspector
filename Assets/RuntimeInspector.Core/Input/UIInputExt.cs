using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityPlus.InputSystem
{
    public interface IInputHandler_MouseDragBegin
    {
#warning TODO - pass the position of the click relative to the object clicked's position and in the clicked object's space to the event.
        void BeginDrag();
    }

    public interface IInputHandler_MouseDragEnd
    {
        void EndDrag();
    }

    [RequireComponent( typeof( UnityEngine.EventSystems.EventSystem ) )]
    public class UIInputExt : MonoBehaviour
    {
        private bool _startedDragging = false;
        private bool _isDragging = false;
        private Vector2 dragStartPos = Vector2.zero;

        void Update()
        {
            HandleDrag();
        }

        void HandleDrag()
        {
            // mouse depressed = register start point.
            // mouse held and previously depressed - listen for when we move past the delta.

            if( !_isDragging && Input.GetKeyDown( KeyCode.Mouse0 ) )
            {
                dragStartPos = Input.mousePosition;

                // otherwise, it takes the dragStartPos from the start/previous drag, and you can start dragging without ever setting it to the clicked pos.
                // This makes it require the button to be pressed in before being held down.
                _startedDragging = true; 
            }
            if( !_isDragging && _startedDragging && Input.GetKey( KeyCode.Mouse0 ) )
            {
                if( Vector2.Distance( Input.mousePosition, dragStartPos ) >= 1.0f ) // drag != click.
                {
                    _isDragging = true;
                    _startedDragging = false; // reset for the next drag.

                    var results = new List<UnityEngine.EventSystems.RaycastResult>();

                    var pointerData = new UnityEngine.EventSystems.PointerEventData( UnityEngine.EventSystems.EventSystem.current );
                    pointerData.position = dragStartPos;

                    UnityEngine.EventSystems.EventSystem.current.RaycastAll( pointerData, results ); // raycast the object at the position where the mouse drag began.

                    if( results.Count > 0 )
                    {
                        var gameObject = results[0].gameObject;

                        IInputHandler_MouseDragBegin h = gameObject.GetComponent<IInputHandler_MouseDragBegin>();
                        if( h != null )
                        {
                            h.BeginDrag();
                        }
                    }
                }
               // else
               // {
               //       We could handle simple clicking here.
               // }
            }

            if( _isDragging && Input.GetKeyUp( KeyCode.Mouse0 ) )
            {
                _isDragging = false;

                var results = new List<UnityEngine.EventSystems.RaycastResult>();

                var pointerData = new UnityEngine.EventSystems.PointerEventData( UnityEngine.EventSystems.EventSystem.current );
                pointerData.position = Input.mousePosition;

                UnityEngine.EventSystems.EventSystem.current.RaycastAll( pointerData, results );

                if( results.Count > 0 )
                {
                    var gameObject = results[0].gameObject;

                    IInputHandler_MouseDragEnd h = gameObject.GetComponent<IInputHandler_MouseDragEnd>();
                    if( h != null )
                    {
                        h.EndDrag();
                    }
                }
            }
        }
    }
}
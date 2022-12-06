using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RuntimeInspector.Core.Input
{
    public interface IMouseDragBeginHandler
    {
        void BeginDrag();
    }

    public interface IMouseDragEndHandler
    {
        void EndDrag();
    }

    [RequireComponent( typeof( UnityEngine.EventSystems.EventSystem ) )]
    public class UIInputExt : MonoBehaviour
    {
        private bool _isDragging = false;
        private Vector2 dragStartPos = Vector2.zero;

        void Update()
        {
            HandleDrag();
        }

        void HandleDrag()
        {
            // mouse down = register start point.
            // mouse pressed - listen for when we move past the delta.

            if( !_isDragging && UnityEngine.Input.GetKeyDown( KeyCode.Mouse0 ) )
            {
                dragStartPos = UnityEngine.Input.mousePosition;

            }
            if( !_isDragging && UnityEngine.Input.GetKey( KeyCode.Mouse0 ) )
            {
                if( Vector2.Distance( UnityEngine.Input.mousePosition, dragStartPos ) >= 1.0f ) // drag != click.
                {
                    _isDragging = true;

                    var results = new List<UnityEngine.EventSystems.RaycastResult>();

                    var pointerData = new UnityEngine.EventSystems.PointerEventData( UnityEngine.EventSystems.EventSystem.current );
                    pointerData.position = dragStartPos;

                    UnityEngine.EventSystems.EventSystem.current.RaycastAll( pointerData, results );

                    if( results.Count > 0 )
                    {
                        var gameObject = results[0].gameObject;

                        IMouseDragBeginHandler h = gameObject.GetComponent<IMouseDragBeginHandler>();
                        if( h != null )
                        {
                            h.BeginDrag();
                        }
                    }
                }
            }

            if( _isDragging && UnityEngine.Input.GetKeyUp( KeyCode.Mouse0 ) )
            {
                _isDragging = false;

                var results = new List<UnityEngine.EventSystems.RaycastResult>();

                var pointerData = new UnityEngine.EventSystems.PointerEventData( UnityEngine.EventSystems.EventSystem.current );
                pointerData.position = UnityEngine.Input.mousePosition;

                UnityEngine.EventSystems.EventSystem.current.RaycastAll( pointerData, results );

                if( results.Count > 0 )
                {
                    var gameObject = results[0].gameObject;

                    IMouseDragEndHandler h = gameObject.GetComponent<IMouseDragEndHandler>();
                    if( h != null )
                    {
                        h.EndDrag();
                    }
                }
            }
        }
    }
}
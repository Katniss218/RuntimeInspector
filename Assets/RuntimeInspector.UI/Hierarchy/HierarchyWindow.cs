using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RuntimeInspector.UI.GUIUtils;
using System.Linq;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace RuntimeInspector.UI.Hierarchy
{
    public class HierarchyWindow : MonoBehaviour, IPointerClickHandler
    {
        [field: SerializeField]
        public RectTransform ViewerPanel { get; set; }

        [field: SerializeField]
        public Transform[] DeadEnds { get; set; }

        private Dictionary<GameObject, HierarchyElement> _rootHierarchies = new Dictionary<GameObject, HierarchyElement>();

        //InspectorStyle style;

        public UnityEvent<object> onSelect;

        /*void Awake()
        {
            style = InspectorStyle.Default;
        }
        */

        public void RemoveFromRoot( HierarchyElement hi )
        {
            _rootHierarchies.Remove( hi.Obj );
        }

        public void RedrawHierarchy()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            GameObject[] rootGameObjects = activeScene.GetRootGameObjects();
            foreach( var go in rootGameObjects )
            {
                if( _rootHierarchies.TryGetValue( go, out HierarchyElement hi ) ) // after reparenting, it stops being root, and the root hierarchy can grow uncontrollably.
                {
                    // if the object was destroyed - WTF?
                    if( hi == null )
                    {
                        Debug.LogWarning( "WTF - object was already destroyed, but is present??" );
                        _rootHierarchies.Remove( go );
                    }
                    else 
                    {
                        hi.UpdateHierarchyItem();
                        continue;
                    }
                }

                hi = HierarchyElement.Create( this, go );
                _rootHierarchies.Add( go, hi );
            }

#warning TODO - Layout update is kinda flicker'y and takes a long time with nested objects.
            LayoutRebuilder.ForceRebuildLayoutImmediate( this.ViewerPanel );
        }

        void Start()
        {
            RedrawHierarchy();
        }

        int timer = 0;
        const int UPDATE_DELAY = 50;

        void Update()
        {
            timer++;

            if( timer > UPDATE_DELAY )
            {
                timer = 0;

                RedrawHierarchy();
            }
        }

        public void OnPointerClick( PointerEventData eventData )
        {
            if( eventData.button == PointerEventData.InputButton.Right )
            {
                ContextMenu cm = ContextMenu.Create( GameObject.Find( "ContextMenuCanvas" ).transform, eventData.position );
                cm.AddOption( "Create Empty", () => new GameObject( "New Game Object" ) );
            }
        }
    }
}
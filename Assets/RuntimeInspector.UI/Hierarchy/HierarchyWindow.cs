using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RuntimeInspector.UI.GUIUtils;
using System.Linq;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

namespace RuntimeInspector.UI.Hierarchy
{
    public class HierarchyWindow : MonoBehaviour
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
        public void RedrawHierarchy()
        {
            // go through

            // maybe don't store the UI elements for every object, since most will be collapsed.

            Scene activeScene = SceneManager.GetActiveScene();
            GameObject[] rootGameObjects = activeScene.GetRootGameObjects();
            foreach( var go in rootGameObjects )
            {
                if( _rootHierarchies.TryGetValue( go, out HierarchyElement hi ) )
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

                hi = HierarchyElement.Create( this, go.transform );
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
    }
}
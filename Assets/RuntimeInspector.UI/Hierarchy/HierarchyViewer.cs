using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RuntimeInspector.UI.GUIUtils;
using System.Linq;
using UnityEngine.UI;

namespace RuntimeInspector.UI.Hierarchy
{
    public class HierarchyViewer : MonoBehaviour
    {
        [field: SerializeField]
        public RectTransform ViewerPanel { get; set; }

        [field: SerializeField]
        public Transform[] DeadEnds { get; set; }

        private Dictionary<GameObject, HierarchyItem> _rootHierarchies = new Dictionary<GameObject, HierarchyItem>();

        public void RedrawHierarchy()
        {
            // go through

            // maybe don't store the UI elements for every object, since most will be collapsed.


            InspectorStyle style = InspectorStyle.Default;

            Scene activeScene = SceneManager.GetActiveScene();
            GameObject[] rootGameObjects = activeScene.GetRootGameObjects();
            foreach( var go in rootGameObjects )
            {
                if( _rootHierarchies.TryGetValue( go, out HierarchyItem hi ) )
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

                hi = HierarchyItem.Create( ViewerPanel, go.transform );
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
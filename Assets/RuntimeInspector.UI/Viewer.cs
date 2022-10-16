using RuntimeInspector.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeInspector.UI
{
    /// <summary>
    /// Used to inspect objects.
    /// </summary>
    public class Viewer : MonoBehaviour
    {
        ObjectReflector reflector = new ObjectReflector();

        [field: SerializeField]
        public RectTransform ViewerPanel { get; set; }

        [field: SerializeField]
        public Component DrawnObj { get; set; }

        // you'll have the viewer class. that viewer class will set you up when you tell it to.

        // you can then click on stuff to inspect it.
        // clicking on stuff might prove troublesome, not everything will have a collider.

        // need to show hierarchy as well.

        public void SetObject( object obj )
        {
            reflector.BindTo( obj );
        }

        // it can be used to preview gameobjects, as well as other things.
        // the core reflector API should be Unity-agnostic.
        // Unity compat done through extensions.

        public void Show()
        {
            if( !reflector.IsBound )
            {
                return;
            }

            foreach( var member in reflector.AssignableMembers )
            {
                try
                {
                    IDrawer drawer = DrawerManager.GetDrawerOfType( member.Type );
                    RectTransform rt = drawer.Draw( ViewerPanel, member );
                }
                catch
                {
                    // temporary.
                }
            }
        }

        void Start()
        {
            SetObject( DrawnObj );
            Show();
        }

        void Update()
        {
            // continuously redraw the values if updated.
        }
    }
}
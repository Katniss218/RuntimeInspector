using RuntimeInspector.Core;
using System;
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

        private List<InspectorInputField> monitors = new List<InspectorInputField>();

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

            foreach( var binding in reflector.AssignableMembers )
            {

                GameObject nested = new GameObject( "_binding" );
                nested.layer = 5;
                RectTransform nestedTransform = nested.AddComponent<RectTransform>();

                nestedTransform.SetParent( ViewerPanel );
                nestedTransform.sizeDelta = new Vector2( 0.0f, DrawerUtils.FIELD_HEIGHT );


                InspectorInputField submitter = nested.AddComponent<InspectorInputField>();
                submitter.Binding = binding;

                monitors.Add( submitter );
            }
        }

        void Start()
        {
            SetObject( DrawnObj );
            Show();
        }

        int timer = 0;

        void Update()
        {
            timer++;
            if( timer > 50 )
            {
                timer = 0;
                foreach( var monitor in monitors )
                {
                    // don't update members that we're trying to input values to.
                    if( monitor.InputField != null && monitor.InputField.isFocused )
                    {
                        continue;
                    }

                    for( int i = 0; i < monitor.transform.childCount; i++ )
                    {
                        Destroy( monitor.transform.GetChild( i ).gameObject );
                    }

                    IDrawer drawer = DrawerManager.GetDrawerOfType( monitor.Binding.Type );

                    try
                    {
                        RectTransform rt = drawer.Draw( monitor.GetComponent<RectTransform>(), monitor.Binding );
                        // continuously redraw the values if updated, but not those that are currently being edited. I think that should be responsibility of each individual UI element.
                    }
                    catch( Exception ex )
                    {
                        Debug.LogWarning( $"EXCEPTION while trying to get value of: {ex}" );
                        // temporary.
                    }
                }
#warning TODO - this should be moved to a generic drawer, make that drawer recursively display all fields. But only after we figure out how to stop it from displaying Unity fields.
            }
        }
    }
}
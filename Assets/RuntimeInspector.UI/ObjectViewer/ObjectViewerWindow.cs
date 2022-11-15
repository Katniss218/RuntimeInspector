using RuntimeInspector.Core.AssetManagement;
using RuntimeInspector.UI.GUIUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RuntimeInspector.UI.ObjectViewer
{
    /// <summary>
    /// The Object Viewer Window is used to list and search through every object of a specific type that is currently loaded.
    /// </summary>
    public class ObjectViewerWindow : MonoBehaviour, IInspectorWindow
    {
        /// <summary>
        /// The type of object that will be listed in the window.
        /// </summary>
        public Type ObjectType { get; private set; }

        /// <summary>
        /// The method to call when the user selects a specific object from the list.
        /// </summary>
        public Action<Type, object> onSubmit { get; set; }

        [SerializeField]
        private RectTransform _list;

        [SerializeField]
        private TMPro.TMP_InputField _nameInputField;

        private Object[] _allObjects; // Every object of type T.

        private List<Object> _foundObjects; // Objects of type T matching the search query.

        InspectorStyle style;

        void Awake()
        {
            this._nameInputField.onSubmit.AddListener( SubmitName );
            this.style = InspectorStyle.Default;
        }

        public void SubmitName( string name )
        {
            this.FindObjects();
            this.UpdateSearchQuery( SearchQuery.Empty.WithName( name ) );
        }

        /// <summary>
        /// Sets the type and recalculates the cached objects.
        /// </summary>
        public void FindObjects( Type objType )
        {
            this.ObjectType = objType;

            FindObjects();
        }

        /// <summary>
        /// Sets the type and recalculates the cached objects.
        /// </summary>
        public void FindObjects()
        {
            this._allObjects = FindObjectsOfType( this.ObjectType );
            this._foundObjects = new List<Object>( this._allObjects );

            UpdateList();
        }

        internal void Select( Object obj )
        {
            if( obj.GetType() != ObjectType )
            {
                throw new InvalidOperationException( "The type of the passed object must be the same as the type of the window" );
            }

            onSubmit?.Invoke( ObjectType, obj );
        }

        /// <summary>
        /// Updates the cache of found objects to the subset of the all objects that match the query.
        /// </summary>
        public void UpdateSearchQuery( SearchQuery query )
        {
            _foundObjects = new List<Object>();

            foreach( var obj in _allObjects )
            {
                if( obj == null )
                {
                    continue;
                }
                if( query.Matches( obj ) )
                {
                    _foundObjects.Add( obj );
                }
            }

            UpdateList();
        }

        private void UpdateList()
        {
            for( int i = 0; i < _list.childCount; i++ )
            {
                Destroy( _list.GetChild( i ).gameObject );
            }

            foreach( var obj in _foundObjects )
            {
                CreateEntry( obj );
            }
        }

        void Start()
        {
            if( this.ObjectType == null )
            {
                return;
            }
            FindObjects( this.ObjectType );
            UpdateSearchQuery( SearchQuery.Empty );
        }

        public void Close()
        {
            Destroy( this.gameObject );
        }

        public static ObjectViewerWindow Create( Transform modalCanvas, Type objType )
        {
            GameObject prefab = AssetRegistry<GameObject>.GetAsset( "RuntimeInspector/Prefabs/ObjectViewerWindow" );

            GameObject windowGO = Instantiate( prefab, modalCanvas );
            ObjectViewerWindow window = windowGO.GetComponent<ObjectViewerWindow>();
            window.FindObjects( objType );

            return window;
        }

        private RectTransform CreateEntry( Object obj )
        {
            GameObject gameObject = new GameObject( $"_label" );
            gameObject.layer = 5;

            RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.SetParent( _list );

            rectTransform.anchorMin = new Vector2( 0.0f, 0.5f );
            rectTransform.anchorMax = new Vector2( 0.0f, 0.5f );
            rectTransform.pivot = new Vector2( 0.0f, 0.5f );
            rectTransform.anchoredPosition = new Vector2( 0.0f, 0.0f );
            rectTransform.sizeDelta = new Vector2( 200.0f, style.FieldHeight );

            TMPro.TextMeshProUGUI labelText = gameObject.AddComponent<TMPro.TextMeshProUGUI>();
            labelText.fontSize = style.FontSize;
            labelText.alignment = TMPro.TextAlignmentOptions.Left;
            labelText.overflowMode = TMPro.TextOverflowModes.Overflow;
            labelText.color = style.LabelTextColor;

            labelText.text = obj.name;
            labelText.font = style.Font;

            ObjectViewerElement elem = gameObject.AddComponent<ObjectViewerElement>();
            elem.Window = this;
            elem.Obj = obj;

            return rectTransform;
        }
    }
}

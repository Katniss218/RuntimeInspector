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
        public Type Type { get; private set; }

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

        void Awake()
        {
            this._nameInputField.onSubmit.AddListener( SubmitName );
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
            this.Type = objType;

            FindObjects();
        }

        /// <summary>
        /// Sets the type and recalculates the cached objects.
        /// </summary>
        public void FindObjects()
        {
            this._allObjects = FindObjectsOfType( this.Type );
            this._foundObjects = new List<Object>( this._allObjects );

            UpdateList();
        }

        internal void Submit( Object value )
        {
            if( !Type.IsAssignableFrom( value.GetType() ) )
            {
                throw new InvalidOperationException( $"Invalid value type '{value.GetType().FullName}'. The value must be derived from or of the type '{Type.FullName}'." );
            }

            onSubmit?.Invoke( Type, value );
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
            if( this.Type == null )
            {
                return;
            }
            FindObjects( this.Type );
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

        private RectTransform CreateEntry( Object value )
        {
            GameObject gameObject = new GameObject( $"_label" );
            gameObject.layer = 5;

            RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.SetParent( _list );

            rectTransform.anchorMin = new Vector2( 0.0f, 0.5f );
            rectTransform.anchorMax = new Vector2( 0.0f, 0.5f );
            rectTransform.pivot = new Vector2( 0.0f, 0.5f );
            rectTransform.anchoredPosition = new Vector2( 0.0f, 0.0f );
            rectTransform.sizeDelta = new Vector2( 200.0f, InspectorStyle.Default.FieldHeight );

            TMPro.TextMeshProUGUI labelText = gameObject.AddComponent<TMPro.TextMeshProUGUI>();
            labelText.fontSize = InspectorStyle.Default.FontSize;
            labelText.alignment = TMPro.TextAlignmentOptions.Left;
            labelText.overflowMode = TMPro.TextOverflowModes.Overflow;
            labelText.color = InspectorStyle.Default.LabelTextColor;

            labelText.text = value.name;
            labelText.font = InspectorStyle.Default.Font;

            ObjectViewerElement elem = gameObject.AddComponent<ObjectViewerElement>();
            elem.Window = this;
            elem.Value = value;

            return rectTransform;
        }
    }
}

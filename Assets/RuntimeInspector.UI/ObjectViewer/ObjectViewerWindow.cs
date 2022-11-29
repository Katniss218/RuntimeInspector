using RuntimeInspector.Core.AssetManagement;
using RuntimeInspector.UI.GUIUtils;
using RuntimeInspector.UI.Inspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RuntimeInspector.UI.ObjectViewer
{
    public struct Entry
    {
        public string DisplayName { get; set; }
        public object Obj { get; set; }

        public bool IsDefault => DisplayName == null;

        public static Entry Default => new Entry() { DisplayName = null, Obj = null };
    }

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

        private Entry[] _allEntries;

        private List<Entry> _foundEntries;

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
            List<Object> allObjects = new List<Object>( FindObjectsOfType( this.Type ) );

            List<Entry> entries = new List<Entry>()
            {
                Entry.Default
            };

            foreach( var obj in allObjects )
            {
                entries.Add( new Entry() { DisplayName = obj.name, Obj = obj } );
            }

            this._allEntries = entries.ToArray();

            this._foundEntries = new List<Entry>( this._allEntries );

            UpdateList();
        }

        internal void Submit( object value )
        {
            if( !Utils.UnityUtils.IsUnityNull( value ) && !Type.IsAssignableFrom( value.GetType() ) )
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
            _foundEntries = new List<Entry>();

            foreach( var obj in _allEntries )
            {
                if( query.Matches( obj ) )
                {
                    _foundEntries.Add( obj );
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

            foreach( var obj in _foundEntries )
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

        private RectTransform CreateEntry( Entry entry )
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

            labelText.text = entry.DisplayName;
            labelText.font = InspectorStyle.Default.Font;

            ObjectViewerElement elem = gameObject.AddComponent<ObjectViewerElement>();
            elem.Window = this;
            elem.Value = entry.Obj;

            return rectTransform;
        }
    }
}
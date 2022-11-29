using RuntimeInspector.Core.AssetManagement;
using RuntimeInspector.UI.GUIUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;
using Object = UnityEngine.Object;

namespace RuntimeInspector.UI.AssetViewer
{
    /// <summary>
    /// The Object Viewer Window is used to list and search through every object of a specific type that is currently loaded.
    /// </summary>
    public class AssetViewerWindow : MonoBehaviour, IInspectorWindow
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

        private (string assetID, object)[] _allObjects; // objects that support batched fetching.

        private List<(string assetID, object)> _foundObjects;

        void Awake()
        {
            this._nameInputField.onSubmit.AddListener( SubmitName );
        }

        public void SubmitName( string name )
        {
            this.FindObjects();


            this.UpdateSearchQuery( name );
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
            Type assetRegistryType = typeof( AssetRegistry<> );
            Type t = assetRegistryType.MakeGenericType( Type );

            MethodInfo method = t.GetMethod( nameof( AssetRegistry<object>.GetAllReflection ) );
            try
            {
                object obj = method.Invoke( null, null );
                List<(string, object)> objects = (List<(string, object)>)obj;
                this._allObjects = objects.ToArray();
            }
            catch( InvalidOperationException ex )
            {
                Debug.LogException( ex );
            }

            this._foundObjects = new List<(string assetID, object)>( this._allObjects );

            UpdateList();
        }

        internal void Submit( object value )
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
        public void UpdateSearchQuery( string assetID )
        {
            _foundObjects = new List<(string assetID, object)>();

            if( !string.IsNullOrEmpty( assetID ) )
            {
                Type assetRegistryType = typeof( AssetRegistry<> );
                Type t = assetRegistryType.MakeGenericType( Type );

                MethodInfo method = t.GetMethod( nameof( AssetRegistry<object>.GetAsset ) );
                try
                {
                    object asset = method.Invoke( null, new[] { assetID } );
                    _foundObjects.Add( (assetID, asset) );
                }
                catch( InvalidOperationException ex )
                {
                    Debug.LogException( ex );
                }
            }

            foreach( var obj in _allObjects )
            {
                if( obj.Item2 == null )
                {
                    continue;
                }
                if( obj.assetID.Contains( assetID ) )
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
                CreateEntry( $"{obj.assetID}", obj.Item2 );
            }
        }

        void Start()
        {
            if( this.Type == null )
            {
                return;
            }
            FindObjects( this.Type );
            UpdateSearchQuery( "" );
        }

        public void Close()
        {
            Destroy( this.gameObject );
        }

        public static AssetViewerWindow Create( Transform modalCanvas, Type objType )
        {
            GameObject prefab = AssetRegistry<GameObject>.GetAsset( "RuntimeInspector/Prefabs/AssetViewerWindow" );

            GameObject windowGO = Instantiate( prefab, modalCanvas );
            AssetViewerWindow window = windowGO.GetComponent<AssetViewerWindow>();
            window.FindObjects( objType );

            return window;
        }

        private RectTransform CreateEntry( string assetID, object value )
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

            labelText.text = assetID;
            labelText.font = InspectorStyle.Default.Font;

            AssetViewerElement elem = gameObject.AddComponent<AssetViewerElement>();
            elem.Window = this;
            elem.Value = value;

            return rectTransform;
        }
    }
}
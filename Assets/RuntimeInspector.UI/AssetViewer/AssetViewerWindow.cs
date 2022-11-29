using RuntimeInspector.Core.AssetManagement;
using RuntimeInspector.UI.GUIUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;
using Object = UnityEngine.Object;
using RuntimeInspector.UI.Inspector;

namespace RuntimeInspector.UI.AssetViewer
{
    public struct Entry
    {
        public string AssetID { get; set; }
        public object Obj { get; set; }

        public bool IsDefault => AssetID == null;

        public static Entry Default => new Entry() { AssetID = null, Obj = null };
    }

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

        private Entry[] _allEntries; // objects that support batched fetching.

        private List<Entry> _foundEntries;

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
            assetRegistryType = assetRegistryType.MakeGenericType( Type );

            MethodInfo method = assetRegistryType.GetMethod( nameof( AssetRegistry<object>.GetAllReflection ) );
            try
            {
                List<(string assetID, object obj)> foundAssets = (List<(string, object)>)method.Invoke( null, null );

                List<Entry> entries = new List<Entry>( foundAssets.Count )
                {
                    Entry.Default
                };

                foreach( var obj in foundAssets )
                {
                    entries.Add( new Entry() { AssetID = obj.assetID, Obj = obj.obj } );
                }

                this._allEntries = entries.ToArray();
            }
            catch( InvalidOperationException ex )
            {
                Debug.LogException( ex );
            }

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

        private Entry? FindExact( string assetID )
        {
            Type assetRegistryType = typeof( AssetRegistry<> );
            assetRegistryType = assetRegistryType.MakeGenericType( Type );

            MethodInfo checkMethod = assetRegistryType.GetMethod( nameof( AssetRegistry<object>.Exists ) );
            MethodInfo getMethod = assetRegistryType.GetMethod( nameof( AssetRegistry<object>.GetAsset ) );

            if( (bool)checkMethod.Invoke( null, new[] { assetID } ) )
            {
                object asset = getMethod.Invoke( null, new[] { assetID } );

                return new Entry() { AssetID = assetID, Obj = asset };
            }
            return null;
        }

        /// <summary>
        /// Updates the cache of found objects to the subset of the all objects that match the query.
        /// </summary>
        public void UpdateSearchQuery( string assetID )
        {
            _foundEntries = new List<Entry>();

            if( !string.IsNullOrEmpty( assetID ) )
            {
                Entry? result = FindExact( assetID );
                if( result != null )
                {
                    _foundEntries.Add( result.Value );
                }
            }

            foreach( var entry in _allEntries )
            {
                if( entry.IsDefault || entry.AssetID.Contains( assetID ) )
                {
                    _foundEntries.Add( entry );
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

            foreach( var entry in _foundEntries )
            {
                CreateEntry( entry );
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

            labelText.text = entry.AssetID;
            labelText.font = InspectorStyle.Default.Font;

            AssetViewerElement elem = gameObject.AddComponent<AssetViewerElement>();
            elem.Window = this;
            elem.Value = entry.Obj;

            return rectTransform;
        }
    }
}
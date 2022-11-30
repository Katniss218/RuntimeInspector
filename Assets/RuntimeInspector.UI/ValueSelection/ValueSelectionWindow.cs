using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeInspector.UI.GUIUtils;
using RuntimeInspector.Core.AssetManagement;

namespace RuntimeInspector.UI.ValueSelection
{
    public interface IEntryProvider
    {
        Entry[] GetAllEntries( Type type );
    }

    public struct Entry
    {
        public string Identifier { get; set; }
        public object Obj { get; set; }

        public bool IsDefault => Identifier == null;

        /// <summary>
        /// A special case when the entry represents the default value.
        /// </summary>
        public static Entry Default => new Entry() { Identifier = null, Obj = null };
    }

    public class ValueSelectionWindow : MonoBehaviour, IInspectorWindow
    {
        public Type Type { get; private set; }

        /// <summary>
        /// The method to call when the user selects a specific object from the list.
        /// </summary>
        public Action<Type, object> onSubmit { get; set; }

        [SerializeField]
        private RectTransform _list;

#warning TODO - helper class to translate the various possible window arrangements and input fields into a search query update that is sent whenever we need to refresh the list.

        private Entry[] _allEntries; // objects that support batched fetching.

        public IEntryProvider EntryProvider;

        public void FindObjectsAndRefreshList()
        {
            this._allEntries = EntryProvider.GetAllEntries( this.Type );

            this.RefreshFilteredEntries( SearchQuery.Empty );
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
        public void RefreshFilteredEntries( SearchQuery filter )
        {
            List<Entry> filteredEntries = new List<Entry>();

            foreach( var obj in _allEntries )
            {
                if( filter.Matches( obj ) )
                {
                    filteredEntries.Add( obj );
                }
            }

            RefreshList( filteredEntries );
        }

        private void RefreshList( IEnumerable<Entry> entriesToDisplay )
        {
            for( int i = 0; i < _list.childCount; i++ )
            {
                Destroy( _list.GetChild( i ).gameObject );
            }

            foreach( var entry in entriesToDisplay )
            {
                CreateEntry( entry );
            }
        }

        public void Close()
        {
#warning TODO - this sometimes doesn't get unpinned. Something to do with when you open a window and its node gets redrawn.
            Destroy( this.gameObject );
        }

        public static ValueSelectionWindow Create( Transform modalCanvas, Type objType, IEntryProvider entryProvider )
        {
            GameObject prefab = AssetRegistry<GameObject>.GetAsset( "RuntimeInspector/Prefabs/ValueSelectionWindow" );

            GameObject windowGO = Instantiate( prefab, modalCanvas );
            ValueSelectionWindow window = windowGO.GetComponent<ValueSelectionWindow>();
            window.Type = objType;
            window.EntryProvider = entryProvider;
            window.FindObjectsAndRefreshList();

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

            labelText.text = entry.Identifier;
            labelText.font = InspectorStyle.Default.Font;

            ValueSelectionElement elem = gameObject.AddComponent<ValueSelectionElement>();
            elem.Window = this;
            elem.Value = entry.Obj;

            return rectTransform;
        }
    }
}
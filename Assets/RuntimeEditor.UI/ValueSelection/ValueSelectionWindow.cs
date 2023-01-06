using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeEditor.UI.GUIUtils;
using UnityPlus.AssetManagement;

namespace RuntimeEditor.UI.ValueSelection
{
    /// <summary>
    /// A window that displays a searchable list of values that can be submitted when the user clicks on them.
    /// </summary>
    public class ValueSelectionWindow : MonoBehaviour, IInspectorWindow
    {
        /// <summary>
        /// The type of the object that this window displays.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// The entry provider that will specify what the window will display.
        /// </summary>
        public IEntryProvider EntryProvider;

        /// <summary>
        /// The method to call when the user selects a specific object from the list.
        /// </summary>
        public Action<Type, object> onSubmit { get; set; }

        public Action onClose { get; set; }

        [SerializeField]
        RectTransform _list;

        Entry[] _allEntries;

        /// <summary>
        /// Calls the entry provider and refreshes the list with the results.
        /// </summary>
        public void FindObjectsAndRefreshList()
        {
            this._allEntries = EntryProvider.GetAllEntries( this.Type );

            this.RefreshFilteredEntries( SearchQuery.Empty );
        }

        /// <summary>
        /// Makes the window submit a specified value.
        /// </summary>
        internal void Submit( object value )
        {
            if( !value.IsUnityNull() && !Type.IsAssignableFrom( value.GetType() ) )
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
            Destroy( this.gameObject );

            this.onClose?.Invoke();
        }

        public static ValueSelectionWindow Create( Transform modalCanvas, Type objType, IEntryProvider entryProvider )
        {
            GameObject prefab = AssetRegistry<GameObject>.GetAsset( "RuntimeEditor/Prefabs/ValueSelectionWindow" );

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
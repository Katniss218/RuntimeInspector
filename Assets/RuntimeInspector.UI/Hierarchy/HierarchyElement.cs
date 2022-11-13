using RuntimeInspector.UI.GUIUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RuntimeInspector.UI.Hierarchy
{
    public class HierarchyElement : MonoBehaviour, IPointerClickHandler
    {
        /// <summary>
        /// The GameObject corresponding to this hierarchy item.
        /// </summary>
        public Transform Obj { get; private set; }

        /// <summary>
        /// The UI element that contains the children of this HierarchyElement.
        /// </summary>
        public RectTransform List { get; private set; }

        /// <summary>
        /// If true, this hierarchy item should display its children.
        /// </summary>
        public bool IsExpanded { get; private set; } = false;

        public HierarchyElement Parent { get; private set; }

        TMPro.TextMeshProUGUI _text;

        void Update()
        {
            if( Obj == null )
            {
                UpdateHierarchyItem();
            }
        }

        public void UpdateHierarchyItem()
        {
            if( Obj == null )
            {
                Destroy( this.gameObject );
                return;
            }
            
            // If the hierarchy's children are stale - delete, and redraw all of them. TODO - this could potentially be optimized.
            if( this.List.childCount != this.Obj.childCount )
            {
                for( int i = 0; i < this.List.childCount; i++ )
                {
                    Destroy( this.List.GetChild( i ).gameObject );
                }

                if( IsExpanded )
                {
                    for( int i = 0; i < this.Obj.childCount; i++ )
                    {
                        Create( this, this.Obj.GetChild( i ) );
                    }
                }
            }

            // Update the children of the root hierarchies to reflect the up-to-date state of things.
            for( int i = 0; i < this.List.childCount; i++ )
            {
                this.List.GetChild( i ).GetComponent<HierarchyElement>().UpdateHierarchyItem();
            }

            UpdateSelf();
        }

        private void UpdateSelf()
        {
            _text.text = $"{(this.IsExpanded ? "▼" : "-")} [{Obj.childCount}] {Obj.gameObject.name}";
            if( IsExpanded )
            {
                this.List.gameObject.SetActive( true );
            }
            else
            {
                this.List.gameObject.SetActive( false );
            }

            if( this.Parent != null && Obj.parent != this.Parent.Obj )
            {
                this.transform.SetParent( this.Parent.List );
                this.transform.SetSiblingIndex( Obj.GetSiblingIndex() );
            }
            int siblingIndex = this.transform.GetSiblingIndex();
            int objSiblingIndex = this.Obj.GetSiblingIndex();
            if( siblingIndex != objSiblingIndex )
            {
                this.transform.SetSiblingIndex( objSiblingIndex );
            }
        }

        public void ToggleExpanded()
        {
            this.IsExpanded = !this.IsExpanded;
            this.UpdateHierarchyItem();
        }

        public void OnPointerClick( PointerEventData e )
        {
            ToggleExpanded();
        }

        public static HierarchyElement Create( RectTransform list, Transform obj )
        {
            return CreateInternal( null, list, obj );
        }
        
        public static HierarchyElement Create( HierarchyElement parent, Transform obj )
        {
            return CreateInternal( parent, parent.List, obj );
        }

        /// <summary>
        /// Creates a new HierarchyItem that is a child of another HierarchyItem, and refers to a specific object.
        /// </summary>
        private static HierarchyElement CreateInternal( HierarchyElement hierarchyParent, RectTransform list, Transform obj )
        {
            InspectorStyle style = InspectorStyle.Default;

            GameObject gameObject = new GameObject();

            RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.SetParent( list );

            rectTransform.anchorMin = new Vector2( 0.0f, 0.5f );
            rectTransform.anchorMax = new Vector2( 1.0f, 0.5f );
            rectTransform.pivot = new Vector2( 0.5f, 1.0f );
            rectTransform.anchoredPosition = new Vector2( 0.0f, 0.0f );
            rectTransform.sizeDelta = new Vector2( 0.0f, style.FieldHeight );

            HierarchyElement hi = gameObject.AddComponent<HierarchyElement>();
            hi.Obj = obj;
            hi.Parent = hierarchyParent;

#warning TODO - replace with custom layout element class that takes preferred width from the content size fitter.

            VerticalLayoutGroup layoutGroup = gameObject.AddComponent<VerticalLayoutGroup>();
            layoutGroup.padding = new RectOffset( 0, 0, 0, 0 );

            layoutGroup.spacing = 0.0f;
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = false;
            layoutGroup.childScaleWidth = false;
            layoutGroup.childScaleHeight = false;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = false;

            ContentSizeFitter contentSizeFitter = gameObject.AddComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            RectTransform label = InspectorLabel.Create( rectTransform, "", style );
            TMPro.TextMeshProUGUI textMesh = label.GetComponent<TMPro.TextMeshProUGUI>();
            hi._text = textMesh;

            RectTransform newList = InspectorVerticalList.Create( "abc", rectTransform, style, new InspectorVerticalList.Params() { IncludeMargin = true } );
            hi.List = newList;

            hi.UpdateHierarchyItem();

            return hi;
        }
    }
}
using RuntimeInspector.Core.AssetManagement;
using RuntimeInspector.Core.Input;
using RuntimeInspector.UI.GUIUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RuntimeInspector.UI.Hierarchy
{
    public class HierarchyElement : MonoBehaviour, IPointerClickHandler, IInputHandler_MouseDragBegin, IInputHandler_MouseDragEnd
    {
        static HierarchyElement dragged = null;

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

        public HierarchyWindow Window { get; set; }

        public HierarchyElement Parent { get; private set; }

        TMPro.TextMeshProUGUI _text;
        Image _image;

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
            _text.text = $"[{Obj.childCount}] {Obj.gameObject.name}";
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
            if( this.IsExpanded )
            {
                this._image.sprite = AssetRegistry<Sprite>.GetAsset( "RuntimeInspector/Sprites/icon_expanded" );
            }
            else
            {
                this._image.sprite = AssetRegistry<Sprite>.GetAsset( "RuntimeInspector/Sprites/icon_collapsed" );
            }
            this.UpdateHierarchyItem();
        }

        public static HierarchyElement Create( HierarchyWindow window, Transform obj )
        {
            return CreateInternal( null, window, window.ViewerPanel, obj );
        }

        public static HierarchyElement Create( HierarchyElement parent, Transform obj )
        {
            return CreateInternal( parent, parent.Window, parent.List, obj );
        }

        /// <summary>
        /// Creates a new HierarchyItem that is a child of another HierarchyItem, and refers to a specific object.
        /// </summary>
        private static HierarchyElement CreateInternal( HierarchyElement hierarchyParent, HierarchyWindow window, RectTransform list, Transform obj )
        {
            InspectorStyle style = InspectorStyle.Default;

            GameObject gameObject = new GameObject( $"o_{obj.gameObject.name}" );

            RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.SetParent( list );

            rectTransform.anchorMin = new Vector2( 0.0f, 0.5f );
            rectTransform.anchorMax = new Vector2( 1.0f, 0.5f );
            rectTransform.pivot = new Vector2( 0.5f, 1.0f );
            rectTransform.anchoredPosition = new Vector2( 0.0f, 0.0f );
            rectTransform.sizeDelta = new Vector2( 0.0f, style.FieldHeight );

            Image imgRT = gameObject.AddComponent<Image>(); // add a raycastee so that you can click on this element.
            imgRT.color = new Color( 0, 0, 0, 0 );

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

            RectTransform label = InspectorLabel.Create( rectTransform, AssetRegistry<Sprite>.GetAsset( "RuntimeInspector/Sprites/icon_collapsed" ), "", style );
            TMPro.TextMeshProUGUI textMesh = label.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            textMesh.raycastTarget = false;
            hi._text = textMesh;
            Image image = label.GetComponentInChildren<Image>();
            hi._image = image;
            hi.Window = window;

            RectTransform newList = InspectorVerticalList.Create( "abc", rectTransform, style, new InspectorVerticalList.Params() { IncludeMargin = true } );
            hi.List = newList;

            hi.UpdateHierarchyItem();

            return hi;
        }

        public void OnPointerClick( PointerEventData e )
        {
            if( e.button == PointerEventData.InputButton.Right )
            {
                ToggleExpanded();
            }

            else if( e.button == PointerEventData.InputButton.Left )
            {
                Window.onSelect?.Invoke( /*new Vector3( 5, 6, 2 ) );  //*/ Obj );
            }
        }

        public void BeginDrag()
        {
            dragged = this;
            Debug.Log( $"Begin drag on {this.gameObject.name}" );
        }

        public void EndDrag()
        {
#warning TODO - add graphical elements.
            if( Input.GetKey( KeyCode.LeftShift ) )
            {
                dragged.Obj.SetParent( this.Obj );

#warning TODO - doesn't work.
                this.transform.SetParent( dragged.List );
            }
            else
            {
                if( dragged.Obj.parent == this.Obj.parent )
                {
                    int siblingIndex = this.Obj.GetSiblingIndex();
                    int draggedSiblingIndex = dragged.Obj.GetSiblingIndex();

                    // move the dragged object above or below the clicked object, depending on where the target object is relative to it.
                    // if dragged starts below target -> resulting dragged is above target.
                    // if dragged starts above target -> resulting dragged is below target.
                    int targetSiblingIndex = draggedSiblingIndex < siblingIndex ? siblingIndex : siblingIndex;
                    dragged.Obj.SetSiblingIndex( targetSiblingIndex );
                }
            }
            dragged = null;
            Debug.Log( $"End drag on {this.gameObject.name}" );
        }
    }
}
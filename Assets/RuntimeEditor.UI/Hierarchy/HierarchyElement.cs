using UnityPlus.AssetManagement;
using UnityPlus.InputSystem;
using RuntimeEditor.UI.GUIUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RuntimeEditor.UI.Hierarchy
{
    public class HierarchyElement : MonoBehaviour, IPointerClickHandler, IInputHandler_MouseDragBegin, IInputHandler_MouseDragEnd
    {
        static HierarchyElement dragged = null;

        /// <summary>
        /// The GameObject corresponding to this hierarchy item.
        /// </summary>
        public Transform ObjTransform { get; private set; }

        public GameObject Obj
        {
            get => this.ObjTransform.gameObject;
            set => this.ObjTransform = value.transform;
        }

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
            if( ObjTransform == null )
            {
                UpdateHierarchyItem();
            }
        }

        public void UpdateHierarchyItem()
        {
            if( ObjTransform == null )
            {
                Destroy( this.gameObject );
                return;
            }

            // If the hierarchy's children are stale - delete, and redraw all of them. TODO - this could potentially be optimized.
            if( this.List.childCount != this.ObjTransform.childCount )
            {
                for( int i = 0; i < this.List.childCount; i++ )
                {
                    Destroy( this.List.GetChild( i ).gameObject );
                }

                if( IsExpanded )
                {
                    for( int i = 0; i < this.ObjTransform.childCount; i++ )
                    {
                        Create( this, this.ObjTransform.GetChild( i ).gameObject );
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
            _text.text = $"[{ObjTransform.childCount}] {ObjTransform.gameObject.name}";
            if( IsExpanded )
            {
                this.List.gameObject.SetActive( true );
            }
            else
            {
                this.List.gameObject.SetActive( false );
            }

            if( this.Parent != null && ObjTransform.parent != this.Parent.ObjTransform )
            {
                this.transform.SetParent( this.Parent.List );
                this.transform.SetSiblingIndex( ObjTransform.GetSiblingIndex() );
            }
            int siblingIndex = this.transform.GetSiblingIndex();
            int objSiblingIndex = this.ObjTransform.GetSiblingIndex();
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
                this._image.sprite = AssetRegistry<Sprite>.GetAsset( "RuntimeEditor/Sprites/icon_expanded" );
            }
            else
            {
                this._image.sprite = AssetRegistry<Sprite>.GetAsset( "RuntimeEditor/Sprites/icon_collapsed" );
            }
            this.UpdateHierarchyItem();
        }

        public static HierarchyElement Create( HierarchyWindow window, GameObject obj )
        {
            return CreateInternal( null, window, window.ViewerPanel, obj );
        }

        public static HierarchyElement Create( HierarchyElement parent, GameObject obj )
        {
            return CreateInternal( parent, parent.Window, parent.List, obj );
        }

        /// <summary>
        /// Creates a new HierarchyItem that is a child of another HierarchyItem, and refers to a specific object.
        /// </summary>
        private static HierarchyElement CreateInternal( HierarchyElement hierarchyParent, HierarchyWindow window, RectTransform list, GameObject obj )
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

#warning TODo - replace with an icon button instead of label. Make label use an icon without button.
            RectTransform label = InspectorLabel.Create( rectTransform, AssetRegistry<Sprite>.GetAsset( "RuntimeEditor/Sprites/icon_collapsed" ), "", style );
            TMPro.TextMeshProUGUI textMesh = label.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            textMesh.raycastTarget = false;
            hi._text = textMesh;
            Image image = label.GetComponentInChildren<Image>();
            hi._image = image;
            hi.Window = window;
            image.raycastTarget = true;

            PointerClickHandler pc = image.gameObject.AddComponent<PointerClickHandler>();
            pc.OnClickFunc = ( x ) =>
            {
                if( x.button == PointerEventData.InputButton.Left )
                {
                    hi.ToggleExpanded();
                }
            };

            RectTransform newList = InspectorVerticalList.Create( "abc", rectTransform, style, new InspectorVerticalList.Params() { IncludeMargin = true } );
            hi.List = newList;

            hi.UpdateHierarchyItem();

            return hi;
        }

        public void OnPointerClick( PointerEventData e )
        {
            if( e.button == PointerEventData.InputButton.Left )
            {
                Window.onSelect?.Invoke( Obj );
            }
            else if( e.button == PointerEventData.InputButton.Right )
            {
                ContextMenu cm = ContextMenu.Create( GameObject.Find( "ContextMenuCanvas" ).transform, e.position );
                cm.AddOption( "Delete", () => Destroy( Obj ) );
            }
        }

        public void BeginDrag( PointerEventData e )
        {
            Debug.Log( $"Begin drag on {this.gameObject.name}" );
            dragged = this;
        }

        public void EndDrag( PointerEventData e )
        {
#warning TODO - if the user started dragging when the mouse was not over a hierarchy element, then this will still be called, but the drag was w never started.
            Debug.Log( $"End drag on {this.gameObject.name}" );
#warning TODO - add graphical elements.
            if( Input.GetKey( KeyCode.LeftShift ) )
            {
                dragged.ObjTransform.SetParent( this.ObjTransform );
                dragged.Parent = this;

                dragged.transform.SetParent( this.List );
                this.UpdateHierarchyItem();
            }
            else
            {
                if( dragged.ObjTransform.parent == this.ObjTransform.parent )
                {
                    int siblingIndex = this.ObjTransform.GetSiblingIndex();
                    int draggedSiblingIndex = dragged.ObjTransform.GetSiblingIndex();

                    // move the dragged object above or below the clicked object, depending on where the target object is relative to it.
                    // if dragged starts below target -> resulting dragged is above target.
                    // if dragged starts above target -> resulting dragged is below target.
                    int targetSiblingIndex = draggedSiblingIndex < siblingIndex ? siblingIndex : siblingIndex;
                    dragged.ObjTransform.SetSiblingIndex( targetSiblingIndex );
                }
            }
            dragged = null;
        }
    }
}
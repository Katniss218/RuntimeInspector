using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using RuntimeInspector.UI.GUIUtils;

namespace RuntimeInspector.UI
{
    // A Context Menu is a UI element that is basically a list of options.
    // An option can be either a button that does something, a spacer, or a collapsed group. Groups can be nested.

    // Only one context menu can be shown at a time.
    // when a context menu is opened, a transparent layer appears under it, to catch clicking off the menu and close it.

    public class ContextMenu : MonoBehaviour
    {
        private static ContextMenu _current;

        private static RectTransform _clickBlocker;

        /// <summary>
        /// Adds a clickable option to the context menu.
        /// </summary>
        /// <param name="text">The text that will be shown for the option.</param>
        /// <param name="onClick">The method that will be called when the option is clicked.</param>
        public void AddOption( string text, Action onClick )
        {
            InspectorStyle style = InspectorStyle.Default;

            GameObject gameObject = new GameObject( $"opt_{text}" );

            RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.SetParent( this.transform );

            rectTransform.anchorMin = new Vector2( 0.0f, 0.5f );
            rectTransform.anchorMax = new Vector2( 1.0f, 0.5f );
            rectTransform.pivot = new Vector2( 0.5f, 1.0f );
            rectTransform.anchoredPosition = new Vector2( 0.0f, 0.0f );
            rectTransform.sizeDelta = new Vector2( 0.0f, style.FieldHeight );

            Image imgRT = gameObject.AddComponent<Image>(); // add a raycastee so that you can click on this element.
            imgRT.color = new Color( 0, 0, 0, 0 );

            ContextMenuOption hi = gameObject.AddComponent<ContextMenuOption>();
            hi.OnClickFunc = ( x ) =>
            {
                onClick();
                ContextMenu.Close();
            };

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

            RectTransform label = InspectorLabel.Create( rectTransform, text, style );
            TMPro.TextMeshProUGUI textMesh = label.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            textMesh.raycastTarget = false;
        }

        /// <summary>
        /// Adds a spacer to the context menu.
        /// </summary>
        public void AddSpacer()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a group that can have multiple elements to the context menu.
        /// </summary>
        /// <param name="text">The text that will be shown for the group.</param>
        public ContextMenuGroup AddGroup( string text )
        {
            throw new NotImplementedException();
        }

        private static RectTransform CreateClickBlocker( Transform contextMenuCanvas )
        {
            GameObject gameObject = new GameObject( "Context Menu Click Blocker" );

            RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.SetParent( contextMenuCanvas );
            rectTransform.anchorMin = new Vector2( 0.0f, 0.0f );
            rectTransform.anchorMax = new Vector2( 1.0f, 1.0f );
            rectTransform.pivot = new Vector2( 0.0f, 0.5f );
            rectTransform.anchoredPosition = new Vector2( 0.0f, 0.0f );
            rectTransform.sizeDelta = new Vector2( 0.0f, 0.0f );

            Image imgRT = gameObject.AddComponent<Image>(); // add a raycastee so that you can click on this element.
            imgRT.color = new Color( 0, 0, 0, 0 );

            PointerClickHandler pc = gameObject.AddComponent<PointerClickHandler>();
            pc.OnClickFunc = ( x ) =>
            {
                if( _current != null )
                {
                    ContextMenu.Close();
                }
            };

            return rectTransform;
        }

        public static ContextMenu Create( Transform contextMenuCanvas, Vector2 screenPos )
        {
            InspectorStyle style = InspectorStyle.Default;

            if( _current != null )
            {
                throw new InvalidOperationException( "Only one context menu can be shown at a time" );
            }
            if( _clickBlocker == null )
            {
                _clickBlocker = CreateClickBlocker( contextMenuCanvas );
            }

            GameObject gameObject = new GameObject( $"Context Menu" );
            gameObject.layer = 5;

            RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.SetParent( contextMenuCanvas );
            rectTransform.anchorMin = new Vector2( 0.0f, 0.0f );
            rectTransform.anchorMax = new Vector2( 0.0f, 0.0f );
            rectTransform.pivot = new Vector2( 0.0f, 1.0f );
            rectTransform.anchoredPosition = screenPos;
            rectTransform.sizeDelta = new Vector2( 200.0f, 200.0f );

            VerticalLayoutGroup layoutGroup = gameObject.AddComponent<VerticalLayoutGroup>();
            layoutGroup.padding = new RectOffset( 0, 0, 0, 0 );
            layoutGroup.spacing = style.Spacing;
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = false;
            layoutGroup.childScaleWidth = false;
            layoutGroup.childScaleHeight = false;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = false;

            ContentSizeFitter contentSizeFitter = gameObject.AddComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            Image image = gameObject.AddComponent<Image>();
            image.color = new Color( 0.5f, 0.5f, 0.5f );
            image.raycastTarget = false;

            ContextMenu menu = rectTransform.gameObject.AddComponent<ContextMenu>();
            _current = menu;

            return menu;
        }

        public static void Close()
        {
            Destroy( _current.gameObject );
            Destroy( _clickBlocker.gameObject );
        }
    }
}
using RuntimeEditor.Core;
using RuntimeEditor.UI.Inspector;
using RuntimeEditor.UI.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RuntimeEditor.UI.GUIUtils
{
    public static class InspectorGraphNodeUI
    {
        /// <summary>
        /// Creates a new UI element for a graph node. 
        /// </summary>
        public static (RectTransform, GraphNodeUI) Create( RectTransform parent, ObjectGraphNode graphNode, InspectorStyle style )
        {
            GameObject gameObject = new GameObject( $"{graphNode.Name} ({graphNode.GetInstanceType().FullName})" );
            gameObject.layer = 5;

            RectTransform rootTransform = gameObject.AddComponent<RectTransform>();
            rootTransform.SetParent( parent );
            rootTransform.anchorMin = new Vector2( 0.0f, 0.5f );
            rootTransform.anchorMax = new Vector2( 1.0f, 0.5f );
            rootTransform.pivot = new Vector2( 0.5f, 1.0f );
            rootTransform.anchoredPosition = new Vector2( 0.0f, 0.0f );
            rootTransform.sizeDelta = new Vector2( 0.0f, style.FieldHeight );

            GraphNodeUI submitter = gameObject.AddComponent<GraphNodeUI>();
            submitter.Root = rootTransform;

#warning TODO - replace with custom layout element class that takes preferred width from the content size fitter.
            // call LayoutUtility.GetPreferredSize on the child object?

            Image image = gameObject.AddComponent<Image>(); // add raycastee
            image.color = new Color( 0.0f, 0.0f, 0.0f, 0.0f );

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

            return (rootTransform, submitter);
        }
    }
}
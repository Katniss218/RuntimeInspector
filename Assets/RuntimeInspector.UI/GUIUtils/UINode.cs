using RuntimeInspector.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RuntimeInspector.UI.GUIUtils
{
    public static class UINode
    {
        public static (RectTransform, ObjectGraphNodeUI) Create( RectTransform parent, ObjectGraphNode binding, InspectorStyle style )
        {
            GameObject gameObject = new GameObject( $"{binding.Name} ({binding.GetInstanceType().FullName})" );
            gameObject.layer = 5;

            RectTransform rootTransform = gameObject.AddComponent<RectTransform>();
            rootTransform.SetParent( parent );
            rootTransform.anchorMin = new Vector2( 0.0f, 0.5f );
            rootTransform.anchorMax = new Vector2( 1.0f, 0.5f );
            rootTransform.pivot = new Vector2( 0.5f, 1.0f );
            rootTransform.anchoredPosition = new Vector2( 0.0f, 0.0f );
            rootTransform.sizeDelta = new Vector2( 0.0f, style.FieldHeight );

            ObjectGraphNodeUI submitter = gameObject.AddComponent<ObjectGraphNodeUI>();
            //submitter.UpdateGraphNode( binding ); // ------- noNOPE this is still needed here because it looks for that.
            submitter.Root = rootTransform;

#warning TODO - replace with custom layout element class that takes preferred width from the content size fitter.
            // call LayoutUtility.GetPreferredSize on the child object?

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
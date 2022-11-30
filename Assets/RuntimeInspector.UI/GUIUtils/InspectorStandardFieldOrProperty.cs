using RuntimeInspector.Core;
using RuntimeInspector.UI.Inspector;
using RuntimeInspector.UI.ValueSelection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace RuntimeInspector.UI.GUIUtils
{
    /// <summary>
    /// Helper class to create GUI input fields.
    /// </summary>
    public static class InspectorStandardFieldOrProperty
    {
        public static RectTransform Create( RectTransform parent, Sprite typeIcon, ObjectGraphNode graphNode, InspectorStyle style )
        {
            return CreateInternal( parent, typeIcon, graphNode, style, null );
        }

        public static RectTransform Create( RectTransform parent, Sprite typeIcon, ObjectGraphNode graphNode, IEntryProvider entryProvider, InspectorStyle style )
        {
            return CreateInternal( parent, typeIcon, graphNode, style, entryProvider );
        }

        private static RectTransform CreateInternal( RectTransform parent, Sprite typeIcon, ObjectGraphNode graphNode, InspectorStyle style, IEntryProvider entryProvider )
        {
            GameObject root = new GameObject( $"{graphNode.Name} ({graphNode.GetInstanceType().FullName})" );
            root.layer = 5;

            RectTransform rootTransform = root.AddComponent<RectTransform>();
            rootTransform.SetParent( parent );
            rootTransform.anchorMin = new Vector2( 0.0f, 0.5f );
            rootTransform.anchorMax = new Vector2( 1.0f, 0.5f );
            rootTransform.pivot = new Vector2( 0.5f, 0.5f );
            rootTransform.anchoredPosition = new Vector2( 0.0f, 0.0f );
            rootTransform.sizeDelta = new Vector2( 0.0f, style.FieldHeight );

            RectTransform label = InspectorLabel.Create( rootTransform, typeIcon, graphNode.GetDisplayName(), style );

            GraphNodeUI existingGraphNodeUI = parent.GetComponent<GraphNodeUI>();

            RectTransform value;
            if( entryProvider == null )
            {
                value = InspectorInputField.Create( rootTransform, existingGraphNodeUI, graphNode, entryProvider, style );
            }
            else
            {
                value = InspectorInputField.Create( rootTransform, existingGraphNodeUI, graphNode, entryProvider, style );
            }

            value.anchorMin = new Vector2( 0.5f, 0.0f );
            value.anchorMax = new Vector2( 1.0f, 1.0f );
            value.pivot = new Vector2( 1.0f, 0.5f );
            value.anchoredPosition = new Vector2( 0.0f, 0.0f );
            value.sizeDelta = new Vector2( 0.0f, 0.0f );

            return rootTransform;
        }
    }
}
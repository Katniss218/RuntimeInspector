using RuntimeInspector.Core;
using RuntimeInspector.UI.Inspector;
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
        public static RectTransform Create( RectTransform parent, Sprite typeIcon, ObjectGraphNode binding, InspectorStyle style )
        {
            GameObject root = new GameObject( $"{binding.Name} ({binding.GetInstanceType().FullName})" );
            root.layer = 5;

            RectTransform rootTransform = root.AddComponent<RectTransform>();
            rootTransform.SetParent( parent );
            rootTransform.anchorMin = new Vector2( 0.0f, 0.5f );
            rootTransform.anchorMax = new Vector2( 1.0f, 0.5f );
            rootTransform.pivot = new Vector2( 0.5f, 0.5f );
            rootTransform.anchoredPosition = new Vector2( 0.0f, 0.0f );
            rootTransform.sizeDelta = new Vector2( 0.0f, style.FieldHeight );

            RectTransform label = InspectorLabel.Create( rootTransform, typeIcon, binding.Name, style );

            GraphNodeUI uiBinding = parent.GetComponent<GraphNodeUI>();

            RectTransform value = InspectorTextInputField.Create( rootTransform, uiBinding, binding, style );

            value.anchorMin = new Vector2( 0.5f, 0.0f );
            value.anchorMax = new Vector2( 1.0f, 1.0f );
            value.pivot = new Vector2( 1.0f, 0.5f );
            value.anchoredPosition = new Vector2( 0.0f, 0.0f );
            value.sizeDelta = new Vector2( 0.0f, 0.0f );

            return rootTransform;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RuntimeInspector.UI.GUIUtils
{
    public class InspectorStyle
    {
        public float FontSize { get; set; } = 12.0f;

        public float FieldHeight { get; set; } = 24.0f;
        public float FieldSpacing { get; set; } = 2.0f;

        public float TypeIconSize { get; set; } = 16.0f;

        public int IndentWidth { get; set; } = 16;
        public int IndentMargin { get; set; } = 8;

        public Color InputFieldColor { get; set; } = new Color( 0.5f, 0.5f, 0.5f );
        public Color InputFieldColorReadonly { get; set; } = new Color( 0.4f, 0.4f, 0.4f );
        public Color LabelTextColor { get; set; } = new Color( 1.0f, 1.0f, 1.0f );
        public Color ValueTextColor { get; set; } = new Color( 1.0f, 1.0f, 1.0f );

        public static InspectorStyle Default => new InspectorStyle();
    }
}

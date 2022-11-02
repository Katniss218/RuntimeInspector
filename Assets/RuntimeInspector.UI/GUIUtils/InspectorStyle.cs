﻿using System;
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
        public float TypeIconMargin { get; set; } = 8.0f;

        public int IndentWidth { get; set; } = 16;
        public int IndentMargin { get; set; } = 8;
        public float InputFieldMargin { get; set; } = 2.0f;
        public float Spacing { get; set; } = 4.0f;

        public Color InputFieldColor { get; set; } = new Color( 0.3f, 0.3f, 0.3f );
        public Color InputFieldColorReadonly { get; set; } = new Color( 0.36f, 0.3f, 0.3f );
        public Color InputFieldColorWriteonly { get; set; } = new Color( 0.3f, 0.36f, 0.3f );
        public Color LabelTextColor { get; set; } = new Color( 1.0f, 1.0f, 1.0f );
        public Color ValueTextColor { get; set; } = new Color( 1.0f, 1.0f, 1.0f );

        public static InspectorStyle Default => new InspectorStyle();
    }
}

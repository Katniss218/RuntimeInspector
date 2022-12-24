using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeEditor.UI
{
    public static class TMP_InputField_Ex
    {
        public static void RegenerateCaret( this TMPro.TMP_InputField value )
        {
            value.enabled = false;
            value.enabled = true; // regenerate the caret.
        }
    }
}
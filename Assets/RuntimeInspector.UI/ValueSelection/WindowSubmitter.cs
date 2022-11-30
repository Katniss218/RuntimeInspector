using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RuntimeInspector.UI.ValueSelection
{
    public class WindowSubmitter : MonoBehaviour
    {
        [SerializeField]
        TMPro.TMP_InputField _nameField;

        [SerializeField]
        ValueSelectionWindow _window;

        private void Awake()
        {
            _nameField.onSubmit.AddListener( SubmitName );
        }

        public void SubmitName( string name )
        {
            _window.RefreshFilteredEntries( SearchQuery.Empty.WithName( name ) );
        }
    }
}
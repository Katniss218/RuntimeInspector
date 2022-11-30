using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RuntimeInspector.UI.ValueSelection
{
    /// <summary>
    /// Handles the translation between UI elements and search query.
    /// </summary>
    public class ValueSelectionWindowFilterHandler : MonoBehaviour
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
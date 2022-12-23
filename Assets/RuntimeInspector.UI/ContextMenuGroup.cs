using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace RuntimeInspector.UI
{
    public class ContextMenuGroup : MonoBehaviour
    {

        /// <summary>
        /// Adds a clickable option to the context menu group.
        /// </summary>
        /// <param name="text">The text that will be shown for the option.</param>
        /// <param name="onClick">The method that will be called when the option is clicked.</param>
        public void AddOption( string text, Action onClick )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a spacer to the context menu group.
        /// </summary>
        public void AddSpacer()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a group that can have multiple elements to the context menu group.
        /// </summary>
        /// <param name="text">The text that will be shown for the group.</param>
        public ContextMenuGroup AddGroup( string text )
        {
            throw new NotImplementedException();
        }
    }
}
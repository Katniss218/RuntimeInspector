using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityPlus.InputSystem
{
    /*
    
    # ACTION INPUT SYSTEM
 
    Consists of Actions, and Input Handler.
 
 
    Action contains a list of input combinations that activate it.
 
    Input Handler manages the input and fires off the actions once their conditions have been met.
 
    We also need a registry or something that contains every action that the game uses.
 
    ## Drag&Drop.
    drag&drop system would be part of the input system.
    you click. you hold. you release.
 
    when you click, it checks what you clicked on.
    when you hold, it keeps the state from when you began.
    when you release, it checks what you released on.
 
    ## Requirements:
 
    Input Path is a sequence of combinations of key/button presses, and conditions that can stop execution early.
 
    parts of those input paths can be rebound in-game.
 
    - Actions that are assignable to multiple input paths.
    - Actions that are reassignable, despite the fact that they might not always be fired.
    So something like a unit, has an action to move, but only for that unit.
    the unit sends the action to the global input path handler that actually looks at what keys the user has pressed, etc, and tells the unit whether or not the action was successful.
    the action has to be identifyable and loaded when the unit is created. Its input path is serialized in a settings file somewhere.


    */

    public static class InputMap
    {
        // defined inputs and their identifiers.
        // these appear in the settings window, and have to be defined on startup (NOT at an arbitrary point at runtime).
        private static Dictionary<string, InputPath> _definedInputs;

        /// <summary>
        /// Checks whether or not a given input ID is defined.
        /// </summary>
        public static bool IsDefined( string id )
        {
            return _definedInputs.ContainsKey( id );
        }

        /// <summary>
        /// Sets the input path of a given input ID.
        /// </summary>
        public static void Define( string id, InputPath inputPath )
        {
            // this will be called by either the settings loader or by the class defining the default path for an input.

            _definedInputs[id] = inputPath;
        }
    }

    public class InputPath
    {
        // specifies which buttons need to be pressed/held/released as well as when and how.

        // we want this to support double-clicks, etc.
        // Double-clicks don't happen instantaneously.
        // We need to either store the previous states, or add a given input action to a queue and run a provided callable when the input manager determines that the condition was met.

        /// <summary>
        /// Returns true 
        /// </summary>
        /// <returns>True if the given input path has completed successfully. False otherwise.</returns>
        public bool Run()
        {
            throw new NotImplementedException();
        }
    }

    // we need something to identify which actions are which when the user rebinds them.
}
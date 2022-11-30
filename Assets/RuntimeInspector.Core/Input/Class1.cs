using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.RuntimeInspector.Core.Input
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

    public static class InputHandler
    {
        public static List<string> CurrentlyBlockedActions; 
        // if we press attack on horseback, we don't want to swing our weapon. So we have to block that action when we get on, and unblock when we get off.
    }

    public class InputPath
    {
        // specifies which buttons need to be pressed/held/released as well as when and how.

        public bool Check()
        {
            throw new NotImplementedException();
        }
    }

    // we need something to identify which actions are which when the user rebinds them.
}
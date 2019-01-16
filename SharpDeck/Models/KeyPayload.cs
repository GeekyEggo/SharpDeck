﻿namespace SharpDeck.Models
{
    public class KeyPayload : ActionPayload
    {
        /// <summary>
        /// Gets or sets a value that is set when the action is triggered with a specific value from a Multi Action.
        /// For example if the user sets the Game Capture Record action to be disabled in a Multi Action, you would see the value 1. Only the value 0 and 1 are valid.
        /// </summary>
        public int UserDesiredState { get; set; }
    }
}

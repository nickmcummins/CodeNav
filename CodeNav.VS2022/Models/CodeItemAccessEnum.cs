﻿using System.ComponentModel;

namespace CodeNav.Models
{
    public enum CodeItemAccessEnum
    {
        [Description("")]
        Unknown,
        [Description("All")]
        All,
        [Description("Public")]
        Public,
        [Description("Private")]
        Private,
        [Description("Protected")]
        Protected,
        [Description("Internal")]
        Internal,
        [Description("Sealed")]
        Sealed
    }
}

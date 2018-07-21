namespace AssimilateNode.I2cBus
{
    /// <summary>
    /// Identifies the rolew of the I2C slave
    /// </summary>
    public enum Role
    {
        UNDEFINED = -1,
        SENSOR = 0,
        ACTOR = 1,
    };
}

namespace System.Diagnostics
{
    /// <summary>
    /// Needed by the compiler on netduino
    /// </summary>
    public enum DebuggerBrowsableState
    {
        Never = 0,
        Collapsed = 2,
        RootHidden = 3
    }
}

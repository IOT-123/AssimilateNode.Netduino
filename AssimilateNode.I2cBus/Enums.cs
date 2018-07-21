namespace AssimilateNode.I2cBus
{
    /// <summary>
    /// Identifies the rolew of the I2C slave
    /// </summary>
    public enum Role
    {
        /// <summary>
        /// Not set
        /// </summary>
        UNDEFINED = -1,
        /// <summary>
        /// Is a sensor
        /// </summary>
        SENSOR = 0,
        /// <summary>
        /// Is an actor
        /// </summary>
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
        /// <summary>
        /// Never
        /// </summary>
        Never = 0,
        /// <summary>
        /// Collapsed
        /// </summary>
        Collapsed = 2,
        /// <summary>
        /// RootHidden
        /// </summary>
        RootHidden = 3
    }
}

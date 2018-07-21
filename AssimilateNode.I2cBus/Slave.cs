namespace AssimilateNode.I2cBus
{
    /// <summary>
    /// Base Information derived from metadata for each slave
    /// </summary>
    public class Slave    {
        /// <summary>
        /// I2C Address
        /// </summary>
        public byte address { get; set; }
        /// <summary>
        /// Internal name of slave
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Role of Slave
        /// </summary>
        public Role role { get; set; }
        /// <summary>
        /// Clock Stretch used (on Arduino I2C BUS)
        /// </summary>
        public int clock_stretch { get; set; }
    }
}


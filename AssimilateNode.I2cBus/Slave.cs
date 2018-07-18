namespace AssimilateNode.I2cBus
{
    public class Slave    {
        public byte address { get; set; }
        public string name { get; set; }
        public Role role { get; set; }
        public int clock_stretch { get; set; }
    }
}


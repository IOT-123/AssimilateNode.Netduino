using System;
using Microsoft.SPOT;
using Netduino.Foundation.Communications;
using System.Collections;
using AssimilateNode.Core;

namespace AssimilateNode.I2cBus
{

    /// <summary>
    /// Arguments for PropertyReceived events
    /// </summary>
    public class PropertyReceivedventArgs : EventArgs
    {
        /// <summary>
        /// Slave I2C address
        /// </summary>
        public byte slaveAddress { get; set; }
        /// <summary>
        /// Index of property sent as defined on each slave
        /// </summary>
        public byte propertyIndex { get; set; }
        /// <summary>
        /// Slave role: ACTOR/SENSOR
        /// </summary>
        public Role role { get; set; }
        /// <summary>
        /// Name of property received
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Value of property received
        /// </summary>
        public string value { get; set; }
    }

    /// <summary>
    /// Entry point for I2C comunication
    /// </summary>
    public class I2cCommunication
    {

        /// <summary>
        /// Event when a single name/value property is received
        /// </summary>
        public static event PropertyReceivedDelegate PropertyReceived;
        /// <summary>
        /// Delegate for PropertyReceived event
        /// </summary>
        public delegate void PropertyReceivedDelegate(object sender, PropertyReceivedventArgs e);

        /// <summary>
        /// Event when a complete cycle of slaves completing a dump of properties
        /// </summary>
        public static event SlaveCyleCompleteDelegate SlaveCyleComplete;
        /// <summary>
        /// Delegate for SlaveCyleComplete event
        /// </summary>
        public delegate void SlaveCyleCompleteDelegate(object sender, EventArgs e);

        private ArrayList _assimSlaves;
        private const byte SLAVE_SCAN_LOW = 8;
        private const byte SLAVE_SCAN_HIGH = 30;



        /// <summary>
        /// Constructor for class
        /// </summary>
        public I2cCommunication()
        {
            _assimSlaves = new ArrayList();
            scanBusAddresses();
        }

        /// <summary>
        /// Get metadata from slaves, store in _assimSlaves and JSON files
        /// </summary>
        public void getMetadata()
        {
            Debug.Print(MethodNames.GET_METADATA);
            bool i2cNodeProcessed;
            string name = "", value = "";
            ushort speed = 1000;
            ushort timeout = 200;
            foreach (Slave slave in _assimSlaves)
            {
                byte slaveAddress = slave.address;
                Hashtable rootPairs = new Hashtable();
                ArrayList userMetas = Config.deserializeUserMetas(slaveAddress); 
                I2CBus i2C = new I2CBus(slaveAddress, speed, timeout);
                while (true) {
                    i2cNodeProcessed = false;
                    for (byte segment = 0; segment < 3; segment++) { // 3 requests per meta
                        try {
                            var byteArray = i2C.ReadBytes(16);
                            var packet = getPacketFromBytes(byteArray);
                            switch (segment) {
                                case 0:
                                    name = packet;
                                    break;
                                case 1:
                                    if (name == packet)
                                    {// this is the main symptom of message sequence is out of sync - not experienced on Netduino node
                                        Debug.Print(LogMessages.OUT_OF_SYNC);
                                        Debug.Print(LogMessages.RESTARTING);
                                        // ToDo: restart the device
                                    }
                                    value = packet;
                                    getUserMetaOrDefault(userMetas, name, ref value);
                                    setMetaOfInterest(slave, rootPairs, name, value);
                                    break;
                                case 2:
                                    Debug.Print(name + (name.Length > 6 ? LogMessages.TAB_1 : LogMessages.TAB_2) + value);
                                    // check if last metadata
                                    if (packet == I2cMessages.SEG3_DISCONTINUE) {// 0 on last property
                                        i2cNodeProcessed = true;
                                        break;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        } catch (Exception ex) {
                            Debug.Print(ex.InnerException.Message);
                            return;
                        }
                    }// end for segment  
                    if (i2cNodeProcessed) { // break out if last metadata
                        break;
                    }
                }// end while true
                Debug.Print(LogMessages.CONFIRM_METADATA + slave.name);
                i2C.WriteByte(I2cMessages.WRITE_CONFIRM_METADATA);
                // SAVE /config/metadata/<i>.json
                if (!Config.serializeSlaveMetas(slaveAddress, rootPairs))
                {
                    Debug.Print(LogMessages.ERROR_SAVING_FILE + slaveAddress.ToString());
                }
            }// end foreach slave
        }

        private string getPacketFromBytes(byte[] byteArray)
        {
            var packet = "";
            foreach (byte b in byteArray)
            {
                if (b != 255)
                {
                    packet += Convert.ToChar(b).ToString();
                }
            }
            return packet;
        }

        /// <summary>
        /// Get properties from slaves, raise events for individual properties and completion
        /// </summary>
        public void getProperties()
        {
            bool i2cNodeProcessed;
            ushort speed = 1000;
            ushort timeout = 200;
            string name = "", value = "";
            foreach (Slave slave in _assimSlaves)
            {
                byte slaveAddress = slave.address;
                byte propIndex = 0;
                //ToDo: clock stretching
                I2CBus i2C = new I2CBus(slaveAddress, speed, timeout);
                while (true)
                {
                    i2cNodeProcessed = false;
                    for (byte segment = 0; segment < 3; segment++)
                    { // 3 requests per meta
                        try
                        {
                            var byteArray = i2C.ReadBytes(16);
                            var packet = getPacketFromBytes(byteArray);
                            switch (segment)
                            {
                                case 0:
                                    name = packet;
                                    break;
                                case 1:
                                    value = packet;
                                    break;
                                case 2:
                                    var args = new PropertyReceivedventArgs {
                                        slaveAddress = slaveAddress,
                                        propertyIndex = propIndex,
                                        role = slave.role,
                                        name = name,
                                        value = value
                                    };
                                    PropertyReceived?.Invoke(this, args);
                                    propIndex++;
                                    if (packet == I2cMessages.SEG3_DISCONTINUE)
                                    {// 0 on last property
                                        i2cNodeProcessed = true;
                                        break;
                                    }
                                    break;
                                default:
                                    break;
                            }

                        }
                        catch
                        {

                        }
                    }
                    if (i2cNodeProcessed)
                    { // break out if last property for slave
                        break;
                    }
                }
            }
            SlaveCyleComplete?.Invoke(null, EventArgs.Empty);
        }

        private void setMetaOfInterest(Slave slave, Hashtable rootPairs, String i2cName, String value)
        {
            switch (i2cName)
            {
                case I2cMessages.ASSIM_NAME:
                    slave.name = value;
                    return;
                case I2cMessages.ASSIM_VERSION:
                    return;
                case I2cMessages.POWER_DOWN:
                    return;
                case I2cMessages.PREPARE_MS:
                    return;
                case I2cMessages.RESPONSE_MS:
                    return;
                case I2cMessages.VCC_MV:
                    return;
                case I2cMessages.ASSIM_ROLE:
                    slave.role = value == I2cMessages.SENSOR ? Role.SENSOR : Role.ACTOR;
                    return;
                case I2cMessages.CLOCK_STRETCH:
                    slave.clock_stretch = Convert.ToInt32(value);
                    return;
                default:
                    break;// end of base properties
            }
            var valueParts = value.Split(':');
            var subValueParts = valueParts[1].Split('|');
            var idxStr = valueParts[0];
            var idx = Convert.ToInt32(idxStr);
            var propKey = "";
            if (!rootPairs.Contains(idxStr))
            {
                rootPairs.Add(idxStr, new Hashtable());
            }
            Hashtable propertyIdxHashtable = ((Hashtable)rootPairs[idxStr]);
            // property based metadata
            switch (i2cName)
            {
                case I2cMessages.VIZ_CARD_TYPE:
                    value = valueParts[1];
                    propertyIdxHashtable.AddOrUpdate(VizJsonKeys.CARD_TYPE, value);
                    setDefaultsForCardTtype(value, propertyIdxHashtable);
                    return;
                case I2cMessages.VIZ_ICONS:
                    propKey = subValueParts[0];
                    value = subValueParts[1];
                    if (!propertyIdxHashtable.Contains(VizJsonKeys.ICONS))
                    {
                        propertyIdxHashtable.Add(VizJsonKeys.ICONS, new ArrayList());
                    }
                    var leafHashtable = new Hashtable();
                    leafHashtable.Add(HashtableKeys.NAME, propKey);
                    leafHashtable.Add(HashtableKeys.VALUE, value);
                    ((ArrayList)propertyIdxHashtable[VizJsonKeys.ICONS]).Add(leafHashtable);
                    return;
                case I2cMessages.VIZ_LABELS:
                    propKey = subValueParts[0];
                    value = subValueParts[1];
                    if (!propertyIdxHashtable.Contains(VizJsonKeys.LABELS))
                    {
                        propertyIdxHashtable.Add(VizJsonKeys.LABELS, new ArrayList());
                    }
                    var leafHashtable2 = new Hashtable();
                    leafHashtable2.Add(HashtableKeys.NAME, propKey);
                    leafHashtable2.Add(HashtableKeys.VALUE, value);
                    ((ArrayList)propertyIdxHashtable[VizJsonKeys.LABELS]).Add(leafHashtable2);
                    return;
                case I2cMessages.VIZ_MIN:
                    value = valueParts[1];
                    propertyIdxHashtable.AddOrUpdate(VizJsonKeys.MIN, Convert.ToInt32(value));
                    return;
                case I2cMessages.VIZ_MAX:
                    value = valueParts[1];
                    propertyIdxHashtable.AddOrUpdate(VizJsonKeys.MAX, Convert.ToInt32(value));
                    return;
                case I2cMessages.VIZ_UNITS:
                    value = valueParts[1];
                    propertyIdxHashtable.AddOrUpdate(VizJsonKeys.UNITS, value);
                    return;
                case I2cMessages.VIZ_TOTAL:
                    value = valueParts[1];
                    propertyIdxHashtable.AddOrUpdate(VizJsonKeys.TOTAL, Convert.ToInt32(value));
                    return;
                case I2cMessages.VIZ_VALUES:
                    propKey = subValueParts[0];
                    value = subValueParts[1];
                    if (!propertyIdxHashtable.Contains(VizJsonKeys.VALUES))
                    {
                        propertyIdxHashtable.AddOrUpdate(VizJsonKeys.VALUES, new ArrayList());
                    }
                    if (((ArrayList)propertyIdxHashtable[VizJsonKeys.VALUES]).Count == 2)
                    {// remove the defaults - ToDo: more robust strategy
                        ((ArrayList)propertyIdxHashtable[VizJsonKeys.VALUES]).RemoveAt(0);
                        ((ArrayList)propertyIdxHashtable[VizJsonKeys.VALUES]).RemoveAt(0);
                    }
                    var leafHashtable3 = new Hashtable();
                    leafHashtable3.Add(HashtableKeys.NAME, propKey);
                    leafHashtable3.Add(HashtableKeys.VALUE, value);
                    ((ArrayList)propertyIdxHashtable[VizJsonKeys.VALUES]).Add(leafHashtable3);
                    return;
                case I2cMessages.VIZ_IS_SERIES:
                    value = valueParts[1];
                    bool valueBool = value == JsonValues.IS_SEREIES_VALUE;
                    propertyIdxHashtable.AddOrUpdate(VizJsonKeys.IS_SERIES, valueBool);
                    if (valueBool)
                    {
                        setDefaultsForCardTtype(I2cMessages.CARDTYPE_CHART_LINE, propertyIdxHashtable);
                    }
                    return;
                case I2cMessages.VIZ_M_SERIES:
                    return;
                case I2cMessages.VIZ_HIGH:
                    value = valueParts[1];
                    propertyIdxHashtable.AddOrUpdate(VizJsonKeys.HIGH, Convert.ToInt32(value));
                    return;
                case I2cMessages.VIZ_LOW:
                    value = valueParts[1];
                    propertyIdxHashtable.AddOrUpdate(VizJsonKeys.LOW, Convert.ToInt32(value));
                    return;
                case I2cMessages.VIZ_TOTL_UNIT:
                    value = subValueParts[0];
                    propertyIdxHashtable.AddOrUpdate(VizJsonKeys.TOTAL, Convert.ToInt32(value));
                    value = subValueParts[1];
                    propertyIdxHashtable.AddOrUpdate(VizJsonKeys.UNITS, value);
                    return;
            }
        }

        private void setDefaultsForCardTtype(string cardType, Hashtable propertyIdxHashtable)
        {
            //Debug.Print("setDefaultsForCardTtype");
            switch (cardType)
            {
                case I2cMessages.CARDTYPE_TOGGLE:
                case I2cMessages.CARDTYPE_INPUT:
                case I2cMessages.CARDTYPE_SLIDER:
                case I2cMessages.CARDTYPE_BUTTON:
                    return;
                case I2cMessages.CARDTYPE_TEXT:
                    propertyIdxHashtable.AddOrUpdate(VizJsonKeys.UNITS, JsonValues.TEXT_STATUS);
                    return;
                case I2cMessages.CARDTYPE_CHART_DONUT:
                    propertyIdxHashtable.AddOrUpdate(VizJsonKeys.TOTAL, JsonValues.CHART_DONUT_TOTAL);
                    propertyIdxHashtable.AddOrUpdate(VizJsonKeys.UNITS, JsonValues.CHART_DONUT_UNITS);
                    propertyIdxHashtable.AddOrUpdate(VizJsonKeys.VALUES, new ArrayList());
                    var leafHashtable1 = new Hashtable();
                    leafHashtable1.AddOrUpdate(HashtableKeys.NAME, VizJsonKeys.LABELS);
                    leafHashtable1.AddOrUpdate(HashtableKeys.VALUE, JsonValues.CHART_DONUT_VALUE_LABELS);
                    ((ArrayList)propertyIdxHashtable[VizJsonKeys.VALUES]).Add(leafHashtable1);
                    var leafHashtable2 = new Hashtable();
                    leafHashtable2.AddOrUpdate(HashtableKeys.NAME, VizJsonKeys.SERIES);
                    leafHashtable2.AddOrUpdate(HashtableKeys.VALUE, JsonValues.CHART_DONUT_VALUE_SERIES);
                    ((ArrayList)propertyIdxHashtable[VizJsonKeys.VALUES]).Add(leafHashtable2);
                    return;
                case I2cMessages.CARDTYPE_CHART_LINE:
                    propertyIdxHashtable.AddOrUpdate(VizJsonKeys.MAX, 12);
                    propertyIdxHashtable.AddOrUpdate(VizJsonKeys.LOW, 0);
                    propertyIdxHashtable.AddOrUpdate(VizJsonKeys.HIGH, 100);
                    return;
            }
        }

        private void getUserMetaOrDefault(ArrayList userMetas, string i2cName, ref string value) {
            if (i2cName.IndexOf(I2cMessages.VIZ_PREFIX) != 0) { // VIZ_ metadata only
                return;
            }
            var slaveValueParts = value.Split(':');
            var slavePropIdx = Convert.ToByte(slaveValueParts[0]);
            foreach (Hashtable hashtable in userMetas) {
                if (hashtable[HashtableKeys.NAME].ToString() == i2cName) {
                    var userValueParts = hashtable[HashtableKeys.VALUE].ToString().Split(':');
                    var userPropIdx = Convert.ToByte(userValueParts[0]);
                    if (slavePropIdx == userPropIdx) {
                        if (i2cName == I2cMessages.VIZ_ICONS)// has dual indexes 12:0|abc
                        {
                            var userSubParts = userValueParts[1].Split('|');
                            var userSubIdx = Convert.ToByte(userSubParts[0]);
                            var slaveSubParts = slaveValueParts[1].Split('|');
                            var slaveSubIdx = Convert.ToByte(slaveSubParts[0]);
                            if (slaveSubIdx == userSubIdx) {
                                value = hashtable[HashtableKeys.VALUE].ToString();
                                return;
                            }
                        } else {
                            value = hashtable[HashtableKeys.VALUE].ToString();
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Print the _assimSlaves details to Debug output
        /// </summary>
        public void printMetadata()
        {
            Debug.Print(MethodNames.PRINT_METADATA);
            Debug.Print(LogMessages.DEVICE_COUNT + _assimSlaves.Count.ToString());
            foreach (Slave slave in _assimSlaves) {
                Debug.Print(slave.address.ToString() + LogMessages.TAB_1 + slave.name + (slave.name.Length > 6 ? LogMessages.TAB_1 : LogMessages.TAB_2) + getRoleName(slave.role) + LogMessages.TAB_1 + slave.clock_stretch);
            }
        }

        private string getRoleName(Role role)
        {
            switch (role)
            {
                case Role.ACTOR:
                    return I2cMessages.ACTOR;
                case Role.SENSOR:
                    return I2cMessages.SENSOR;
                default:
                    return "UNDEFINED";
            }
        }

        private void scanBusAddresses()
        {
            Debug.Print(MethodNames.SCAN_BUS_ADDRESSES);
            if (loadAddressWhitelist())
            {
                Debug.Print(LogMessages.LOADED_WHITELIST);
                return;
            }
            ushort speed = 100;
            ushort timeout = 200;
            for (byte i = SLAVE_SCAN_LOW; i < SLAVE_SCAN_HIGH; i++)
            {
                I2CBus i2C = new I2CBus(i, speed, timeout);
                try
                {
                    i2C.WriteByte(0);
                    var slaveAddressOnly = new Slave { address = i, clock_stretch = 40000, name = "not_assigned", role = Role.UNDEFINED};
                    _assimSlaves.Add(slaveAddressOnly);
                }
                catch { }
            }
            if (_assimSlaves.Count == 0)
            {
                //Debug.Print("No I2C devices found.");
            }
            else
            {
                //Debug.Print(_numberOfDevices.ToString() + " I2C devices found.");
            }
        }

        private bool loadAddressWhitelist()
        {
            var whitelist = Config.getWhitelistArrayList();
            if (whitelist == null)
            {
                return false;
            }
            foreach (Hashtable item in whitelist)
            {
                _assimSlaves.Add(new Slave { address = Convert.ToByte(item[HashtableKeys.VALUE].ToString()), clock_stretch = 40000, name = "not_assigned", role = Role.UNDEFINED });
            }
            return true;
        }
    }
}

using System;
using Microsoft.SPOT;
using Netduino.Foundation.Communications;
using System.Collections;
using AssimilateNode.Core;

namespace AssimilateNode.I2cBus
{
    public class I2cCommunication
    {


        private ArrayList _assimSlaves;
        private const byte SLAVE_SCAN_LOW = 8;
        private const byte SLAVE_SCAN_HIGH = 30;

        public I2cCommunication()
        {
            _assimSlaves = new ArrayList();
            scanBusAddresses();
        }

        public void getMetadata()
        {
            Debug.Print(MethodNames.GET_METADATA);
            bool i2cNodeProcessed;
            string name = "", value = "";
            ushort speed = 100;
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
                            String packet = "";
                            foreach (byte b in byteArray) {
                                if (b != 255) {
                                    packet += Convert.ToChar(b).ToString();
                                }
                            }
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

        public void getProperties()
        {


            //char packet[16];
            //char name[16];
            //char value[16];
            //bool i2cNodeProcessed;
            //int intOfChar;
            //_debug_assim.out_fla(F("[I2C REQUEST SENSOR VALUES]\tSLAVE_ADDR\tPROP_IDX\tSEGMENT_IDX"), true, 1);
            //for (byte index = 0; index < _deviceCount; index++)
            //{
            //    byte prop_index = 0;
            //    byte slave_address = _assimSlaves[index].address;
            //    while (true)
            //    {
            //        i2cNodeProcessed = false;
            //        for (byte segment = 0; segment < 3; segment++)
            //        {
            //            _debug_assim.out_fla(F("[I2C REQUEST SENSOR VALUES]\t\t"), false, 1);
            //            _debug_assim.out_str(String(slave_address), false, 1);
            //            _debug_assim.out_fla(F("\t\t"), false, 1);
            //            _debug_assim.out_str(String(prop_index), false, 1);
            //            _debug_assim.out_fla(F("\t\t"), false, 1);
            //            _debug_assim.out_str(String(segment), true, 1);
            //            Wire.setClockStretchLimit(_assimSlaves[index].clock_stretch);
            //            Wire.requestFrom(slave_address, 16);
            //            byte writeIndex = 0;
            //            while (Wire.available())
            //            { // slave may send less than requested
            //                const char c = Wire.read();
            //                intOfChar = int(c);
            //                packet[writeIndex] = intOfChar > -1 && intOfChar < 255 ? c : 0x00;// replace invalid chars with zero char      
            //                writeIndex++;
            //            }// end while wire.available
            //            switch (segment)
            //            {
            //                case 0:
            //                    // set property name
            //                    strcpy(name, packet);
            //                    break;
            //                case 1:
            //                    // set property value            
            //                    strcpy(value, packet);
            //                    break;
            //                case 2:
            //                    // bubble up the name / value pairs
            //                    nvCallback(slave_address, prop_index, _assimSlaves[index].role, name, value);
            //                    prop_index++;
            //                    // check if last metadata
            //                    if (int(packet[0]) != 49)
            //                    {// 0 on last property
            //                        i2cNodeProcessed = true;
            //                        break;
            //                    }
            //                default:;
            //            }
            //        }// end for segment
            //        if (i2cNodeProcessed)
            //        { // break out of true loop if last metadata
            //            break;
            //        }
            //    }// end while true
            //}// end for index	
            //spCallback();


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

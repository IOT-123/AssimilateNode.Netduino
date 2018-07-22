
namespace AssimilateNode.Core
{

    /// <summary>
    /// Names of methods logged
    /// </summary>
    public static class MethodNames
    {
        /// <summary>
        /// "AssimilateNode.I2cBus.I2cCommunication..getMetadata"
        /// </summary>
        public const string GET_METADATA = "AssimilateNode.I2cBus.I2cCommunication.getMetadata";
        /// <summary>
        /// "AssimilateNode.I2cBus.I2cCommunication.getProperties"
        /// </summary>
        public const string GET_PROPERTIES = "AssimilateNode.I2cBus.I2cCommunication.getProperties";
        /// <summary>
        /// "AssimilateNode.I2cBus.I2cCommunication.printMetadata"
        /// </summary>
        public const string PRINT_METADATA = "AssimilateNode.I2cBus.I2cCommunication.printMetadata";
        /// <summary>
        /// "AssimilateNode.I2cBus.I2cCommunication.scanBusAddresses"
        /// </summary>
        public const string SCAN_BUS_ADDRESSES = "AssimilateNode.I2cBus.I2cCommunication.scanBusAddresses";
     }

    /// <summary>
    /// Strings used in Log Messages
    /// </summary>
    public static class LogMessages
    {
        /// <summary>
        /// "OUT OF SYNC"
        /// </summary>
        public const string OUT_OF_SYNC = "OUT OF SYNC";
        /// <summary>
        /// "RESTARTING"
        /// </summary>
        public const string RESTARTING = "RESTARTING";
        /// <summary>
        /// "\t"
        /// </summary>
        public const string TAB_1 = "\t";
        /// <summary>
        /// "\t\t"
        /// </summary>
        public const string TAB_2 = "\t\t";
        /// <summary>
        /// "------------------------CONFIRMING METADATA FOR "
        /// </summary>
        public const string CONFIRM_METADATA = "------------------------CONFIRMING METADATA FOR ";
        /// <summary>
        /// "DEVICE COUNT: "
        /// </summary>
        public const string DEVICE_COUNT = "DEVICE COUNT: ";
        /// <summary>
        /// "Addresses loaded from whitelist"
        /// </summary>
        public const string LOADED_WHITELIST = "Addresses loaded from whitelist";
        /// <summary>
        /// "Error saving file: "
        /// </summary>
        public const string ERROR_SAVING_FILE = "Error saving file: ";
    }

    /// <summary>
    /// Standard messages sent over I2C
    /// </summary>
    public static class I2cMessages
    {
        /// <summary>
        /// "0"
        /// </summary>
        public static string SEG3_DISCONTINUE => "0";
        /// <summary>
        /// 1
        /// </summary>
        public static byte WRITE_CONFIRM_METADATA => 1;
        /// <summary>
        /// "ASSIM_NAME"
        /// </summary>
        public const string ASSIM_NAME = "ASSIM_NAME";
        /// <summary>
        /// "ASSIM_VERSION"
        /// </summary>
        public const string ASSIM_VERSION = "ASSIM_VERSION";
        /// <summary>
        /// "ASSIM_ROLE"
        /// </summary>
        public const string ASSIM_ROLE = "ASSIM_ROLE";
        /// <summary>
        /// "POWER_DOWN"
        /// </summary>
        public const string POWER_DOWN = "POWER_DOWN";
        /// <summary>
        /// "PREPARE_MS"
        /// </summary>
        public const string PREPARE_MS = "PREPARE_MS";
        /// <summary>
        /// "RESPONSE_MS"
        /// </summary>
        public const string RESPONSE_MS = "RESPONSE_MS";
        /// <summary>
        /// "VCC_MV"
        /// </summary>
        public const string VCC_MV = "VCC_MV";
        /// <summary>
        /// "CLOCK_STRETCH"
        /// </summary>
        public const string CLOCK_STRETCH = "CLOCK_STRETCH";
        /// <summary>
        /// "VIZ_"
        /// </summary>
        public const string VIZ_PREFIX = "VIZ_";
        /// <summary>
        /// "VIZ_CARD_TYPE"
        /// </summary>
        public const string VIZ_CARD_TYPE = "VIZ_CARD_TYPE";
        /// <summary>
        /// "VIZ_ICONS"
        /// </summary>
        public const string VIZ_ICONS = "VIZ_ICONS";
        /// <summary>
        /// "VIZ_LABELS"
        /// </summary>
        public const string VIZ_LABELS = "VIZ_LABELS";
        /// <summary>
        /// "VIZ_MIN"
        /// </summary>
        public const string VIZ_MIN = "VIZ_MIN";
        /// <summary>
        /// "VIZ_MAX"
        /// </summary>
        public const string VIZ_MAX = "VIZ_MAX";
        /// <summary>
        /// "VIZ_UNITS"
        /// </summary>
        public const string VIZ_UNITS = "VIZ_UNITS";
        /// <summary>
        /// "VIZ_TOTAL"
        /// </summary>
        public const string VIZ_TOTAL = "VIZ_TOTAL";
        /// <summary>
        /// "VIZ_VALUES"
        /// </summary>
        public const string VIZ_VALUES = "VIZ_VALUES";
        /// <summary>
        /// "VIZ_IS_SERIES"
        /// </summary>
        public const string VIZ_IS_SERIES = "VIZ_IS_SERIES";
        /// <summary>
        /// "VIZ_M_SERIES" not used yet
        /// </summary>
        public const string VIZ_M_SERIES = "VIZ_M_SERIES";
        /// <summary>
        /// "VIZ_HIGH"
        /// </summary>
        public const string VIZ_HIGH = "VIZ_HIGH";
        /// <summary>
        /// "VIZ_LOW"
        /// </summary>
        public const string VIZ_LOW = "VIZ_LOW";
        /// <summary>
        /// "VIZ_TOTL_UNIT"
        /// </summary>
        public const string VIZ_TOTL_UNIT = "VIZ_TOTL_UNIT";
        /// <summary>
        /// "toggle"
        /// </summary>
        public const string CARDTYPE_TOGGLE = "toggle";
        /// <summary>
        /// "input"
        /// </summary>
        public const string CARDTYPE_INPUT = "input";
        /// <summary>
        /// "slider"
        /// </summary>
        public const string CARDTYPE_SLIDER = "slider";
        /// <summary>
        /// "button"
        /// </summary>
        public const string CARDTYPE_BUTTON = "button";
        /// <summary>
        /// "text"
        /// </summary>
        public const string CARDTYPE_TEXT = "text";
        /// <summary>
        /// "chart-donut"
        /// </summary>
        public const string CARDTYPE_CHART_DONUT = "chart-donut";
        /// <summary>
        /// "chart-line"
        /// </summary>
        public const string CARDTYPE_CHART_LINE = "chart-line";
        /// <summary>
        /// "SENSOR"
        /// </summary>
        public const string SENSOR = "SENSOR";
        /// <summary>
        /// "ACTOR"
        /// </summary>
        public const string ACTOR = "ACTOR";
    }

    /// <summary>
    /// JSON keys for Crouton
    /// </summary>
    public static class VizJsonKeys
    {
        /// <summary>
        /// "card_type"
        /// </summary>
        public const string CARD_TYPE = "card_type";
        /// <summary>
        /// "icons"
        /// </summary>
        public const string ICONS = "icons";
        /// <summary>
        /// "units"
        /// </summary>
        public const string UNITS = "units";
        /// <summary>
        /// "total"
        /// </summary>
        public const string TOTAL = "total";
        /// <summary>
        /// "values"
        /// </summary>
        public const string VALUES = "values";
        /// <summary>
        /// "value"
        /// </summary>
        public const string VALUE = "value";
        /// <summary>
        /// "labels"
        /// </summary>
        public const string LABELS = "labels";
        /// <summary>
        /// "series"
        /// </summary>
        public const string SERIES = "series";
        /// <summary>
        /// "min"
        /// </summary>
        public const string MIN = "min";
        /// <summary>
        /// "max"
        /// </summary>
        public const string MAX = "max";
        /// <summary>
        /// "low"
        /// </summary>
        public const string LOW = "low";
        /// <summary>
        /// "high"
        /// </summary>
        public const string HIGH = "high";
        /// <summary>
        /// "is_series"
        /// </summary>
        public const string IS_SERIES = "is_series";
    }

    /// <summary>
    /// JSON values for Crouton
    /// </summary>
    public static class JsonValues
    {
        /// <summary>
        /// "STATUS"
        /// </summary>
        public const string TEXT_STATUS = "STATUS";
        /// <summary>
        /// 100
        /// </summary>
        public const int CHART_DONUT_TOTAL = 100;
        /// <summary>
        /// "%"
        /// </summary>
        public const string CHART_DONUT_UNITS = "%";
        /// <summary>
        /// "[]"
        /// </summary>
        public const string CHART_DONUT_VALUE_LABELS = "[]";
        /// <summary>
        /// "0"
        /// </summary>
        public const string CHART_DONUT_VALUE_SERIES = "0";
        /// <summary>
        /// 12
        /// </summary>
        public const int CHART_LINE_MAX = 12;
        /// <summary>
        /// 0
        /// </summary>
        public const int CHART_LINE_LOW = 0;
        /// <summary>
        /// 100
        /// </summary>
        public const int CHART_LINE_HIGH = 100;
        /// <summary>
        /// "true"
        /// </summary>
        public const string IS_SEREIES_VALUE = "true";
    }

    /// <summary>
    /// JSON keys for \SD\config\device.json
    /// </summary>
    public static class DeviceJsonKeys
    {
        /// <summary>
        /// "www_auth_username"
        /// </summary>
        public const string WWW_AUT_USERNAME = "www_auth_username";
        /// <summary>
        /// "www_auth_password"
        /// </summary>
        public const string WWW_AUTH_PASSWORD = "www_auth_password";
        /// <summary>
        /// "www_auth_exclude_files"
        /// </summary>
        public const string WWW_AUTH_EXCLUDE_FILES = "www_auth_exclude_files";
        /// <summary>
        /// "sensor_interval"
        /// </summary>
        public const string SENSOR_INTERVAL = "sensor_interval";
        /// <summary>
        /// "ntp_server_name"
        /// </summary>
        public const string NTP_SERVER_NAME = "ntp_server_name";
        /// <summary>
        /// "time_zone"
        /// </summary>
        public const string TIME_ZONE = "time_zone";
        /// <summary>
        /// "wifi_ssid"
        /// </summary>
        public const string WIFI_SSID = "wifi_ssid";
        /// <summary>
        /// "wifi_key"
        /// </summary>
        public const string WIFI_KEY = "wifi_key";
        /// <summary>
        /// "mqtt_broker"
        /// </summary>
        public const string MQTT_BROKER = "mqtt_broker";
        /// <summary>
        /// "mqtt_username"
        /// </summary>
        public const string MQTT_USERNAME = "mqtt_username";
        /// <summary>
        /// "mqtt_password"
        /// </summary>
        public const string MQTT_PASSWORD = "mqtt_password";
        /// <summary>
        ///  "mqtt_port"
        /// </summary>
        public const string MQTT_PORT = "mqtt_port";
        /// <summary>
        /// "mqtt_device_name"
        /// </summary>
        public const string MQTT_DEVICE_NAME = "mqtt_device_name";
        /// <summary>
        /// "mqtt_device_description"
        /// </summary>
        public const string MQTT_DEVICE_DESCRIPTION = "mqtt_device_description";
        /// <summary>
        /// "viz_color"
        /// </summary>
        public const string VIZ_COLOR = "viz_color";
    }


    /// <summary>
    /// Hashtable keys mains from SD JSON
    /// </summary>
    public static class HashtableKeys
    {
        /// <summary>
        /// "name"
        /// </summary>
        public const string NAME = "name";
        /// <summary>
        /// "value"
        /// </summary>
        public const string VALUE = "value";
    }


}

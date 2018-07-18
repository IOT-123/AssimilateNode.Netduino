

namespace SharedClasses
{

    public static class MethodNames
    {
        public const string GET_METADATA = "getMetadata";
        public const string PRINT_METADATA = "printMetadata";
        public const string SCAN_BUS_ADDRESSES = "scanBusAddresses";
    }

    public static class LogMessages
    {
        public const string OUT_OF_SYNC = "OUT OF SYNC";
        public const string RESTARTING = "RESTARTINGC";
        public const string TAB_1 = "\t";
        public const string TAB_2 = "\t\t";
        public const string CONFIRM_METADATA = "------------------------CONFIRMING METADATA FOR ";
        public const string DEVICE_COUNT = "DEVICE COUNT: ";
        public const string LOADED_WHITELIST = "Addresses loaded from whitelist";
    }

    public static class I2cMessages
    {
        public static string SEG3_DISCONTINUE => "0";
        public static byte WRITE_CONFIRM_METADATA => 1;
        public const string ASSIM_NAME = "ASSIM_NAME";
        public const string ASSIM_VERSION = "ASSIM_VERSION";
        public const string ASSIM_ROLE = "ASSIM_ROLE";
        public const string POWER_DOWN = "POWER_DOWN";
        public const string PREPARE_MS = "PREPARE_MS";
        public const string RESPONSE_MS = "RESPONSE_MS";
        public const string VCC_MV = "VCC_MV";
        public const string CLOCK_STRETCH = "CLOCK_STRETCH";
        public const string VIZ_PREFIX = "VIZ_";
        public const string VIZ_CARD_TYPE = "VIZ_CARD_TYPE";
        public const string VIZ_ICONS = "VIZ_ICONS";
        public const string VIZ_LABELS = "VIZ_LABELS";
        public const string VIZ_MIN = "VIZ_MIN";
        public const string VIZ_MAX = "VIZ_MAX";
        public const string VIZ_UNITS = "VIZ_UNITS";
        public const string VIZ_TOTAL = "VIZ_TOTAL";
        public const string VIZ_VALUES = "VIZ_VALUES";
        public const string VIZ_IS_SERIES = "VIZ_IS_SERIES";
        public const string VIZ_M_SERIES = "VIZ_M_SERIES";// not used yet
        public const string VIZ_HIGH = "VIZ_HIGH";
        public const string VIZ_LOW = "VIZ_LOW";
        public const string VIZ_TOTL_UNIT = "VIZ_TOTL_UNIT";
        public const string CARDTYPE_TOGGLE = "toggle";
        public const string CARDTYPE_INPUT = "input";
        public const string CARDTYPE_SLIDER = "slider";
        public const string CARDTYPE_BUTTON = "button";
        public const string CARDTYPE_TEXT = "text";
        public const string CARDTYPE_CHART_DONUT = "chart-donut";
        public const string CARDTYPE_CHART_LINE = "chart-line";
        public const string SENSOR = "SENSOR";
        public const string ACTOR = "ACTOR";
    }


    public static class JsonKeys
    {
        public const string CARD_TYPE = "card_type";
        public const string ICONS = "icons";
        public const string UNITS = "units";
        public const string TOTAL = "total";
        public const string VALUES = "values";
        public const string VALUE = "value";
        public const string LABELS = "labels";
        public const string SERIES = "series";
        public const string MIN = "min";
        public const string MAX = "max";
        public const string LOW = "low";
        public const string HIGH = "high";
        public const string IS_SERIES = "is_series";
        
    }

    public static class JsonValues
    {
        public const string TEXT_STATUS = "STATUS";
        public const int CHART_DONUT_TOTAL = 100;
        public const string CHART_DONUT_UNITS = "%";
        public const string CHART_DONUT_VALUE_LABELS = "[]";
        public const string CHART_DONUT_VALUE_SERIES = "0";
        public const int CHART_LINE_MAX = 12;
        public const int CHART_LINE_LOW = 0;
        public const int CHART_LINE_HIGH = 100;
        public const string IS_SEREIES_VALUE = "true";
    }

    public static class HashtableKeys
    {
        public const string NAME = "name";
        public const string VALUE = "value";
    }


}

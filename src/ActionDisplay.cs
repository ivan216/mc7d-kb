namespace _3dedit
{
    /// <summary>
    /// All keybind display text — edit here to change what appears on the
    /// keyboard reference. Two variants per action:
    ///   <c>XxxDisplay</c> – drawn inside each key (use \n to split into 2 lines;
    ///                         single-line text is centred vertically).
    ///   <c>XxxTooltip</c> – single-line text shown on mouse hover.
    /// </summary>
    public static class ActionDisplay
    {
        /// <summary>args: {0}=fromAxis.name, {1}=toAxis.name</summary>
        public static string TwistDisplay  = "{0}→{1}";
        public static string TwistTooltip  = "Twist from {0} to {1}";

        /// <summary>args: {0}=axis.name, {1}=layerMask</summary>
        public static string GripDisplay   = "{0}{{{1}}}";
        public static string GripTooltip   = "Grip axis {0} layer {1}";

        public static string RecenterDisplay = "CTR";
        public static string RecenterTooltip = "Recenter";

        /// <summary>args: {0}=grip.axis.name, {1}=layerMask, {2}=fromAxis.name, {3}=toAxis.name</summary>
        public static string GripTwistDisplay = "{0}{{{1}}}\n{2}→{3}";
        public static string GripTwistTooltip = "Grip axis {0} layer {1} and twist from {2} to {3}";

        /// <summary>args: {0}=layerMask</summary>
        public static string LayerDisplay   = "{{{0}}}";
        public static string LayerTooltip   = "Layer(bitmask) {0}";

        /// <summary>args: {0}="-" or "+", {1}=axis.name</summary>
        public static string Twist2cDisplay = "{0}{1}";
        public static string Twist2cTooltip = "Twist2c axis {0}{1}";

        /// <summary>args: {0}="-" or "+", {1}=axis.name</summary>
        public static string Twist3cDisplay = "{0}{1}";
        public static string Twist3cTooltip = "Twist3c axis {0}{1}";

        /// <summary>args: {0}=layout name</summary>
        public static string ChangeLayoutDisplay = "[{0}]";
        public static string ChangeLayoutTooltip = "change layout to {0}";

        /// <summary>args: {0}=macro id</summary>
        public static string MacroDisplay   = "M #{0}";
        public static string MacroTooltip   = "apply Macro id #{0}";

        public static string MacroRevDisplay = "Mrev";
        public static string MacroRevTooltip = "apply Macro Reverse";
    }
}

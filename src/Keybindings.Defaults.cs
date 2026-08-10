using System;
using System.Collections.Generic;

namespace _3dedit
{

    public partial class Keybindings
    {
        public static readonly Dictionary<string, Func<IAction>> ActionFactories = new Dictionary<string, Func<IAction>>
        {
            { "Grip", () => new Grip() },
            { "Twist", () => new Twist() },
            { "GripTwist", () => new GripTwist() },
            { "Twist2c", () => new Twist2c() },
            { "Twist3c", () => new Twist3c() },
            { "Layer", () => new Layer() },
            { "Recenter", () => new Recenter() },
            { "ChangeLayout", () => new ChangeLayout() },
            { "Macro", () => new Macro() },
            { "MacroReverse", () => new MacroReverse() },
        };

        public static Dictionary<string, KeybindSet> defaultBinds = new Dictionary<string, KeybindSet> {
            { "5D_2key",  new KeybindSet(new Dictionary<string, IAction> {
                { "D", new Grip(Axis.W, 1) },
                { "V", new Grip(Axis.W, -1) },
                { "F", new Grip(Axis.X, 1) },
                { "W", new Grip(Axis.X, -1) },
                { "E", new Grip(Axis.Y, 1) },
                { "C", new Grip(Axis.Y, -1) },
                { "R", new Grip(Axis.Z, 1) },
                { "S", new Grip(Axis.Z, -1) },
                { "G", new Grip(Axis.V, 1) },
                { "A", new Grip(Axis.V, -1) },
                { "X", new Grip(Axis.W, 0b11111) },

                { "J", new Twist(Axis.Z, Axis.X) },
                { "L", new Twist(Axis.X, Axis.Z) },

                { "K", new Twist(Axis.Z, Axis.Y) },
                { "I", new Twist(Axis.Y, Axis.Z) },

                { "U", new Twist(Axis.X, Axis.Y) },
                { "O", new Twist(Axis.Y, Axis.X) },

                { "Y", new Twist(Axis.V, Axis.Y) },
                { "H", new Twist(Axis.Y, Axis.V) },

                { "M", new Twist(Axis.X, Axis.V) },
                { "Oemcomma", new Twist(Axis.V, Axis.X) },

                { "N", new Twist(Axis.Z, Axis.V) },
                { "OemPeriod", new Twist(Axis.V, Axis.Z) },

                { "Space", new Recenter() },

                { "D1", new Layer(1) },
                { "D2", new Layer(2) },
                { "D3", new Layer(4) },
                { "ShiftKey", new Layer(1|2) },
            })},
            { "Default_3key", new KeybindSet(new Dictionary<string, IAction>{
                { "D", new Grip(Axis.W, 1) },
                { "V", new Grip(Axis.W, -1) },
                { "F", new Grip(Axis.X, 1) },
                { "W", new Grip(Axis.X, -1) },
                { "E", new Grip(Axis.Y, 1) },
                { "C", new Grip(Axis.Y, -1) },
                { "R", new Grip(Axis.Z, 1) },
                { "S", new Grip(Axis.Z, -1) },
                { "G", new Grip(Axis.V, 1) },
                { "A", new Grip(Axis.V, -1) },
                { "T", new Grip(Axis.U, 1) },
                { "Q", new Grip(Axis.U, -1) },
                { "B", new Grip(Axis.T, 1) },
                { "Z", new Grip(Axis.T, -1) },
                { "X", new Grip(Axis.W, 0b11111) },

                { "L", new Twist2c(Axis.X, false) },
                { "K", new Twist2c(Axis.Y, false) },
                { "J", new Twist2c(Axis.Z, true) },
                { "H", new Twist2c(Axis.W, false) },
                { "O", new Twist2c(Axis.V, false) },
                { "I", new Twist2c(Axis.U, false) },
                { "U", new Twist2c(Axis.T, false) },

                { "Space", new Recenter() },

                { "D1", new Layer(1) },
                { "D2", new Layer(2) },
                { "D3", new Layer(4) },
            }) }
        };
    }
}

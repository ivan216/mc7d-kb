using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace _3dedit
{

    public partial class Keybindings
    {
        public class Axis
        {
            public static readonly Axis X = new Axis("X", 2, true), Y = new Axis("Y", 4), Z = new Axis("Z", 3), W = new Axis("W", 1), V = new Axis("V", 5, true), U = new Axis("U", 6), T = new Axis("T", 7);
            public static readonly Dictionary<string, Axis> fromString = new Dictionary<string, Axis>()
            {
                { "X", X },
                { "Y", Y },
                { "Z", Z },
                { "W", W },
                { "V", V },
                { "U", U },
                { "T", T }
            };

            public string name;
            public int idx;
            public bool inverted;

            public Axis(string name, int idx)
            {
                this.name = name;
                this.idx = idx;
                this.inverted = false;
            }
            public Axis(string name, int idx, bool inverted)
            {
                this.name = name;
                this.idx = idx;
                this.inverted = inverted;
            }
        }

        public class KeybindSet
        {
            public Dictionary<string, IAction> binds = new Dictionary<string, IAction>();

            public KeybindSet() { }
            public KeybindSet(Dictionary<string, IAction> binds) {
                this.binds = binds;
            }

            public string Serialize()
            {
                List<string> res = new List<string>();

                foreach (var item in binds)
                {
                    string k = item.Key;
                    res.Add($"{k},{item.Value.Serialize()}");
                }

                return String.Join(" ", res.ToArray());
            }

            public void Deserialize(string data, string keybindSetName)
            {
                string[] p = data.Split(' ');

                foreach (var item in p)
                {
                    try
                    {
                        string[] p2 = item.Split(',');
                        if (p2.Length < 2) continue;

                        IAction action = null;

                        if (ActionFactories.TryGetValue(p2[1], out var factory))
                            action = factory();

                        if (action != null)
                        {
                            string k = p2[0];

                            // Skip bindings with invalid chord keys
                            if (string.IsNullOrEmpty(k) || !ChordUtils.IsValid(k, out _))
                                continue;

                            action.Deserialize(item);
                            binds.Add(k, action);
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show($"Error occured while loading action for keybind set {keybindSetName ?? ""}: {item}\r\n{e.Message}");
                    }
                }
            }
        }

        public interface IAction {
            void OnKeyDown(ref Cube7D Cube, ref bool redraw, ref bool didTwist);
            void OnKeyUp(ref Cube7D Cube, ref bool redraw, ref bool didTwist);

            string Serialize();
            void Deserialize(string s);

            Control[] SetupControls();
            /// <summary>Short human-readable description for the keybinds reference display.</summary>
            string GetDescription();
            /// <summary>Single-line tooltip shown on hover (defaults to GetDescription if empty).</summary>
            string GetTooltip();
        }
    }
}

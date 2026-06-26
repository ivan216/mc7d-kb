using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
using static _3dedit.Keybindings;


namespace _3dedit
{

    public class Keybindings
    {
        public static Keybindings loaded;
        public static Action<int, bool> ExecuteMacroById;
        public static bool MacroReverseHeld;

        public event EventHandler KeybindLayoutsChanged;
        public event EventHandler ActiveLayoutChanged;

        public Dictionary<string, KeybindSet> keybinds = new Dictionary<string, KeybindSet>();
        public KeybindSet activeKeybinds;
        public string activeKeybindsName;

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

        public Keybindings()
        {
            foreach (var item in defaultBinds)
            {
                keybinds.Add(item.Key, item.Value);
            }
            switchKeybindSet("5D_2key");
        }

        public IAction GetAction(string key)
        {
            bool res = activeKeybinds.binds.TryGetValue(key, out IAction action);
            return action;
        }

        public string Serialize()
        {
            List<string> res = new List<string>();

            foreach (var item in keybinds)
            {
                res.Add($"{item.Key} : {item.Value.Serialize()}");
            }

            return string.Join("\r\n", res.ToArray());
        }

        public void LoadKeybindSet(string s, int idx)
        {
            try
            {
                string[] p = s.Split(new string[] { " : " }, StringSplitOptions.None);
                KeybindSet kbs = new KeybindSet();
                kbs.Deserialize(p[1], p[0]);

                if (keybinds.ContainsKey(p[0]))
                {
                    keybinds.Remove(p[0]);
                }
                keybinds.Add(p[0], kbs);

                if (keybinds.Count == 1 || activeKeybindsName == p[0])
                {
                    switchKeybindSet(p[0]);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error occured while loading keybind set on line {idx} of the keybinds file.\r\n{e.Message}");
            }
        }
        public bool switchKeybindSet(string name)
        {
            if (keybinds.ContainsKey(name))
            {
                activeKeybinds = keybinds[name];
                activeKeybindsName = name;

                OnActiveLayoutChanged(EventArgs.Empty);
                return true;
            }
            return false;
        }

        public bool CreateKeybindSet(string name)
        {
            if (name.Length < 0)
            {
                MessageBox.Show($"Layout name cannot be blank");
                return false;
            }

            if (keybinds.ContainsKey(name))
            {
                MessageBox.Show($"Layout already exists with that name ({name})");
                return false;
            }

            keybinds.Add(name, new KeybindSet());
            OnKeybindLayoutsChanged(EventArgs.Empty);
            return true;
        }

        public bool DeleteKeybindSet(string name)
        {
            if (keybinds.ContainsKey(name))
            {
                keybinds.Remove(name);
                OnKeybindLayoutsChanged(EventArgs.Empty);
            }

            return true;
        }

        protected void OnKeybindLayoutsChanged(EventArgs e)
        {
            KeybindLayoutsChanged?.Invoke(this, e);
        }

        protected void OnActiveLayoutChanged(EventArgs e)
        {
            ActiveLayoutChanged?.Invoke(this, e);
        }

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

                        switch (p2[1])
                        {
                            case "Grip":
                                action = new Grip();
                                break;
                            case "Twist":
                                action = new Twist();
                                break;
                            case "Recenter":
                                action = new Recenter();
                                break;
                            case "GripTwist":
                                action = new GripTwist();
                                break;
                            case "Twist2c":
                                action = new Twist2c();
                                break;
                            case "Twist3c":
                                action = new Twist3c();
                                break;
                            case "Layer":
                                action = new Layer();
                                break;
                            case "ChangeLayout":
                                action = new ChangeLayout();
                                break;
                            case "Macro":
                                action = new Macro();
                                break;
                            case "MacroReverse":
                                action = new MacroReverse();
                                break;
                        }

                        if (action != null)
                        {
                            string k = p2[0];
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
        }


        public class Twist : IAction
        {
            public Axis fromAxis;
            public Axis toAxis;

            public Twist()
            {
                this.fromAxis = null;
                this.toAxis = null;
            }
            public Twist(Axis fromAxis, Axis toAxis) {
                this.fromAxis = fromAxis;
                this.toAxis = toAxis;
            }

            public void OnKeyDown(ref Cube7D Cube, ref bool redraw, ref bool didTwist)
            {
                if (Cube.Gripped[0] == -1)
                {
                    return;
                }

                if (Cube.D < fromAxis.idx || Cube.D < toAxis.idx)
                {
                    return;
                }

                Axis from = fromAxis;
                Axis to = toAxis;
                bool flip = false;
                int gripAxisIdx = Cube.Gripped[0];

                if (gripAxisIdx == from.idx)
                {
                    from = gripAxisIdx == Axis.W.idx ? Axis.V : Axis.W;
                    flip = true;
                }
                if (gripAxisIdx == to.idx)
                {
                    to = gripAxisIdx == Axis.W.idx ? Axis.V : Axis.W;
                    flip = true;
                }

                bool grippingPos = ((Cube.Gripped[1] & 1) == 1) ^ (gripAxisIdx == Axis.X.idx || gripAxisIdx == Axis.V.idx);
                flip = flip && grippingPos && Cube.Gripped[0] < 5;

                if (flip ^ from.inverted ^ to.inverted)
                {
                    Axis tmp = from;
                    from = to;
                    to = tmp;
                }

                Cube.TwistGrip(from.idx, to.idx);

                redraw = true;
                didTwist = true;
            }

            public void OnKeyUp(ref Cube7D Cube, ref bool redraw, ref bool didTwist) { }

            public string Serialize()
            {
                return $"Twist,{fromAxis.name},{toAxis.name}";
            }

            public void Deserialize(string s)
            {
                string[] p = s.Split(',');
                if (p[1] != "Twist" || !Axis.fromString.ContainsKey(p[2]) || !Axis.fromString.ContainsKey(p[3]))
                {
                    throw new Exception($"Invalid Twist: {s}");
                }

                fromAxis = Axis.fromString[p[2]];
                toAxis = Axis.fromString[p[3]];
            }

            public Control[] SetupControls()
            {
                var axes = Axis.fromString.Keys.ToArray();

                // Use default values if axes are null (for UI setup)
                Axis displayFromAxis = this.fromAxis ?? Axis.X;
                Axis displayToAxis = this.toAxis ?? Axis.Y;

                ComboBox fromComboBox = new ComboBox
                {
                    Anchor = AnchorStyles.Left | AnchorStyles.Top,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    ItemHeight = 24,
                    Name = "Twist fromAxis",
                    Size = new Size(56, 30),
                };
                fromComboBox.Items.AddRange(axes);
                fromComboBox.SelectedIndex = fromComboBox.Items.IndexOf(displayFromAxis.name);
                fromComboBox.SelectedIndexChanged += (object sender, EventArgs e) =>
                {
                    this.fromAxis = Axis.fromString[(string)((ComboBox)sender).SelectedItem];
                };

                ComboBox toComboBox = new ComboBox
                {
                    Anchor = AnchorStyles.Left | AnchorStyles.Top,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    ItemHeight = 24,
                    Name = "Twist toAxis",
                    Size = new Size(56, 30),
                };
                toComboBox.Items.AddRange(axes);
                toComboBox.SelectedIndex = toComboBox.Items.IndexOf(displayToAxis.name);
                toComboBox.SelectedIndexChanged += (object sender, EventArgs e) =>
                {
                    this.toAxis = Axis.fromString[(string)((ComboBox)sender).SelectedItem];
                };
                
                fromComboBox.MouseWheel += (object sender, MouseEventArgs e) => ((HandledMouseEventArgs)e).Handled = true;
                toComboBox.MouseWheel += (object sender, MouseEventArgs e) => ((HandledMouseEventArgs)e).Handled = true;
                return new Control[] { fromComboBox, toComboBox };
            }
        }

        public class Grip : IAction
        {
            public Axis axis;
            public int layerMask;

            public Grip()
            {
                this.axis = Axis.X;
                this.layerMask = 1;
            }
            public Grip(Axis axis, int layerMask){
                this.axis = axis;
                this.layerMask = layerMask;
            }

            private int NormLayerMask()
            {
                return this.axis.inverted ? -this.layerMask : this.layerMask;
            }

            public void OnKeyDown(ref Cube7D Cube, ref bool redraw, ref bool didTwist)
            {
                Cube.Grip(axis.idx, this.NormLayerMask());
                redraw = true;
            }
            public void OnKeyUp(ref Cube7D Cube, ref bool redraw, ref bool didTwist)
            {
                int m0 = Cube.NormGripMask(this.NormLayerMask());
                if (Cube.Gripped[0] == axis.idx && Cube.Gripped[1] == m0)
                {
                    Cube.Grip(-1, 1);
                    redraw = true;
                }
            }

            public string Serialize()
            {
                return $"Grip,{axis.name},{layerMask}";
            }

            public void Deserialize(string s)
            {

                string[] p = s.Split(',');
                if (p[1] != "Grip" || !Axis.fromString.ContainsKey(p[2]))
                {
                    throw new Exception($"Invalid Grip: {s}");
                }

                int mask;
                Int32.TryParse(p[3], out mask);
                layerMask = mask;

                axis = Axis.fromString[p[2]];
            }

            public Control[] SetupControls()
            {
                ComboBox axisComboBox = new ComboBox
                {
                    Anchor = AnchorStyles.Left | AnchorStyles.Top,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    ItemHeight = 24,
                    Name = "Grip toAxis",
                    Size = new Size(56, 30),
                };
                axisComboBox.Items.AddRange(Axis.fromString.Keys.ToArray());
                axisComboBox.SelectedIndex = axisComboBox.Items.IndexOf(this.axis.name);
                axisComboBox.SelectedIndexChanged += (object sender, EventArgs e) =>
                {
                    this.axis = Axis.fromString[(string)((ComboBox)sender).SelectedItem];
                };

                axisComboBox.MouseWheel += (object sender, MouseEventArgs e) => ((HandledMouseEventArgs)e).Handled = true;


                NumericUpDown layerInput = new NumericUpDown
                {
                    Anchor = AnchorStyles.Top | AnchorStyles.Left,
                    Width = 56,
                    Minimum = -127,
                    Maximum = 127,
                    Value = layerMask,
                };
                layerInput.ValueChanged += (object sender, EventArgs e) => this.layerMask = (int)((NumericUpDown)sender).Value;
                layerInput.MouseWheel += (object sender, MouseEventArgs e) => ((HandledMouseEventArgs)e).Handled = true;

                return new Control[] { axisComboBox, layerInput };
            }
        }

        public class Recenter : IAction
        {
            public void OnKeyDown(ref Cube7D Cube, ref bool redraw, ref bool didTwist)
            {
                if (Cube.Gripped[0] != -1)
                {
                    Cube.RotateCubeByGrip();
                    redraw = true;
                }
            }
            public void OnKeyUp(ref Cube7D Cube, ref bool redraw, ref bool didTwist) { }

            public string Serialize() { return "Recenter"; }
            public void Deserialize(string s) { }
            public Control[] SetupControls()
            {
                return new Control[] { };
            }
        }

        public class GripTwist : IAction
        {
            private Grip grip;
            private Twist twist;

            public GripTwist()
            {
                this.grip = new Grip();
                this.twist = new Twist();
            }
            public GripTwist(Axis gripAxis, int layerMask, Axis fromAxis, Axis toAxis)
            {
                this.grip = new Grip(gripAxis, layerMask);
                this.twist = new Twist(fromAxis, toAxis);
            }

            public void OnKeyDown(ref Cube7D Cube, ref bool redraw, ref bool didTwist) { }
            public void OnKeyUp(ref Cube7D Cube, ref bool redraw, ref bool didTwist)
            {
                this.grip.OnKeyDown(ref Cube, ref redraw, ref didTwist);
                this.twist.OnKeyDown(ref Cube, ref redraw, ref didTwist);
                this.twist.OnKeyUp(ref Cube, ref redraw, ref didTwist);
                this.grip.OnKeyUp(ref Cube, ref redraw, ref didTwist);
            }

            public string Serialize()
            {
                return $"GripTwist,{this.grip.axis.name},{this.grip.layerMask},{this.twist.fromAxis.name},{this.twist.toAxis.name}";
            }
            public void Deserialize(string s)
            {
                string[] p = s.Split(',');
                if (p[1] != "GripTwist" || !Axis.fromString.ContainsKey(p[2]) || !Axis.fromString.ContainsKey(p[4]) || !Axis.fromString.ContainsKey(p[5]))
                {
                    throw new Exception($"Invalid GripTwist: {s}");
                }

                this.twist.fromAxis = Axis.fromString[p[4]];
                this.twist.toAxis = Axis.fromString[p[5]];

                Int32.TryParse(p[3], out int mask);
                this.grip.layerMask = mask;
                this.grip.axis = Axis.fromString[p[2]];
            }
            public Control[] SetupControls()
            {
                var twistControls = this.twist.SetupControls();
                Splitter split = new Splitter()
                {
                    BorderStyle = BorderStyle.FixedSingle,
                    Cursor = Cursors.Default,
                };
                var gripControls = this.grip.SetupControls();
                return gripControls.Concat(new Control[] { split }).Concat(twistControls).ToArray();
            }
        }

        public class Twist2c : IAction
        {
            public Axis axis;
            public bool negative;


            public Twist2c()
            {
                this.axis = Axis.X;
                this.negative = false;
            }
            public Twist2c(Axis axis, bool negative)
            {
                this.axis = axis;
                this.negative = negative;
            }

            public void OnKeyDown(ref Cube7D Cube, ref bool redraw, ref bool didTwist)
            {
                // Twist2c requires grip before setting axes
                if (Cube.Gripped[0] == -1)
                {
                    return;
                }

                Twist t = Cube.partialTwist;

                if (t.fromAxis == null)
                {
                    t.fromAxis = this.axis;
                }
                else
                {
                    t.toAxis = this.axis;
                }

                if (this.negative)
                {
                    Axis tmp = t.toAxis;
                    t.toAxis = t.fromAxis;
                    t.fromAxis = tmp;
                }

                if (t.toAxis != null && t.fromAxis != null)
                {
                    t.OnKeyDown(ref Cube, ref redraw, ref didTwist);
                    t.OnKeyUp(ref Cube, ref redraw, ref didTwist);
                    t.toAxis = null;
                    t.fromAxis = null;

                    redraw = true;
                    didTwist = true;
                }
            }
           public void OnKeyUp(ref Cube7D Cube, ref bool redraw, ref bool didTwist) { }

            public string Serialize()
            {
                return $"Twist2c,{(this.negative ? "-" : "+")},{axis.name}";
            }
            public void Deserialize(string s)
            {
                string[] p = s.Split(',');
                if (p[1] != "Twist2c" || !Axis.fromString.ContainsKey(p[3]))
                {
                    throw new Exception($"Invalid Twist2c: {s}");
                }

                this.axis = Axis.fromString[p[3]];
                this.negative = p[2] == "-";
            }
            public Control[] SetupControls()
            {

                ComboBox axisComboBox = new ComboBox
                {
                    Anchor = AnchorStyles.Left | AnchorStyles.Top,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    ItemHeight = 24,
                    Name = "Twist2c toAxis",
                    Size = new Size(56, 30),
                };
                axisComboBox.Items.AddRange(Axis.fromString.Keys.ToArray());
                axisComboBox.SelectedIndex = axisComboBox.Items.IndexOf(this.axis.name);
                axisComboBox.SelectedIndexChanged += (object sender, EventArgs e) =>
                {
                    this.axis = Axis.fromString[(string)((ComboBox)sender).SelectedItem];
                };

                axisComboBox.MouseWheel += (object sender, MouseEventArgs e) => ((HandledMouseEventArgs)e).Handled = true;

                ComboBox negComboBox = new ComboBox
                {
                    Anchor = AnchorStyles.Left | AnchorStyles.Top,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    ItemHeight = 24,
                    Name = "negative",
                    Size = new Size(56, 30),
                };
                negComboBox.Items.AddRange(new string[] { "+", "-" });
                negComboBox.SelectedIndex = this.negative ? 1 : 0;
                negComboBox.SelectedIndexChanged += (object sender, EventArgs e) =>
                {
                    this.negative = (string)((ComboBox)sender).SelectedItem == "-";
                };

                negComboBox.MouseWheel += (object sender, MouseEventArgs e) => ((HandledMouseEventArgs)e).Handled = true;

                return new Control[] { negComboBox, axisComboBox };
            }
        }

        public class Layer : IAction
        {
            public int layerMask;

            public Layer()
            {
                this.layerMask = 1;
            }
            public Layer(int layerMask)
            {
                this.layerMask = layerMask;
            }

            public void OnKeyDown(ref Cube7D Cube, ref bool redraw, ref bool didTwist)
            {
                Cube.LayerOverrides.Add(this);
                redraw = true;
            }
            public void OnKeyUp(ref Cube7D Cube, ref bool redraw, ref bool didTwist)
            {
                Cube.LayerOverrides.Remove(this);
                redraw = true;
            }

            public string Serialize()
            {
                return $"Layer,{layerMask}";
            }
            public void Deserialize(string s)
            {
                string[] p = s.Split(',');
                if (p[1] != "Layer")
                {
                    throw new Exception($"Invalid Layer: {s}");
                }
                
                Int32.TryParse(p[2], out int mask);
                this.layerMask = mask;
            }
            public Control[] SetupControls()
            {
                NumericUpDown layerInput = new NumericUpDown
                {
                    Width = 56,
                    Minimum = -127,
                    Maximum = 127,
                    Value = layerMask,
                };
                layerInput.ValueChanged += (object sender, EventArgs e) => this.layerMask = (int)((NumericUpDown)sender).Value;
                layerInput.MouseWheel += (object sender, MouseEventArgs e) => ((HandledMouseEventArgs)e).Handled = true;

                return new Control[] { layerInput };
            }
        }

        public class Twist3c : IAction
        {
            public Axis axis;
            public bool negative;

            public Twist3c()
            {
                this.axis = Axis.X;
                this.negative = false;
            }
            public Twist3c(Axis axis, bool negative)
            {
                this.axis = axis;
                this.negative = negative;
            }

            public void OnKeyDown(ref Cube7D Cube, ref bool redraw, ref bool didTwist)
            {
                var t3c = Cube.partialTwist3c;

                // Step 1: Set grip axis
                if (t3c.step == 0)
                {
                    // Validate dimension
                    if (Cube.D < this.axis.idx)
                    {
                        // Invalid axis for current dimension, ignore
                        return;
                    }

                    t3c.gripAxis = this.axis;

                    // Get layer mask from LayerOverrides (ignore current Gripped state)
                    int baseMask = Cube.GetLayerOverridesMask();

                    // The negative flag in Twist3c controls direction from user perspective:
                    // negative=false means positive direction, negative=true means negative direction
                    //
                    // For the Grip class:
                    // - Non-inverted axes (W,Y,Z,U,T): layerMask sign directly controls direction
                    // - Inverted axes (X,V): layerMask sign is reversed (1 means negative, -1 means positive in Cube.Grip)
                    //
                    // So for Twist3c, we set layerMask to match the user's expectation:
                    // - negative=false: use positive layerMask (1, 2, 4, etc.)
                    // - negative=true: use negative layerMask (-1, -2, -4, etc.)
                    // The Grip.NormLayerMask() will handle axis inversion automatically

                    t3c.gripLayerMask = this.negative ? -baseMask : baseMask;

                    t3c.step = 1;
                    redraw = true;
                }
                // Step 2: Set fromAxis
                else if (t3c.step == 1)
                {
                    // Validate dimension
                    if (Cube.D < this.axis.idx)
                    {
                        // Invalid axis, reset
                        t3c.Reset();
                        redraw = true;
                        return;
                    }

                    t3c.fromAxis = this.axis;
                    // Accumulate negative count
                    if (this.negative)
                    {
                        t3c.negativeCount++;
                    }
                    t3c.step = 2;
                    redraw = true;
                }
                // Step 3: Set toAxis and execute
                else if (t3c.step == 2)
                {
                    // Validate dimension
                    if (Cube.D < this.axis.idx)
                    {
                        // Invalid axis, reset
                        t3c.Reset();
                        redraw = true;
                        return;
                    }

                    t3c.toAxis = this.axis;

                    // Accumulate negative count
                    if (this.negative)
                    {
                        t3c.negativeCount++;
                    }

                    // If negativeCount is odd, swap fromAxis and toAxis
                    if (t3c.negativeCount % 2 == 1)
                    {
                        Axis tmp = t3c.toAxis;
                        t3c.toAxis = t3c.fromAxis;
                        t3c.fromAxis = tmp;
                    }

                    t3c.step = 3;

                    // Now execute the grip+twist
                    if (t3c.IsValid())
                    {
                        // First grip
                        Grip grip = new Grip(t3c.gripAxis, t3c.gripLayerMask);
                        grip.OnKeyDown(ref Cube, ref redraw, ref didTwist);

                        // Then twist
                        Twist twist = new Twist(t3c.fromAxis, t3c.toAxis);
                        twist.OnKeyDown(ref Cube, ref redraw, ref didTwist);

                        // Release grip
                        grip.OnKeyUp(ref Cube, ref redraw, ref didTwist);

                        // Reset for next operation
                        t3c.Reset();
                    }
                    else
                    {
                        // Invalid twist, reset
                        t3c.Reset();
                    }

                    redraw = true;
                }
            }

            public void OnKeyUp(ref Cube7D Cube, ref bool redraw, ref bool didTwist) { }

            public string Serialize()
            {
                return $"Twist3c,{(this.negative ? "-" : "+")},{axis.name}";
            }

            public void Deserialize(string s)
            {
                string[] p = s.Split(',');
                if (p[1] != "Twist3c" || !Axis.fromString.ContainsKey(p[3]))
                {
                    throw new Exception($"Invalid Twist3c: {s}");
                }

                this.axis = Axis.fromString[p[3]];
                this.negative = p[2] == "-";
            }

            public Control[] SetupControls()
            {
                ComboBox negComboBox = new ComboBox
                {
                    Anchor = AnchorStyles.Left | AnchorStyles.Top,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    ItemHeight = 24,
                    Name = "negative",
                    Size = new Size(56, 30),
                };
                negComboBox.Items.AddRange(new string[] { "+", "-" });
                negComboBox.SelectedIndex = this.negative ? 1 : 0;
                negComboBox.SelectedIndexChanged += (object sender, EventArgs e) =>
                {
                    this.negative = (string)((ComboBox)sender).SelectedItem == "-";
                };

                negComboBox.MouseWheel += (object sender, MouseEventArgs e) => ((HandledMouseEventArgs)e).Handled = true;

                ComboBox axisComboBox = new ComboBox
                {
                    Anchor = AnchorStyles.Left | AnchorStyles.Top,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    ItemHeight = 24,
                    Name = "Twist3c axis",
                    Size = new Size(56, 30),
                };
                axisComboBox.Items.AddRange(Axis.fromString.Keys.ToArray());
                axisComboBox.SelectedIndex = axisComboBox.Items.IndexOf(this.axis.name);
                axisComboBox.SelectedIndexChanged += (object sender, EventArgs e) =>
                {
                    this.axis = Axis.fromString[(string)((ComboBox)sender).SelectedItem];
                };

                axisComboBox.MouseWheel += (object sender, MouseEventArgs e) => ((HandledMouseEventArgs)e).Handled = true;

                return new Control[] { negComboBox, axisComboBox };
            }
        }

        public class ChangeLayout : IAction
        {
            public string layout;
            public ChangeLayout()
            {
                this.layout = "";
            }
            public ChangeLayout(string layout)
            {
                this.layout = layout;
            }

            public void OnKeyDown(ref Cube7D Cube, ref bool redraw, ref bool didTwist) { }
            public void OnKeyUp(ref Cube7D Cube, ref bool redraw, ref bool didTwist)
            {
                Keybindings.loaded.switchKeybindSet(this.layout);
            }

            public string Serialize()
            {
                return $"ChangeLayout,{layout}";
            }
            public void Deserialize(string s)
            {
                string[] p = s.Split(',');
                if (p[1] != "ChangeLayout")
                {
                    throw new Exception($"Invalid ChangeLayout: {s}");
                }

                this.layout = p[2];
            }
            public Control[] SetupControls()
            {
                ComboBox layoutComboBox = new ComboBox
                {
                    Anchor = AnchorStyles.Left | AnchorStyles.Top,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    ItemHeight = 24,
                    Name = "layout",
                    Size = new Size(96, 30),
                };
                layoutComboBox.Items.AddRange(Keybindings.loaded.keybinds.Keys.ToArray());
                layoutComboBox.SelectedIndex = layoutComboBox.Items.IndexOf(this.layout);
                layoutComboBox.SelectedIndexChanged += (object sender, EventArgs e) =>
                {
                    this.layout = (string)((ComboBox)sender).SelectedItem;
                };

                layoutComboBox.MouseWheel += (object sender, MouseEventArgs e) => ((HandledMouseEventArgs)e).Handled = true;

                return new Control[] { layoutComboBox };
            }
        }

        public class Macro : IAction
        {
            public int id;

            public Macro()
            {
                this.id = 1;
            }
            public Macro(int id)
            {
                this.id = id;
            }

            public void OnKeyDown(ref Cube7D Cube, ref bool redraw, ref bool didTwist)
            {
                if (ExecuteMacroById == null) return;
                ExecuteMacroById(id, MacroReverseHeld);
                redraw = true;
            }

            public void OnKeyUp(ref Cube7D Cube, ref bool redraw, ref bool didTwist) { }

            public string Serialize()
            {
                return $"Macro,{id}";
            }

            public void Deserialize(string s)
            {
                string[] p = s.Split(',');
                if (p[1] != "Macro" || p.Length < 3)
                {
                    throw new Exception($"Invalid Macro: {s}");
                }

                Int32.TryParse(p[2], out id);
                if (id < 1) id = 1;
            }

            public Control[] SetupControls()
            {
                NumericUpDown idInput = new NumericUpDown
                {
                    Width = 56,
                    Minimum = 1,
                    Maximum = 99999,
                    Value = id,
                };
                idInput.ValueChanged += (object sender, EventArgs e) => this.id = (int)((NumericUpDown)sender).Value;
                idInput.MouseWheel += (object sender, MouseEventArgs e) => ((HandledMouseEventArgs)e).Handled = true;

                return new Control[] { idInput };
            }
        }

        public class MacroReverse : IAction
        {
            public void OnKeyDown(ref Cube7D Cube, ref bool redraw, ref bool didTwist)
            {
                MacroReverseHeld = true;
            }

            public void OnKeyUp(ref Cube7D Cube, ref bool redraw, ref bool didTwist)
            {
                MacroReverseHeld = false;
            }

            public string Serialize() { return "MacroReverse"; }
            public void Deserialize(string s)
            {
                string[] p = s.Split(',');
                if (p[1] != "MacroReverse")
                {
                    throw new Exception($"Invalid MacroReverse: {s}");
                }
            }
            public Control[] SetupControls()
            {
                return new Control[] { };
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace _3dedit
{

    public partial class Keybindings
    {
        public static Keybindings loaded;
        public static Action<int, bool> ExecuteMacroById;
        public static bool MacroReverseHeld;

        public event EventHandler KeybindLayoutsChanged;
        public event EventHandler ActiveLayoutChanged;

        public Dictionary<string, KeybindSet> keybinds = new Dictionary<string, KeybindSet>();
        public KeybindSet activeKeybinds;
        public string activeKeybindsName;

        public Keybindings()
        {
            foreach (var item in defaultBinds)
            {
                keybinds.Add(item.Key, item.Value);
            }
            switchKeybindSet("5D_2key");
        }

        /// <summary>
        /// Resolve a key press to an action using Hyperspeedcube's
        /// consumed-modifier approach.
        ///
        /// Iterates all bindings and checks:
        ///   primary key matches AND
        ///   (binding_mods & ~consumedMods) == (pressed_mods & ~consumedMods)
        ///
        /// Among matches, selects the one with the most modifiers (most specific).
        /// </summary>
        public IAction GetActionWithFallback(string keyName, bool ctrl, bool shift, bool alt, Keys consumedMods)
        {
            // Build the effective "pressed modifier bitmask" after removing consumed mods
            Keys pressedMods = 0;
            if (ctrl) pressedMods |= Keys.Control;
            if (shift) pressedMods |= Keys.Shift;
            if (alt)   pressedMods |= Keys.Alt;
            Keys mask = ~consumedMods;
            Keys effectivePressed = pressedMods & mask;

            IAction bestAction = null;
            int bestModCount = -1;

            foreach (var kvp in activeKeybinds.binds)
            {
                var parsed = ChordUtils.Parse(kvp.Key);
                if (parsed == null || parsed.PrimaryKey != keyName)
                    continue;

                Keys bindingMods = 0;
                if (parsed.Ctrl)  bindingMods |= Keys.Control;
                if (parsed.Shift) bindingMods |= Keys.Shift;
                if (parsed.Alt)   bindingMods |= Keys.Alt;

                // Core Hyperspeedcube check: match after masking consumed mods
                if ((bindingMods & mask) != effectivePressed)
                    continue;

                // Pick the most specific match (most modifiers)
                int modCount = (parsed.Ctrl ? 1 : 0) + (parsed.Shift ? 1 : 0) + (parsed.Alt ? 1 : 0);
                if (modCount > bestModCount)
                {
                    bestModCount = modCount;
                    bestAction = kvp.Value;
                }
            }

            return bestAction;
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
    }
}

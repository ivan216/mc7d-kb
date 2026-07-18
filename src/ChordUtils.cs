using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace _3dedit
{
    /// <summary>
    /// Parsed components of a chord string.
    /// </summary>
    public class ChordParts
    {
        public bool Ctrl { get; set; }
        public bool Shift { get; set; }
        public bool Alt { get; set; }
        public string PrimaryKey { get; set; }
    }

    /// <summary>
    /// Utilities for chord string construction, parsing, validation.
    ///
    /// Chord format: zero or more modifiers ("Ctrl", "Shift", "Alt")
    /// joined by '+' with exactly one primary key at the end.
    /// Canonical modifier order: Ctrl > Shift > Alt.
    /// </summary>
    public static class ChordUtils
    {
        private static readonly HashSet<Keys> ModifierKeyCodes = new HashSet<Keys>
        {
            Keys.ShiftKey, Keys.LShiftKey, Keys.RShiftKey,
            Keys.ControlKey, Keys.LControlKey, Keys.RControlKey,
            Keys.Menu, Keys.LMenu, Keys.RMenu
        };

        private static readonly HashSet<string> ModifierLabels = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Ctrl", "Shift", "Alt"
        };

        // ---- Key classification ----

        public static bool IsModifierKey(Keys keyCode)
        {
            return ModifierKeyCodes.Contains(keyCode);
        }

        /// <summary>
        /// Returns the modifier flag (Keys.Control / Keys.Shift / Keys.Alt)
        /// for a modifier key code, or Keys.None if not a modifier.
        /// </summary>
        public static Keys GetModifierFlag(Keys keyCode)
        {
            if (keyCode == Keys.ShiftKey || keyCode == Keys.LShiftKey || keyCode == Keys.RShiftKey)
                return Keys.Shift;
            if (keyCode == Keys.ControlKey || keyCode == Keys.LControlKey || keyCode == Keys.RControlKey)
                return Keys.Control;
            if (keyCode == Keys.Menu || keyCode == Keys.LMenu || keyCode == Keys.RMenu)
                return Keys.Alt;
            return Keys.None;
        }

        // ---- Chord construction ----

        public static string BuildChord(bool ctrl, bool shift, bool alt, string primaryKey)
        {
            var parts = new List<string>(4);
            if (ctrl) parts.Add("Ctrl");
            if (shift) parts.Add("Shift");
            if (alt) parts.Add("Alt");
            parts.Add(primaryKey);
            return string.Join("+", parts.ToArray());
        }

        public static string BuildChord(bool ctrl, bool shift, bool alt, Keys primaryKey)
        {
            return BuildChord(ctrl, shift, alt, primaryKey.ToString());
        }

        // ---- Parsing ----

        /// <summary>
        /// Parse a chord string into components.
        /// Returns null on invalid input.
        /// </summary>
        public static ChordParts Parse(string chord)
        {
            if (string.IsNullOrEmpty(chord))
                return null;

            var parts = chord.Split('+');
            if (parts.Length == 0)
                return null;

            var result = new ChordParts
            {
                Ctrl = false, Shift = false, Alt = false,
                PrimaryKey = null
            };

            foreach (var part in parts)
            {
                if (string.IsNullOrEmpty(part))
                    return null;

                if (string.Equals(part, "Ctrl", StringComparison.OrdinalIgnoreCase))
                    result.Ctrl = true;
                else if (string.Equals(part, "Shift", StringComparison.OrdinalIgnoreCase))
                    result.Shift = true;
                else if (string.Equals(part, "Alt", StringComparison.OrdinalIgnoreCase))
                    result.Alt = true;
                else if (result.PrimaryKey == null)
                    result.PrimaryKey = part;
                else
                    return null; // multiple primary keys
            }

            return result;
        }

        // ---- Validation ----

        public static bool IsValid(string chord, out string error)
        {
            error = null;

            if (string.IsNullOrEmpty(chord))
            {
                error = "Chord cannot be empty";
                return false;
            }

            var parsed = Parse(chord);
            if (parsed == null)
            {
                error = "This chord format is not supported";
                return false;
            }

            if (parsed.PrimaryKey == null)
            {
                error = "A primary key is required";
                return false;
            }

            if (ModifierLabels.Contains(parsed.PrimaryKey))
            {
                error = "This chord format is not supported";
                return false;
            }

            return true;
        }

        // ---- Normalisation ----

        public static string Normalize(string chord)
        {
            var parsed = Parse(chord);
            return parsed != null && parsed.PrimaryKey != null
                ? BuildChord(parsed.Ctrl, parsed.Shift, parsed.Alt, parsed.PrimaryKey)
                : null;
        }

        // ---- Menu shortcut detection ----

        /// <summary>
        /// Convert a chord string to a Keys value (with modifier flags set).
        /// Returns Keys.None on failure.
        /// </summary>
        public static Keys ChordToKeys(string chord)
        {
            var parsed = Parse(chord);
            if (parsed == null || parsed.PrimaryKey == null) return Keys.None;
            Keys pk = ParseKeys(parsed.PrimaryKey);
            if (pk == Keys.None) return Keys.None;
            if (parsed.Ctrl) pk |= Keys.Control;
            if (parsed.Shift) pk |= Keys.Shift;
            if (parsed.Alt) pk |= Keys.Alt;
            return pk;
        }

        public static bool IsMenuShortcutChord(string chord, MenuStrip menuStrip)
        {
            Keys chordKey = ChordToKeys(chord);
            if (chordKey == Keys.None) return false;

            foreach (ToolStripMenuItem topItem in menuStrip.Items)
            {
                if (MenuItemHasShortcut(topItem, chordKey))
                    return true;
            }
            return false;
        }

        private static bool MenuItemHasShortcut(ToolStripMenuItem item, Keys chordKey)
        {
            if (item.ShortcutKeys != Keys.None && item.ShortcutKeys == chordKey)
                return true;
            foreach (ToolStripMenuItem sub in item.DropDownItems.OfType<ToolStripMenuItem>())
            {
                if (MenuItemHasShortcut(sub, chordKey))
                    return true;
            }
            return false;
        }

        // ---- .NET 3.5 compatibility ----

        public static Keys ParseKeys(string name)
        {
            try { return (Keys)Enum.Parse(typeof(Keys), name); }
            catch { return Keys.None; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace _3dedit
{
    /// <summary>
    /// Represents the parsed components of a chord string.
    /// </summary>
    public class ChordParts
    {
        public bool Ctrl { get; set; }
        public bool Shift { get; set; }
        public bool Alt { get; set; }
        public string PrimaryKey { get; set; }
    }

    /// <summary>
    /// Utility for chord string normalization, validation, and fallback.
    ///
    /// Chord format: zero or more modifiers ("Ctrl", "Shift", "Alt")
    /// joined by '+' with exactly one primary key at the end.
    ///
    /// Normalized modifier order: Ctrl > Shift > Alt.
    /// Modifier keys as primary keys use their Keys.ToString() name
    /// (e.g. "ShiftKey", "ControlKey", "Menu", "LShiftKey", "RControlKey"...).
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

        public static bool IsModifierKey(Keys keyCode) => ModifierKeyCodes.Contains(keyCode);

        /// <summary>
        /// Returns the modifier flag (Keys.Control / Keys.Shift / Keys.Alt)
        /// for a modifier key code, or Keys.None if it's not a modifier.
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

        /// <summary>
        /// Build a normalized chord string from modifier flags and a primary key name.
        /// </summary>
        public static string BuildChord(bool ctrl, bool shift, bool alt, string primaryKey)
        {
            var parts = new List<string>(4);
            if (ctrl) parts.Add("Ctrl");
            if (shift) parts.Add("Shift");
            if (alt) parts.Add("Alt");
            parts.Add(primaryKey);
            return string.Join("+", parts.ToArray());
        }

        /// <summary>
        /// Build a normalized chord string from modifier flags and a Keys value.
        /// </summary>
        public static string BuildChord(bool ctrl, bool shift, bool alt, Keys primaryKey)
            => BuildChord(ctrl, shift, alt, primaryKey.ToString());

        // ---- Parsing ----

        /// <summary>
        /// Parse a chord string into its components.
        /// Returns null on invalid input.
        /// </summary>
        public static ChordParts Parse(string chord)
        {
            if (string.IsNullOrEmpty(chord))
                return null;

            var parts = chord.Split('+');
            if (parts.Length == 0)
                return null;

            var result = new ChordParts { Ctrl = false, Shift = false, Alt = false, PrimaryKey = null };

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

        /// <summary>
        /// Validate a chord string against the freeform modifier rules.
        /// </summary>
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

            // Check for multiple primary keys (e.g. "A+B")
            var parts = chord.Split('+');
            int primaryCount = parts.Count(p =>
                !string.Equals(p, "Ctrl", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(p, "Shift", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(p, "Alt", StringComparison.OrdinalIgnoreCase));

            if (primaryCount > 1)
            {
                error = "Only one primary key is allowed";
                return false;
            }

            // A primary key named exactly like a modifier label is not allowed
            if (ModifierLabels.Contains(parsed.PrimaryKey))
            {
                error = "This chord format is not supported";
                return false;
            }

            return true;
        }

        // ---- Normalization ----

        /// <summary>
        /// Normalize a chord string to canonical form: modifiers in Ctrl > Shift > Alt order,
        /// capitalised consistently.
        /// Returns null if the chord cannot be parsed.
        /// </summary>
        public static string Normalize(string chord)
        {
            var parsed = Parse(chord);
            return parsed != null && parsed.PrimaryKey != null
                ? BuildChord(parsed.Ctrl, parsed.Shift, parsed.Alt, parsed.PrimaryKey)
                : null;
        }

        // ---- Fallback chain ----

        // Fixed priority list matching plan §5.2
        private static readonly bool[][] FallbackPriority =
        {
            new[] { true,  true,  true  },  // Ctrl+Shift+Alt
            new[] { true,  true,  false },  // Ctrl+Shift
            new[] { true,  false, true  },  // Ctrl+Alt
            new[] { false, true,  true  },  // Shift+Alt
            new[] { true,  false, false },  // Ctrl
            new[] { false, true,  false },  // Shift
            new[] { false, false, true  },  // Alt
            new[] { false, false, false },  // (none)
        };

        /// <summary>
        /// Enumerate fallback chords in priority order, from most modifiers to fewest.
        /// Only yields entries whose modifier set is a subset of the given held modifiers.
        ///
        /// Priority order (fixed):
        ///   Ctrl+Shift+Alt > Ctrl+Shift > Ctrl+Alt > Shift+Alt > Ctrl > Shift > Alt > (none)
        /// </summary>
        public static IEnumerable<string> GetFallbackChain(string chord)
        {
            var parsed = Parse(chord);
            if (parsed == null || parsed.PrimaryKey == null)
                yield break;

            bool ctrl = parsed.Ctrl, shift = parsed.Shift, alt = parsed.Alt;
            string primaryKey = parsed.PrimaryKey;

            foreach (var combo in FallbackPriority)
            {
                // Only yield if the combo's modifier set is a subset of the held modifiers
                if ((!combo[0] || ctrl) && (!combo[1] || shift) && (!combo[2] || alt))
                {
                    yield return BuildChord(combo[0], combo[1], combo[2], primaryKey);
                }
            }
        }

        /// <summary>
        /// Check if a chord is a bare modifier-as-primary-key (e.g. "ShiftKey", "LControlKey").
        /// </summary>
        public static bool IsModifierPrimaryKey(string chord)
        {
            var parsed = Parse(chord);
            if (parsed == null || parsed.PrimaryKey == null) return false;
            if (parsed.Ctrl || parsed.Shift || parsed.Alt) return false; // has real modifiers

            Keys key = ParseKeys(parsed.PrimaryKey);
            return ModifierKeyCodes.Contains(key);
        }

        // ---- .NET 3.5 compatibility helpers ----

        /// <summary>
        /// TryParse Keys enum (missing in .NET 3.5). Returns Keys.None on failure.
        /// </summary>
        public static Keys ParseKeys(string name)
        {
            try { return (Keys)Enum.Parse(typeof(Keys), name); }
            catch { return Keys.None; }
        }
    }
}

using System;
using System.Windows.Forms;

namespace _3dedit
{
    /// <summary>
    /// Represents a key chord: a physical key plus optional modifier flags.
    ///
    /// Design inspired by Hyperspeedcube's KeyCombo:
    /// - Self-exclusion: if the Key IS a modifier (e.g. ShiftKey, LControlKey),
    ///   the corresponding modifier flag is cleared so the key press doesn't
    ///   "double-count" as both a modifier and a primary key.
    /// - Modifier state is read from the OS (Control.ModifierKeys) rather than
    ///   tracked manually via KeyDown/KeyUp events.
    /// </summary>
    public struct KeyCombo : IEquatable<KeyCombo>
    {
        public Keys Key { get; }
        public bool Ctrl { get; }
        public bool Shift { get; }
        public bool Alt { get; }

        /// <summary>
        /// Create a KeyCombo with self-excluding modifier flags.
        /// If <paramref name="key"/> is a modifier key (Shift/Ctrl/Alt),
        /// the corresponding flag is forced to false.
        /// </summary>
        public KeyCombo(Keys key, bool ctrl, bool shift, bool alt)
        {
            Keys flag = ChordUtils.GetModifierFlag(key);
            if (flag == Keys.Control) ctrl = false;
            else if (flag == Keys.Shift) shift = false;
            else if (flag == Keys.Alt) alt = false;

            Key = key;
            Ctrl = ctrl;
            Shift = shift;
            Alt = alt;
        }

        /// <summary>
        /// Capture the current OS modifier state and pair it with a key code.
        /// This is the primary factory used during key event handling.
        /// </summary>
        public static KeyCombo FromKeyPress(Keys keyCode)
        {
            Keys mods = Control.ModifierKeys;
            return new KeyCombo(
                keyCode,
                (mods & Keys.Control) != 0,
                (mods & Keys.Shift) != 0,
                (mods & Keys.Alt) != 0
            );
        }

        /// <summary>
        /// Serialize to a canonical chord string (e.g. "Ctrl+Shift+D").
        /// </summary>
        public string ToChordString()
        {
            return ChordUtils.BuildChord(Ctrl, Shift, Alt, Key.ToString());
        }

        public override string ToString() => ToChordString();

        public override bool Equals(object obj)
        {
            return obj is KeyCombo other && Equals(other);
        }

        public bool Equals(KeyCombo other)
        {
            return Key == other.Key && Ctrl == other.Ctrl
                && Shift == other.Shift && Alt == other.Alt;
        }

        public override int GetHashCode()
        {
            int h = (int)Key;
            if (Ctrl) h ^= 0x10000;
            if (Shift) h ^= 0x20000;
            if (Alt) h ^= 0x40000;
            return h;
        }

        public static bool operator ==(KeyCombo a, KeyCombo b) => a.Equals(b);
        public static bool operator !=(KeyCombo a, KeyCombo b) => !a.Equals(b);
    }
}

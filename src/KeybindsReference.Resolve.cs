using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace _3dedit
{
    public partial class KeybindsReference
    {
        /// <summary>
        /// Editable map of menu shortcut Keys → 2-line key display text
        /// (use \n to split; single-line is centred vertically).
        /// </summary>
        static readonly Dictionary<Keys, string> MenuShortcutDisplay = new Dictionary<Keys, string>
        {
            { Keys.Control | Keys.O, "Open"  },
            { Keys.Control | Keys.S, "Save"  },
            { Keys.Alt   | Keys.F4, "Exit"   },
            { Keys.Control | Keys.R, "Reset" },
            { Keys.Control | Keys.Z, "Undo"  },
            { Keys.Control | Keys.Y, "Redo"  },
            { Keys.Control | Keys.C, "Stop"  },
            { Keys.Control | Keys.M, "Mrec"  },
            { Keys.F1, "ST"   },
            { Keys.F2, "STP"  },
            { Keys.F3, "conj" },
            { Keys.F4, "cmu"  },
        };

        /// <summary>
        /// Editable map of menu shortcut Keys → tooltip text (single line).
        /// </summary>
        static readonly Dictionary<Keys, string> MenuShortcutTooltips = new Dictionary<Keys, string>
        {
            { Keys.Control | Keys.O, "Open"       },
            { Keys.Control | Keys.S, "Save"       },
            { Keys.Alt   | Keys.F4, "Exit"        },
            { Keys.Control | Keys.R, "Reset"      },
            { Keys.Control | Keys.Z, "Undo"       },
            { Keys.Control | Keys.Y, "Redo"       },
            { Keys.Control | Keys.C, "Stop"       },
            { Keys.Control | Keys.M, "Macro record"},
            { Keys.F1, "Start extra turns"  },
            { Keys.F2, "Stop extra turns"   },
            { Keys.F3, "conjugate"  },
            { Keys.F4, "commutator" },
        };

        /// <summary>Look up a menu shortcut in the given dictionary, falling back to the MenuStrip item text.</summary>
        string FindMenuShortcut(Dictionary<Keys, string> dict, Keys keyCode, bool ctrl, bool shift, bool alt)
        {
            if (keyCode == Keys.None) return null;
            Keys target = keyCode;
            if (ctrl)  target |= Keys.Control;
            if (shift) target |= Keys.Shift;
            if (alt)   target |= Keys.Alt;

            string name;
            if (dict.TryGetValue(target, out name))
                return name;
            return FallbackMenuShortcut(target);
        }

        string FindMenuShortcutDescription(Keys keyCode, bool ctrl, bool shift, bool alt)
        {
            return FindMenuShortcut(MenuShortcutDisplay, keyCode, ctrl, shift, alt);
        }

        string FindMenuShortcutTooltip(Keys keyCode, bool ctrl, bool shift, bool alt)
        {
            return FindMenuShortcut(MenuShortcutTooltips, keyCode, ctrl, shift, alt);
        }

        string FallbackMenuShortcut(Keys target)
        {
            foreach (ToolStripMenuItem top in _menu.Items)
            {
                string result = SearchMenuItem(top, target);
                if (result != null) return result;
            }
            return null;
        }

        static string SearchMenuItem(ToolStripMenuItem item, Keys target)
        {
            if (item.ShortcutKeys == target)
                return item.Text.Replace("&", "");

            foreach (ToolStripMenuItem sub in item.DropDownItems.OfType<ToolStripMenuItem>())
            {
                string result = SearchMenuItem(sub, target);
                if (result != null) return result;
            }
            return null;
        }

        /// <summary>Map left/right modifier key codes to their generic form.</summary>
        static string NormaliseModKey(Keys code)
        {
            if (code == Keys.LShiftKey || code == Keys.RShiftKey) return "ShiftKey";
            if (code == Keys.LControlKey || code == Keys.RControlKey) return "ControlKey";
            if (code == Keys.LMenu || code == Keys.RMenu) return "Menu";
            return code.ToString();
        }

        /// <summary>
        /// Common resolve flow: detect pressed modifiers → check menu shortcut
        /// → check keybinding → return text.  The two type-specific parameters
        /// let ResolveDescription / ResolveTooltip share ~30 lines of logic.
        /// </summary>
        string ResolveForKey(KeyDef k,
            Func<Keys, bool, bool, bool, string> menuLookup,
            Func<Keybindings.IAction, string> textGetter)
        {
            if (_keybinds == null || _keybinds.activeKeybinds == null)
                return "";

            // Detect currently pressed modifiers so the description shows
            // Shift+Key / Ctrl+Key / Shift+Ctrl+Key bindings when appropriate.
            bool ctrl  = (GetAsyncKeyState((int)Keys.ControlKey) & 0x8000) != 0
                      || (GetAsyncKeyState((int)Keys.LControlKey) & 0x8000) != 0;
            bool shift = (GetAsyncKeyState((int)Keys.ShiftKey) & 0x8000) != 0
                      || (GetAsyncKeyState((int)Keys.LShiftKey) & 0x8000) != 0;
            bool alt   = (GetAsyncKeyState((int)Keys.Menu) & 0x8000) != 0
                      || (GetAsyncKeyState((int)Keys.LMenu) & 0x8000) != 0;

            // WinForms menu shortcuts (ProcessCmdKey) take priority over
            // keybindings (KeyDownEvt).  Check them first so the display
            // matches what actually executes when the key is pressed.
            if (_menu != null)
            {
                string menuText = menuLookup(k.Code, ctrl, shift, alt);
                if (menuText != null) return menuText;
            }

            // Keybinding lookup with consumed-modifier awareness
            string keyName = NormaliseModKey(k.Code);
            var action = _keybinds.GetActionWithFallback(keyName, ctrl, shift, alt, this.ConsumedModifiers);
            if (action != null)
                return textGetter(action);

            return "";
        }

        string ResolveDescription(KeyDef k)
        {
            return ResolveForKey(k, FindMenuShortcutDescription, a => a.GetDescription());
        }

        string ResolveTooltip(KeyDef k)
        {
            return ResolveForKey(k, FindMenuShortcutTooltip, a => a.GetTooltip());
        }
    }
}

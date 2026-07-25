using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace _3dedit {
    internal sealed class CStructuredMacroFile {
        const string Header = "MC7D Structured Macro File";

        internal readonly List<CStructuredMacro> Macros = new List<CStructuredMacro>();
        internal string FileName;
        int Dim;
        int Size;

        internal CStructuredMacroFile(int dim, int size) {
            Dim = dim;
            Size = size;
        }

        internal CStructuredMacroFile(string fileName) {
            Load(fileName);
        }

        internal bool CheckSize(int dim, int size) {
            return dim == Dim && size == Size;
        }

        internal void SaveAs(string fileName) {
            FileName = fileName;
            Save();
        }

        internal void Save() {
            if(FileName == null) throw new InvalidOperationException("Structured macro file name is not set.");

            using(StreamWriter sw = new StreamWriter(FileName)) {
                sw.NewLine = "\r\n";
                sw.WriteLine(Header);
                sw.WriteLine("version 1");
                sw.WriteLine("size {0} {1} {2}", Dim, Size, Macros.Count);
                for(int i=0;i<Macros.Count;i++) WriteMacro(sw, Macros[i]);
            }
        }

        void Load(string fileName) {
            using(StreamReader sr = new StreamReader(fileName)) {
                if(ReadRequired(sr) != Header) throw new FormatException("Not an MC7D structured macro file.");
                string versionLine = ReadRequired(sr);
                if(versionLine != "version 1") throw new FormatException("Unsupported structured macro file version.");

                string[] sizeParts = Split(ReadRequired(sr));
                if(sizeParts.Length != 4 || sizeParts[0] != "size")
                    throw new FormatException("Invalid structured macro file size line.");
                Dim = int.Parse(sizeParts[1], CultureInfo.InvariantCulture);
                Size = int.Parse(sizeParts[2], CultureInfo.InvariantCulture);
                int macroCount = int.Parse(sizeParts[3], CultureInfo.InvariantCulture);

                for(int i=0;i<macroCount;i++) {
                    CStructuredMacro macro = ReadMacro(sr);
                    if(StructuredMacroNames.FindIndex(Macros, macro.Name, null) >= 0)
                        throw new FormatException("Duplicate structured macro name: " + macro.Name);
                    Macros.Add(macro);
                }
            }
            FileName = fileName;
        }

        static void WriteMacro(StreamWriter sw, CStructuredMacro macro) {
            sw.WriteLine("macro {0}", macro.Name);
            sw.WriteLine("stickers {0}", macro.NStickers);
            if(macro.NStickers > 0) {
                for(int i=0;i<macro.NStickers;i++) {
                    if(i > 0) sw.Write(' ');
                    sw.Write(macro.Stickers[i].ToString(CultureInfo.InvariantCulture));
                }
                sw.WriteLine();
            }

            if(macro.Vectors != null && macro.Orient != null) {
                sw.Write("dir");
                for(int i=0;i<macro.Vectors.Length;i++)
                    sw.Write(" {0}", macro.Vectors[i].ToString("R", CultureInfo.InvariantCulture));
                for(int i=0;i<macro.Orient.Length;i++)
                    sw.Write(" {0}", macro.Orient[i].ToString(CultureInfo.InvariantCulture));
                sw.WriteLine();
            }

            List<StructuredTwist> twists = macro.GetTwistsSorted();
            sw.WriteLine("twists {0}", twists.Count);
            for(int i=0;i<twists.Count;i++) {
                StructuredTwist twist = twists[i];
                sw.WriteLine("twist {0} {1} {2} {3} {4}",
                    twist.Id.ToString(CultureInfo.InvariantCulture),
                    StructuredAxis.Format(twist.SignedGripAxis),
                    StructuredAxis.FormatName(twist.FromAxis),
                    StructuredAxis.FormatName(twist.ToAxis),
                    twist.DefaultMask.ToString(CultureInfo.InvariantCulture));
            }
            sw.WriteLine("ast {0}", macro.RootNode.ToExpression());
            sw.WriteLine("end");
        }

        static CStructuredMacro ReadMacro(StreamReader sr) {
            string[] macroParts = Split(ReadRequired(sr));
            if(macroParts.Length != 2 || macroParts[0] != "macro")
                throw new FormatException("Expected structured macro header.");

            CStructuredMacro macro = new CStructuredMacro(macroParts[1]);

            string[] stickerParts = Split(ReadRequired(sr));
            if(stickerParts.Length != 2 || stickerParts[0] != "stickers")
                throw new FormatException("Expected structured macro stickers line.");
            macro.NStickers = int.Parse(stickerParts[1], CultureInfo.InvariantCulture);
            if(macro.NStickers > 0) {
                string[] values = Split(ReadRequired(sr));
                if(values.Length != macro.NStickers)
                    throw new FormatException("Structured macro sticker count mismatch.");
                macro.Stickers = new int[macro.NStickers];
                for(int i=0;i<macro.NStickers;i++)
                    macro.Stickers[i] = int.Parse(values[i], CultureInfo.InvariantCulture);
            } else {
                macro.Stickers = null;
            }

            string line = ReadRequired(sr);
            string[] parts = Split(line);
            if(parts.Length > 0 && parts[0] == "dir") {
                if(parts.Length < 10) throw new FormatException("Invalid structured macro direction line.");
                macro.Vectors = new double[9];
                for(int i=0;i<9;i++)
                    macro.Vectors[i] = double.Parse(parts[i + 1], CultureInfo.InvariantCulture);
                int orientCount = parts.Length - 10;
                macro.Orient = new int[orientCount];
                for(int i=0;i<orientCount;i++)
                    macro.Orient[i] = int.Parse(parts[i + 10], CultureInfo.InvariantCulture);
                line = ReadRequired(sr);
                parts = Split(line);
            }

            if(parts.Length != 2 || parts[0] != "twists")
                throw new FormatException("Expected structured macro twists line.");
            int twistCount = int.Parse(parts[1], CultureInfo.InvariantCulture);
            for(int i=0;i<twistCount;i++) {
                string[] twistParts = Split(ReadRequired(sr));
                if(twistParts.Length != 6 || twistParts[0] != "twist")
                    throw new FormatException("Invalid structured macro twist line.");
                int id = int.Parse(twistParts[1], CultureInfo.InvariantCulture);
                int grip = StructuredAxis.Parse(twistParts[2]);
                int from = StructuredAxis.Parse(twistParts[3]);
                int to = StructuredAxis.Parse(twistParts[4]);
                int mask = int.Parse(twistParts[5], CultureInfo.InvariantCulture);
                macro.AddTwistWithId(new StructuredTwist(id, grip, from, to, mask));
            }

            string astLine = ReadRequired(sr);
            if(!astLine.StartsWith("ast ", StringComparison.Ordinal))
                throw new FormatException("Expected structured macro AST line.");
            macro.RootNode = StructuredAstParser.Parse(astLine.Substring(4));
            macro.Compile(null, false);

            if(ReadRequired(sr) != "end") throw new FormatException("Expected structured macro end line.");
            return macro;
        }

        static string ReadRequired(StreamReader sr) {
            string line = sr.ReadLine();
            if(line == null) throw new EndOfStreamException();
            return line.Trim();
        }

        static string[] Split(string line) {
            return line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }

    internal static class StructuredAstParser {
        internal static StructuredSequenceNode Parse(string text) {
            Parser parser = new Parser(text == null ? "" : text);
            StructuredSequenceNode root = parser.ParseSequence('\0');
            parser.RequireEnd();
            return root;
        }

        sealed class Parser {
            readonly string m_text;
            int m_pos;

            internal Parser(string text) {
                m_text = text;
            }

            internal StructuredSequenceNode ParseSequence(char terminator) {
                StructuredSequenceNode sequence = new StructuredSequenceNode();
                while(true) {
                    SkipSpaces();
                    if(m_pos >= m_text.Length) {
                        if(terminator != '\0') throw new FormatException("Unclosed structured macro expression.");
                        return sequence;
                    }
                    if(terminator != '\0' && m_text[m_pos] == terminator) return sequence;
                    sequence.Children.Add(ParseNode());
                }
            }

            internal void RequireEnd() {
                SkipSpaces();
                if(m_pos != m_text.Length)
                    throw new FormatException("Unexpected structured macro expression text.");
            }

            StructuredMacroNode ParseNode() {
                SkipSpaces();
                if(m_pos >= m_text.Length) throw new FormatException("Unexpected end of structured macro expression.");

                char c = m_text[m_pos];
                if(c == 'T') return ParseTwist();
                if(c == '(') return ParsePower();
                if(c == '[') return ParseBracket();
                throw new FormatException("Invalid structured macro expression near: " + m_text.Substring(m_pos));
            }

            StructuredMacroNode ParseTwist() {
                m_pos++;
                int id = ParsePositiveInt();
                return new StructuredTwistNode(id);
            }

            StructuredMacroNode ParsePower() {
                m_pos++;
                SkipSpaces();
                StructuredMacroNode inner = ParseNode();
                SkipSpaces();
                Expect(')');
                SkipSpaces();
                Expect('2');
                StructuredTwistNode twist = inner as StructuredTwistNode;
                if(twist == null) throw new FormatException("Only primitive twist square power is supported.");
                return new StructuredPowerNode(twist, 2);
            }

            StructuredMacroNode ParseBracket() {
                m_pos++;
                StructuredSequenceNode a = ParseSequence(':', ',');
                char separator = m_text[m_pos];
                if(separator != ':' && separator != ',') throw new FormatException("Expected ':' or ','.");
                m_pos++;
                StructuredSequenceNode b = ParseSequence(']');
                Expect(']');
                if(separator == ':') return new StructuredConjugateNode(a, b);
                return new StructuredCommutatorNode(a, b);
            }

            StructuredSequenceNode ParseSequence(char terminator1, char terminator2) {
                StructuredSequenceNode sequence = new StructuredSequenceNode();
                while(true) {
                    SkipSpaces();
                    if(m_pos >= m_text.Length) throw new FormatException("Unclosed structured macro expression.");
                    if(m_text[m_pos] == terminator1 || m_text[m_pos] == terminator2) return sequence;
                    sequence.Children.Add(ParseNode());
                }
            }

            int ParsePositiveInt() {
                SkipSpaces();
                int start = m_pos;
                while(m_pos < m_text.Length && char.IsDigit(m_text[m_pos])) m_pos++;
                if(start == m_pos) throw new FormatException("Expected positive integer.");
                int value = int.Parse(m_text.Substring(start, m_pos - start), CultureInfo.InvariantCulture);
                if(value <= 0) throw new FormatException("Expected positive integer.");
                return value;
            }

            void SkipSpaces() {
                while(m_pos < m_text.Length && char.IsWhiteSpace(m_text[m_pos])) m_pos++;
            }

            void Expect(char c) {
                if(m_pos >= m_text.Length || m_text[m_pos] != c)
                    throw new FormatException("Expected '" + c + "'.");
                m_pos++;
            }
        }
    }
}

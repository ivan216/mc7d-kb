using System;
using System.Collections.Generic;
using System.Text;

namespace _3dedit {
    internal static class StructuredAxis {
        static readonly string[] Names = new string[] { null, "W", "X", "Z", "Y", "V", "U", "T" };

        internal static void ValidateSignedAxis(int axis) {
            int a = Math.Abs(axis);
            if(axis == 0 || a >= Names.Length)
                throw new ArgumentException("Invalid signed axis: " + axis);
        }

        internal static int Parse(string text) {
            if(text == null) throw new ArgumentNullException("text");
            text = text.Trim();
            if(text.StartsWith("{") && text.EndsWith("}"))
                text = text.Substring(1, text.Length - 2).Trim();

            int sign = 1;
            if(text.StartsWith("+")) text = text.Substring(1);
            else if(text.StartsWith("-")) {
                sign = -1;
                text = text.Substring(1);
            }

            for(int i=1;i<Names.Length;i++)
                if(string.Equals(Names[i], text, StringComparison.OrdinalIgnoreCase))
                    return sign * i;
            throw new ArgumentException("Invalid axis name: " + text);
        }

        internal static string Format(int signedAxis) {
            ValidateSignedAxis(signedAxis);
            return (signedAxis > 0 ? "+" : "-") + Names[Math.Abs(signedAxis)];
        }

        internal static int Map(int signedAxis, int[] map) {
            ValidateSignedAxis(signedAxis);
            if(map == null) return signedAxis;

            int a = Math.Abs(signedAxis);
            if(a >= map.Length || map[a] == 0)
                throw new ArgumentException("Axis map does not contain axis " + a);

            return signedAxis > 0 ? map[a] : -map[a];
        }

        internal static bool DistinctAbs(int a, int b, int c) {
            a = Math.Abs(a);
            b = Math.Abs(b);
            c = Math.Abs(c);
            return a != b && a != c && b != c;
        }
    }

    internal sealed class StructuredTwist {
        internal int Id;
        internal int SignedGripAxis;
        internal int FromAxis;
        internal int ToAxis;
        internal int DefaultMask;

        internal StructuredTwist(int id, int signedGripAxis, int fromAxis, int toAxis, int defaultMask) {
            Id = id;
            SignedGripAxis = signedGripAxis;
            FromAxis = fromAxis;
            ToAxis = toAxis;
            DefaultMask = Math.Abs(defaultMask);
            Validate();
        }

        internal void Validate() {
            StructuredAxis.ValidateSignedAxis(SignedGripAxis);
            StructuredAxis.ValidateSignedAxis(FromAxis);
            StructuredAxis.ValidateSignedAxis(ToAxis);
            if(DefaultMask <= 0) throw new ArgumentException("defaultMask must be positive.");
            if(!StructuredAxis.DistinctAbs(SignedGripAxis, FromAxis, ToAxis))
                throw new ArgumentException("Twist axes must be pairwise distinct.");
        }

        internal StructuredTwist CopyWithId(int id) {
            return new StructuredTwist(id, SignedGripAxis, FromAxis, ToAxis, DefaultMask);
        }

        internal StructuredTwist InvertWithId(int id) {
            return new StructuredTwist(id, SignedGripAxis, ToAxis, FromAxis, DefaultMask);
        }

        internal bool SameMove(StructuredTwist other) {
            return other != null
                && SignedGripAxis == other.SignedGripAxis
                && FromAxis == other.FromAxis
                && ToAxis == other.ToAxis
                && DefaultMask == other.DefaultMask;
        }

        internal bool InverseMove(StructuredTwist other) {
            return other != null
                && SignedGripAxis == other.SignedGripAxis
                && FromAxis == other.ToAxis
                && ToAxis == other.FromAxis
                && DefaultMask == other.DefaultMask;
        }

        internal string ToTwistString() {
            return "{" + StructuredAxis.Format(SignedGripAxis) + "}{"
                + StructuredAxis.Format(FromAxis) + "}{"
                + StructuredAxis.Format(ToAxis) + "}";
        }
    }

    internal sealed class CompiledStructuredTwist {
        internal int SourceTwistId;
        internal int SignedGripAxis;
        internal int FromAxis;
        internal int ToAxis;
        internal int LogicalMask;

        internal CompiledStructuredTwist(int sourceTwistId, int signedGripAxis, int fromAxis, int toAxis, int logicalMask) {
            SourceTwistId = sourceTwistId;
            SignedGripAxis = signedGripAxis;
            FromAxis = fromAxis;
            ToAxis = toAxis;
            LogicalMask = logicalMask;
        }

        internal CompiledStructuredTwist Invert() {
            return new CompiledStructuredTwist(SourceTwistId, SignedGripAxis, ToAxis, FromAxis, LogicalMask);
        }
    }

    internal static class StructuredTwistRuntime {
        internal static int ReverseMask(int mask, int size) {
            int res = 0;
            for(int i=0;i<size;i++) {
                res = (res << 1) + (mask & 1);
                mask >>= 1;
            }
            return res;
        }

        internal static int InferSignedGripAxis(int gripAbsAxis, int effectiveMask, int size) {
            if(gripAbsAxis <= 0) throw new ArgumentException("gripAbsAxis must be positive.");
            int top = 1 << (size - 1);
            if((effectiveMask & top) == 0) return gripAbsAxis;
            if((effectiveMask & 1) == 0) return -gripAbsAxis;
            return gripAbsAxis;
        }

        internal static int ToDefaultMask(int signedGripAxis, int effectiveMask, int size) {
            int mask = effectiveMask & ((1 << size) - 1);
            return signedGripAxis < 0 ? ReverseMask(mask, size) : mask;
        }

        internal static StructuredTwist FromCubeTwistInput(int id, int gripAxis, int fromAxis, int toAxis, int cubeMask, int size) {
            StructuredAxis.ValidateSignedAxis(gripAxis);
            StructuredAxis.ValidateSignedAxis(fromAxis);
            StructuredAxis.ValidateSignedAxis(toAxis);

            int effectiveMask = gripAxis < 0 ? cubeMask : ReverseMask(cubeMask, size);
            effectiveMask &= (1 << size) - 1;
            int signedGripAxis = InferSignedGripAxis(Math.Abs(gripAxis), effectiveMask, size);
            int defaultMask = ToDefaultMask(signedGripAxis, effectiveMask, size);
            return new StructuredTwist(id, signedGripAxis, fromAxis, toAxis, defaultMask);
        }

        internal static int ResolveEffectiveMask(int signedGripAxis, int logicalMask, int size) {
            int mask = Math.Abs(logicalMask) & ((1 << size) - 1);
            bool flip = (signedGripAxis < 0) ^ (logicalMask < 0);
            return flip ? ReverseMask(mask, size) : mask;
        }

        internal static void ResolveForCubeTwist(CompiledStructuredTwist twist, int size, int[] axisMap,
            out int gripAxis, out int fromAxis, out int toAxis, out int cubeMask) {
            gripAxis = StructuredAxis.Map(twist.SignedGripAxis, axisMap);
            fromAxis = StructuredAxis.Map(twist.FromAxis, axisMap);
            toAxis = StructuredAxis.Map(twist.ToAxis, axisMap);

            int effectiveMask = ResolveEffectiveMask(gripAxis, twist.LogicalMask, size);
            cubeMask = gripAxis < 0 ? effectiveMask : ReverseMask(effectiveMask, size);
        }
    }

    internal abstract class StructuredMacroNode {
        internal abstract StructuredMacroNode Clone();
        internal abstract string ToExpression();
    }

    internal sealed class StructuredSequenceNode : StructuredMacroNode {
        internal readonly List<StructuredMacroNode> Children = new List<StructuredMacroNode>();

        internal override StructuredMacroNode Clone() {
            StructuredSequenceNode res = new StructuredSequenceNode();
            for(int i=0;i<Children.Count;i++) res.Children.Add(Children[i].Clone());
            return res;
        }

        internal override string ToExpression() {
            StringBuilder sb = new StringBuilder();
            for(int i=0;i<Children.Count;i++) {
                if(i > 0) sb.Append(' ');
                sb.Append(Children[i].ToExpression());
            }
            return sb.ToString();
        }
    }

    internal sealed class StructuredTwistNode : StructuredMacroNode {
        internal int TwistId;

        internal StructuredTwistNode(int twistId) {
            TwistId = twistId;
        }

        internal override StructuredMacroNode Clone() {
            return new StructuredTwistNode(TwistId);
        }

        internal override string ToExpression() {
            return "T" + TwistId;
        }
    }

    internal sealed class StructuredPowerNode : StructuredMacroNode {
        internal StructuredTwistNode Twist;
        internal int Power;

        internal StructuredPowerNode(StructuredTwistNode twist, int power) {
            if(power != 2) throw new ArgumentException("Only primitive square powers are supported.");
            Twist = twist;
            Power = power;
        }

        internal override StructuredMacroNode Clone() {
            return new StructuredPowerNode((StructuredTwistNode)Twist.Clone(), Power);
        }

        internal override string ToExpression() {
            return Twist.ToExpression() + "2";
        }
    }

    internal sealed class StructuredConjugateNode : StructuredMacroNode {
        internal StructuredMacroNode A;
        internal StructuredMacroNode B;

        internal StructuredConjugateNode(StructuredMacroNode a, StructuredMacroNode b) {
            A = a;
            B = b;
        }

        internal override StructuredMacroNode Clone() {
            return new StructuredConjugateNode(A.Clone(), B.Clone());
        }

        internal override string ToExpression() {
            return "[" + A.ToExpression() + ":" + B.ToExpression() + "]";
        }
    }

    internal sealed class StructuredCommutatorNode : StructuredMacroNode {
        internal StructuredMacroNode A;
        internal StructuredMacroNode B;

        internal StructuredCommutatorNode(StructuredMacroNode a, StructuredMacroNode b) {
            A = a;
            B = b;
        }

        internal override StructuredMacroNode Clone() {
            return new StructuredCommutatorNode(A.Clone(), B.Clone());
        }

        internal override string ToExpression() {
            return "[" + A.ToExpression() + "," + B.ToExpression() + "]";
        }
    }

    internal sealed class CStructuredMacro {
        internal string Name;
        internal int NStickers;
        internal int[] Stickers;
        internal double[] Vectors;
        internal int[] Orient;
        internal StructuredSequenceNode RootNode = new StructuredSequenceNode();

        readonly Dictionary<int, StructuredTwist> m_twists = new Dictionary<int, StructuredTwist>();
        int m_nextTwistId = 1;

        internal IEnumerable<StructuredTwist> TwistLeaves {
            get { return m_twists.Values; }
        }

        internal CStructuredMacro(string name) {
            Name = name == null ? "" : name.Replace(' ', '_');
        }

        internal StructuredTwist GetTwist(int id) {
            StructuredTwist twist;
            if(!m_twists.TryGetValue(id, out twist))
                throw new ArgumentException("Unknown structured twist id: " + id);
            return twist;
        }

        internal int AddTwist(int signedGripAxis, int fromAxis, int toAxis, int defaultMask) {
            int id = m_nextTwistId++;
            m_twists.Add(id, new StructuredTwist(id, signedGripAxis, fromAxis, toAxis, defaultMask));
            return id;
        }

        internal int AddTwist(StructuredTwist twist) {
            int id = m_nextTwistId++;
            m_twists.Add(id, twist.CopyWithId(id));
            return id;
        }

        internal void AppendPrimitive(StructuredSequenceNode sequence, StructuredTwist twist) {
            if(sequence == null) throw new ArgumentNullException("sequence");
            twist.Validate();

            if(TryMergePrimitive(sequence, twist)) return;

            int id = AddTwist(twist);
            sequence.Children.Add(new StructuredTwistNode(id));
        }

        bool TryMergePrimitive(StructuredSequenceNode sequence, StructuredTwist incoming) {
            if(sequence.Children.Count == 0) return false;

            StructuredTwistNode lastTwistNode;
            int oldPower;
            StructuredMacroNode last = sequence.Children[sequence.Children.Count - 1];
            if(last is StructuredTwistNode) {
                lastTwistNode = (StructuredTwistNode)last;
                oldPower = 1;
            } else if(last is StructuredPowerNode) {
                StructuredPowerNode power = (StructuredPowerNode)last;
                lastTwistNode = power.Twist;
                oldPower = power.Power;
            } else {
                return false;
            }

            StructuredTwist oldTwist = GetTwist(lastTwistNode.TwistId);
            int delta;
            if(oldTwist.SameMove(incoming)) delta = 1;
            else if(oldTwist.InverseMove(incoming)) delta = -1;
            else return false;

            int newPower = (oldPower + delta) % 4;
            if(newPower < 0) newPower += 4;

            int lastIndex = sequence.Children.Count - 1;
            if(newPower == 0) {
                sequence.Children.RemoveAt(lastIndex);
            } else if(newPower == 1) {
                sequence.Children[lastIndex] = new StructuredTwistNode(oldTwist.Id);
            } else if(newPower == 2) {
                sequence.Children[lastIndex] = new StructuredPowerNode(new StructuredTwistNode(oldTwist.Id), 2);
            } else {
                int invertedId = AddTwist(oldTwist.InvertWithId(0));
                sequence.Children[lastIndex] = new StructuredTwistNode(invertedId);
            }
            return true;
        }

        internal List<CompiledStructuredTwist> Compile(IDictionary<int, int> overrideMasks, bool reverse) {
            List<CompiledStructuredTwist> res = new List<CompiledStructuredTwist>();
            ExpandNode(RootNode, overrideMasks, res);
            if(reverse) res = InvertList(res);
            return res;
        }

        void ExpandNode(StructuredMacroNode node, IDictionary<int, int> overrideMasks, List<CompiledStructuredTwist> output) {
            if(node is StructuredSequenceNode) {
                StructuredSequenceNode seq = (StructuredSequenceNode)node;
                for(int i=0;i<seq.Children.Count;i++) ExpandNode(seq.Children[i], overrideMasks, output);
            } else if(node is StructuredTwistNode) {
                AddCompiledTwist(((StructuredTwistNode)node).TwistId, overrideMasks, output);
            } else if(node is StructuredPowerNode) {
                StructuredPowerNode power = (StructuredPowerNode)node;
                AddCompiledTwist(power.Twist.TwistId, overrideMasks, output);
                AddCompiledTwist(power.Twist.TwistId, overrideMasks, output);
            } else if(node is StructuredConjugateNode) {
                StructuredConjugateNode conj = (StructuredConjugateNode)node;
                List<CompiledStructuredTwist> a = new List<CompiledStructuredTwist>();
                ExpandNode(conj.A, overrideMasks, a);
                AddRange(output, a);
                ExpandNode(conj.B, overrideMasks, output);
                AddRange(output, InvertList(a));
            } else if(node is StructuredCommutatorNode) {
                StructuredCommutatorNode comm = (StructuredCommutatorNode)node;
                List<CompiledStructuredTwist> a = new List<CompiledStructuredTwist>();
                List<CompiledStructuredTwist> b = new List<CompiledStructuredTwist>();
                ExpandNode(comm.A, overrideMasks, a);
                ExpandNode(comm.B, overrideMasks, b);
                AddRange(output, a);
                AddRange(output, b);
                AddRange(output, InvertList(a));
                AddRange(output, InvertList(b));
            } else {
                throw new ArgumentException("Unknown structured macro node.");
            }
        }

        void AddCompiledTwist(int twistId, IDictionary<int, int> overrideMasks, List<CompiledStructuredTwist> output) {
            StructuredTwist twist = GetTwist(twistId);
            int mask = twist.DefaultMask;
            if(overrideMasks != null && overrideMasks.ContainsKey(twistId))
                mask = overrideMasks[twistId];

            output.Add(new CompiledStructuredTwist(twistId, twist.SignedGripAxis, twist.FromAxis, twist.ToAxis, mask));
        }

        static List<CompiledStructuredTwist> InvertList(List<CompiledStructuredTwist> list) {
            List<CompiledStructuredTwist> res = new List<CompiledStructuredTwist>();
            for(int i=list.Count-1;i>=0;i--) res.Add(list[i].Invert());
            return res;
        }

        static void AddRange(List<CompiledStructuredTwist> target, List<CompiledStructuredTwist> source) {
            for(int i=0;i<source.Count;i++) target.Add(source[i]);
        }
    }

    internal sealed class StructuredMacroRecorder {
        sealed class Frame {
            internal StructuredSequenceNode A = new StructuredSequenceNode();
            internal StructuredSequenceNode B = new StructuredSequenceNode();
            internal bool InB;
        }

        readonly List<Frame> m_frames = new List<Frame>();
        CStructuredMacro m_macro;
        int m_suppressDepth;

        internal bool IsRecording {
            get { return m_macro != null; }
        }

        internal bool SuppressRecording {
            get { return m_suppressDepth > 0; }
        }

        internal CStructuredMacro CurrentMacro {
            get { return m_macro; }
        }

        internal void Begin(CStructuredMacro macro) {
            if(macro == null) throw new ArgumentNullException("macro");
            m_macro = macro;
            m_frames.Clear();
            m_suppressDepth = 0;
        }

        internal void Cancel() {
            m_macro = null;
            m_frames.Clear();
            m_suppressDepth = 0;
        }

        internal bool TryFinish(out CStructuredMacro macro, out string error) {
            macro = null;
            error = null;
            if(!IsRecording) {
                error = "Structured macro recording is not active.";
                return false;
            }
            if(m_frames.Count != 0) {
                error = "Structured macro has unclosed F1/F2 frame.";
                return false;
            }

            macro = m_macro;
            Cancel();
            return true;
        }

        internal IDisposable Suppress() {
            m_suppressDepth++;
            return new Suppression(this);
        }

        internal void RecordCubeTwistInput(int gripAxis, int fromAxis, int toAxis, int cubeMask, int size) {
            if(!IsRecording || SuppressRecording) return;
            StructuredTwist twist = StructuredTwistRuntime.FromCubeTwistInput(0, gripAxis, fromAxis, toAxis, cubeMask, size);
            m_macro.AppendPrimitive(CurrentSequence(), twist);
        }

        internal void BeginFrame() {
            if(!IsRecording) return;
            m_frames.Add(new Frame());
        }

        internal bool SplitFrame(out string error) {
            error = null;
            if(!IsRecording) return true;
            if(m_frames.Count == 0) {
                error = "F2 requires an active F1 frame.";
                return false;
            }

            Frame frame = CurrentFrame();
            if(frame.InB || frame.A.Children.Count == 0) {
                m_frames.RemoveAt(m_frames.Count - 1);
                error = "F2 requires a non-empty A sequence and no existing B sequence.";
                return false;
            }

            frame.InB = true;
            return true;
        }

        internal bool EndConjugate(out string error) {
            return EndFrame(true, out error);
        }

        internal bool EndCommutator(out string error) {
            return EndFrame(false, out error);
        }

        bool EndFrame(bool conjugate, out string error) {
            error = null;
            if(!IsRecording) return true;
            if(m_frames.Count == 0) {
                error = conjugate ? "F3 requires a complete F1/F2 frame." : "F4 requires a complete F1/F2 frame.";
                return false;
            }

            Frame frame = CurrentFrame();
            m_frames.RemoveAt(m_frames.Count - 1);
            if(!frame.InB || frame.A.Children.Count == 0 || frame.B.Children.Count == 0) {
                error = conjugate ? "F3 requires non-empty A and B sequences." : "F4 requires non-empty A and B sequences.";
                return false;
            }

            StructuredMacroNode node = conjugate
                ? (StructuredMacroNode)new StructuredConjugateNode(frame.A, frame.B)
                : (StructuredMacroNode)new StructuredCommutatorNode(frame.A, frame.B);
            CurrentSequence().Children.Add(node);
            return true;
        }

        StructuredSequenceNode CurrentSequence() {
            if(m_frames.Count == 0) return m_macro.RootNode;
            Frame frame = CurrentFrame();
            return frame.InB ? frame.B : frame.A;
        }

        Frame CurrentFrame() {
            return m_frames[m_frames.Count - 1];
        }

        sealed class Suppression : IDisposable {
            StructuredMacroRecorder m_owner;

            internal Suppression(StructuredMacroRecorder owner) {
                m_owner = owner;
            }

            public void Dispose() {
                if(m_owner != null) {
                    m_owner.m_suppressDepth--;
                    m_owner = null;
                }
            }
        }
    }

    internal sealed class StructuredRktSelection {
        internal int TwistId;
        internal int TargetCell;
        internal int AdjustCell;
        internal int AdjustDefaultMask;
        internal int TargetDefaultMask;

        internal StructuredRktSelection(int twistId, int targetCell, int adjustCell) {
            TwistId = twistId;
            TargetCell = targetCell;
            AdjustCell = adjustCell;
            AdjustDefaultMask = 0;
            TargetDefaultMask = 0;
        }
    }

    internal static class StructuredRktGenerator {
        internal static CStructuredMacro Generate(CStructuredMacro source, IList<StructuredRktSelection> selections, string name) {
            if(source == null) throw new ArgumentNullException("source");
            Dictionary<int, StructuredRktSelection> map = new Dictionary<int, StructuredRktSelection>();
            if(selections != null) {
                for(int i=0;i<selections.Count;i++) {
                    StructuredRktSelection s = selections[i];
                    if(map.ContainsKey(s.TwistId))
                        throw new ArgumentException("Duplicate RKT selection for twist id " + s.TwistId);
                    map.Add(s.TwistId, s);
                }
            }

            CStructuredMacro result = new CStructuredMacro(name);
            result.NStickers = source.NStickers;
            result.Stickers = CloneArray(source.Stickers);
            result.Vectors = CloneArray(source.Vectors);
            result.Orient = CloneArray(source.Orient);
            result.RootNode = (StructuredSequenceNode)CloneReplacing(source.RootNode, source, result, map);
            return result;
        }

        static StructuredMacroNode CloneReplacing(StructuredMacroNode node, CStructuredMacro source,
            CStructuredMacro result, Dictionary<int, StructuredRktSelection> selections) {
            if(node is StructuredSequenceNode) {
                StructuredSequenceNode oldSeq = (StructuredSequenceNode)node;
                StructuredSequenceNode newSeq = new StructuredSequenceNode();
                for(int i=0;i<oldSeq.Children.Count;i++)
                    newSeq.Children.Add(CloneReplacing(oldSeq.Children[i], source, result, selections));
                return newSeq;
            }

            if(node is StructuredTwistNode) {
                StructuredTwistNode twistNode = (StructuredTwistNode)node;
                StructuredRktSelection selection;
                if(selections.TryGetValue(twistNode.TwistId, out selection))
                    return BuildReplacement(source.GetTwist(twistNode.TwistId), selection, result, false);

                int id = result.AddTwist(source.GetTwist(twistNode.TwistId));
                return new StructuredTwistNode(id);
            }

            if(node is StructuredPowerNode) {
                StructuredPowerNode powerNode = (StructuredPowerNode)node;
                StructuredRktSelection selection;
                if(selections.TryGetValue(powerNode.Twist.TwistId, out selection))
                    return BuildReplacement(source.GetTwist(powerNode.Twist.TwistId), selection, result, true);

                int id = result.AddTwist(source.GetTwist(powerNode.Twist.TwistId));
                return new StructuredPowerNode(new StructuredTwistNode(id), 2);
            }

            if(node is StructuredConjugateNode) {
                StructuredConjugateNode conj = (StructuredConjugateNode)node;
                return new StructuredConjugateNode(
                    CloneReplacing(conj.A, source, result, selections),
                    CloneReplacing(conj.B, source, result, selections));
            }

            if(node is StructuredCommutatorNode) {
                StructuredCommutatorNode comm = (StructuredCommutatorNode)node;
                return new StructuredCommutatorNode(
                    CloneReplacing(comm.A, source, result, selections),
                    CloneReplacing(comm.B, source, result, selections));
            }

            throw new ArgumentException("Unknown structured macro node.");
        }

        static StructuredMacroNode BuildReplacement(StructuredTwist sourceTwist, StructuredRktSelection selection,
            CStructuredMacro result, bool square) {
            ValidateSelection(sourceTwist, selection);

            int adjustMask = PositiveMask(selection.AdjustDefaultMask, sourceTwist.DefaultMask);
            int targetMask = PositiveMask(selection.TargetDefaultMask, sourceTwist.DefaultMask);
            int adjustId = result.AddTwist(selection.AdjustCell, sourceTwist.SignedGripAxis, selection.TargetCell, adjustMask);

            int targetCell;
            int targetDir1;
            int targetDir2;
            BuildTargetTwist(sourceTwist, selection.TargetCell, out targetCell, out targetDir1, out targetDir2);
            int targetId = result.AddTwist(targetCell, targetDir1, targetDir2, targetMask);

            StructuredMacroNode targetNode = new StructuredTwistNode(targetId);
            if(square) targetNode = new StructuredPowerNode(new StructuredTwistNode(targetId), 2);
            return new StructuredConjugateNode(new StructuredTwistNode(adjustId), targetNode);
        }

        static void ValidateSelection(StructuredTwist sourceTwist, StructuredRktSelection selection) {
            sourceTwist.Validate();
            StructuredAxis.ValidateSignedAxis(selection.TargetCell);
            StructuredAxis.ValidateSignedAxis(selection.AdjustCell);

            if(Math.Abs(selection.AdjustCell) == Math.Abs(sourceTwist.SignedGripAxis)
                || Math.Abs(selection.AdjustCell) == Math.Abs(selection.TargetCell)
                || Math.Abs(selection.TargetCell) == Math.Abs(sourceTwist.SignedGripAxis))
                throw new ArgumentException("Invalid RKT adjust/target axes for twist id " + sourceTwist.Id);

            int targetCell;
            int targetDir1;
            int targetDir2;
            BuildTargetTwist(sourceTwist, selection.TargetCell, out targetCell, out targetDir1, out targetDir2);

            if(!StructuredAxis.DistinctAbs(selection.AdjustCell, sourceTwist.SignedGripAxis, selection.TargetCell))
                throw new ArgumentException("Invalid RKT adjust twist axes for twist id " + sourceTwist.Id);
            if(!StructuredAxis.DistinctAbs(targetCell, targetDir1, targetDir2))
                throw new ArgumentException("Invalid RKT target twist axes for twist id " + sourceTwist.Id);
        }

        static void BuildTargetTwist(StructuredTwist sourceTwist, int targetCell,
            out int cell, out int dir1, out int dir2) {
            int srcCell = sourceTwist.SignedGripAxis;
            int srcDir1 = sourceTwist.FromAxis;
            int srcDir2 = sourceTwist.ToAxis;

            if(Math.Abs(targetCell) == Math.Abs(srcDir1) || Math.Abs(targetCell) == Math.Abs(srcDir2)) {
                int matched = Math.Abs(targetCell) == Math.Abs(srcDir1) ? srcDir1 : srcDir2;
                int c0 = srcCell;
                int c1 = srcDir1;
                int c2 = srcDir2;
                if(targetCell != matched) {
                    c1 = -c1;
                    c2 = -c2;
                }
                RotateCycleTo(targetCell, c0, c1, c2, out cell, out dir1, out dir2);
            } else {
                cell = targetCell;
                dir1 = srcDir1;
                dir2 = srcDir2;
            }
        }

        static void RotateCycleTo(int first, int c0, int c1, int c2, out int r0, out int r1, out int r2) {
            if(c0 == first) {
                r0 = c0; r1 = c1; r2 = c2;
            } else if(c1 == first) {
                r0 = c1; r1 = c2; r2 = c0;
            } else if(c2 == first) {
                r0 = c2; r1 = c0; r2 = c1;
            } else {
                throw new ArgumentException("Target cell is not present in RKT cycle.");
            }
        }

        static int PositiveMask(int requested, int fallback) {
            int mask = requested != 0 ? requested : fallback;
            mask = Math.Abs(mask);
            if(mask == 0) throw new ArgumentException("RKT generated mask must be non-zero.");
            return mask;
        }

        static int[] CloneArray(int[] value) {
            if(value == null) return null;
            int[] res = new int[value.Length];
            Array.Copy(value, res, value.Length);
            return res;
        }

        static double[] CloneArray(double[] value) {
            if(value == null) return null;
            double[] res = new double[value.Length];
            Array.Copy(value, res, value.Length);
            return res;
        }
    }
}

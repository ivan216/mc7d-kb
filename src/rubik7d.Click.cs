using System;
using Microsoft.DirectX;

namespace _3dedit
{
    public partial class Form1
    {
        const int CLICK_MODE_2 = 0;
        const int CLICK_MODE_2_OPP = 1;
        const int CLICK_MODE_3 = 2;

        int NClicks = 0;
        int FaceClick = 0;
        int FaceFrom = 0;
        bool ClickQual = true;

        int TwistMask;     // pressed 1-5
        bool RotateCube;   // pressed Ctrl

        void mkPickObject(EAction act, ref Vector3 pt, ref Vector3 vec, double ang)
        {
            m_runUndo = false;
            int stk = CubeView.FindSticker(ref pt, ref vec);
            if (stk < 0) {
                NClicks = 0;
                ClickQual = true;
                // Clear Twist3c state when clicking empty area
                Cube.partialTwist3c.Reset();
                switch (RecordingMacroStatus) {
                    case REC_MACRO_STICKERS:
                        if(StructuredRecordCandidate != null)
                            CancelStructuredMacroReferenceSelection();
                        else
                            RecordingMacroStatus = OldRecMacroStatus = REC_MACRO_NONE;
                        break;
                    case REC_MACRO_APPLY: RecordingMacroStatus = OldRecMacroStatus; break;
                }
                RedrawClickStatus();
                if (AltHighlight) {
                    AltHighlight = false;
                    ProcessHighLights();
                    Redraw();
                }
                return;
            }
            int ncl = NClicks;
            int fclick = FaceClick;
            int ffrom = FaceFrom;

            if (ncl == 0) {   // first click
                switch (act) {
                    case EAction.ActionCtrlClick: {
                            bool rr = Cube.RotateCubeBySticker(stk);
                            if (rr) {
                                Redraw();
                                return;
                            }
                            // Clear Twist3c state when mouse clicking starts (before validity check)
                            Cube.partialTwist3c.Reset();
                            FaceClick = Cube.GetFirstSticker(stk, ClickMode, out FaceFrom);
                            if (FaceClick != 0) {
                                NClicks = ClickMode == CLICK_MODE_3 ? 1 : 2;
                                ClickQual = true;
                                RotateCube = true;
                            } else {
                                NClicks = 0;
                                ClickQual = false;
                            }
                            RedrawClickStatus();
                            Redraw();  // Ensure UI updates for invalid clicks
                            return;
                        }
                    case EAction.ActionCtrlRightClick: {
                            bool rr = Cube.RotateCubeByStickerInverse(stk);
                            if (rr) {
                                Redraw();
                                return;
                            }
                            return;
                        }
                    case EAction.ActionShiftClick: {
                            Cube.FindOtherStickers(stk);
                            AltHighlight = true;
                            Redraw();
                            return;
                        }
                    case EAction.ActionCtrlShiftClick: {
                            Cube.FindAdjStickers(stk);
                            AltHighlight = true;
                            Redraw();
                            return;
                        }
                    default: {
                            ClickQual = true;
                            if (RecordingMacroStatus == REC_MACRO_STICKERS || RecordingMacroStatus == REC_MACRO_APPLY) {
                                if (LMacroStickers == MacroStickers.Length) {
                                    int[] mm = new int[2 * LMacroStickers];
                                    Buffer.BlockCopy(MacroStickers, 0, mm, 0, LMacroStickers * 4);
                                    MacroStickers = mm;
                                }
                                MacroStickers[LMacroStickers++] = Cube.StkMap[stk];
                                if (RecordingMacroStatus == REC_MACRO_STICKERS) {
                                    bool qx = Cube.CheckStickerSet(MacroStickers, LMacroStickers);
                                    if (qx) {
                                        LMStickers = LMacroStickers;
                                        MStickers = new int[LMStickers];
                                        Buffer.BlockCopy(MacroStickers, 0, MStickers, 0, LMStickers * 4);

                                        if(StructuredRecordCandidate != null) {
                                            StructuredRecordCandidate.NStickers = LMStickers;
                                            StructuredRecordCandidate.Stickers = new int[LMStickers];
                                            Buffer.BlockCopy(MStickers, 0, StructuredRecordCandidate.Stickers, 0, LMStickers * 4);
                                            LRevStack = 0;
                                            ShowRevStack();
                                            StructuredRecorder.Begin(StructuredRecordCandidate);
                                            StructuredRecordCandidate = null;
                                            RecordingMacroStatus = OldRecMacroStatus = REC_MACRO_NONE;
                                        } else {
                                            RecordingMacroStatus = REC_MACRO_CODE;
                                            MacroStart = Cube.LPtr;
                                        }
                                    }
                                } else {
                                    int requiredStickers = CurStructuredMacro != null ? CurStructuredMacro.NStickers : CurMacro.NStickers;
                                    if (LMacroStickers == requiredStickers) {
                                        int[] sourceStickers = CurStructuredMacro != null ? CurStructuredMacro.Stickers : CurMacro.Stickers;
                                        int[] cmap = Cube.CmpStickerSet(sourceStickers, MacroStickers, LMacroStickers);
                                        if (cmap == null) {
                                            ClickQual = false;
                                            CurMacro = null;
                                            CurStructuredMacro = null;
                                        } else {
                                            if(CurStructuredMacro != null) {
                                                ExecuteStructuredMacroMapped(CurStructuredMacro, CurStructuredOverrideMasks,
                                                    StructuredMacroReverse, cmap);
                                            } else {
                                                Cube.ApplyMacro(cmap, CurMacro.Code, CurMacro.LMacro, MacroReverse);
                                                ProcessHighLights();
                                                TestBuild();
                                                Redraw();
                                            }
                                        }
                                        CurStructuredMacro = null;
                                        CurStructuredOverrideMasks = null;
                                        RecordingMacroStatus = OldRecMacroStatus;
                                        RedrawClickStatus();
                                    }
                                }
                            } else {
                                TwistMask = 0;
                                int n = GetSize();
                                for (int i = 0; i < n; i++) {
                                    if ((S3DirectX.GetAsyncKeyState(0x31 + i) & 0x8000) != 0) TwistMask |= (1 << i);
                                }
                                if (TwistMask == 0) TwistMask = 1;

                                FaceClick = Cube.GetFirstSticker(stk, ClickMode, out FaceFrom);
                                if (FaceClick != 0) {
                                    NClicks = ClickMode == CLICK_MODE_3 ? 1 : 2;
                                    RotateCube = false;
                                    // Clear Twist3c state when mouse clicking starts
                                    Cube.partialTwist3c.Reset();
                                } else {
                                    NClicks = 0;
                                    ClickQual = false;
                                    // Clear Twist3c state even for invalid clicks
                                    Cube.partialTwist3c.Reset();
                                }
                            }
                            RedrawClickStatus();
                            Redraw();  // Ensure UI updates for invalid clicks
                            return;
                        }
                }
            } else if (ncl == 1) {  // only 3-mode
                FaceFrom = Cube.GetSecondSticker(stk, fclick);
                if (FaceFrom == 0) {
                    ClickQual = false;
                    NClicks = 0;
                } else {
                    ClickQual = true;
                    NClicks = 2;
                }
                RedrawClickStatus();
            } else {  // last click
                bool rr = false;
                int ff = Cube.GetSecondSticker(stk, fclick);
                if (ff != 0) {
                    if (RotateCube) {
                        rr = Cube.RotateCubeByStickers(fclick, ffrom, ff);
                    } else {
                        rr = Cube.Twist(fclick, ffrom, ff, TwistMask);
                        ProcessHighLights();
                        TestBuild();
                    }
                }
                if (!rr) {
                    ClickQual = false;
                    NClicks = 0;
                    RedrawClickStatus();
                    return;
                }
                ClickQual = true;
                FaceFrom = ff;
                NClicks = 0;
                RedrawClickStatus();
                Redraw();
            }
        }
    }
}

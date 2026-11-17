using System;
using System.Drawing;
using ScoreModern.RecoveredFragments;

namespace ScoreModern {
    public sealed partial class Canvas {
        internal bool Scor4DragScreen;
    }

    public sealed partial class Editor {
        PointF? scor4ScreenDragStart;
        short scor4ScreenDragX, scor4ScreenDragY;
        double scor4ScreenDragScale;
        bool scor4ScreenDragKnownFrame;

        void PanScor4Screen(string direction) {
            bool known = canvas.Scor4LiteralZoom;
            ApplyMouseCamera(() => {
                NativeDosViewport.Pan(direction,10,true);
                canvas.Scor4LiteralZoom = known;
            });
        }

        void ZoomScor4Screen(bool up) {
            float zoom = NativeDosViewport.ZoomFactor;
            if (up ? zoom >=16 : zoom <= .2f) return;
            double step = up ? (zoom < 1 ? .125 : .25) : -(zoom > 1 ? .25 : .125);
            ApplyMouseCamera(() => {
                NativeDosViewport.Center(canvas,(float)(zoom+step));
                canvas.Scor4LiteralZoom = true;
            });
        }

        void FitScor4Screen() {
            if (score.Items.Count == 0) {
                Message("There are no items to fit.");
                return;
            }
            bool metric = NativeMeasurements.Get(score) == 1;
            float height = MeasureOriginalPageHeight(false);
            // 25C9:0039–0085 uses the stored height from H and leaves the frame alone at a ratio of 1 or higher.
            float inches = metric ? (float)((double)height / 2.54f) : height;
            if (inches > 0) {
                float zoom = (float)(4.1f / (double)inches);
                if (zoom < 1) {
                    ApplyMouseCamera(() => {
                        NativeDosViewport.ZoomToScor4Fit(zoom);
                        canvas.Scor4LiteralZoom = true;
                    });
                }
            }
            Message("Page height(" + (metric ? "centimeters" : "inches") + ") = " +
                SingleFixedDecimal.Format(height,3).PadLeft(6));
        }

        void BeginScor4ScreenDrag() {
            ReturnScor4Bank();
            canvas.Scor4DragScreen = true;
            scor4ScreenDragStart = null;
            Message("Drag Screen with Mouse: ON *** (Right Button = Drag: Off)");
        }

        void EndScor4ScreenDrag() {
            canvas.Scor4DragScreen = false;
            scor4ScreenDragStart = null;
            Message("Drag: OFF ****");
            command.Focus();
        }

        bool Scor4ScreenDrag(int buttons, PointF position, PointF start, bool released) {
            if (!canvas.Scor4DragScreen) return false;
            if ((buttons & 2) != 0) {
                EndScor4ScreenDrag();
                return true;
            }
            if (buttons != 1) return true;
            Point canvasOrigin = canvas.PointToScreen(Point.Empty);
            if (!scor4ScreenDragStart.HasValue) {
                // Hold the start in screen coordinates because updating the camera can move the canvas.
                scor4ScreenDragStart = new PointF(canvasOrigin.X + start.X, canvasOrigin.Y + start.Y);
                scor4ScreenDragX = NativeDosViewport.OriginX;
                scor4ScreenDragY = NativeDosViewport.OriginY;
                scor4ScreenDragScale = (NativeNotationView.IsDosViewport ? 1.0 : 30.0 / NativeDosViewport.Divisor) / canvas.RenderScale;
                scor4ScreenDragKnownFrame = canvas.Scor4LiteralZoom;
            }
            PointF origin = scor4ScreenDragStart.Value;
            short x = unchecked((short)(scor4ScreenDragX + (int)Math.Round((canvasOrigin.X + position.X - origin.X) * scor4ScreenDragScale, MidpointRounding.AwayFromZero)));
            short y = unchecked((short)(scor4ScreenDragY + (int)Math.Round((canvasOrigin.Y + position.Y - origin.Y) * scor4ScreenDragScale, MidpointRounding.AwayFromZero)));
            if (x != NativeDosViewport.OriginX || y != NativeDosViewport.OriginY) {
                ApplyMouseCamera(() => {
                    NativeDosViewport.OriginX = x;
                    NativeDosViewport.OriginY = y;
                    canvas.Scor4LiteralZoom = scor4ScreenDragKnownFrame;
                    NativeDosViewport.DisplayChanged();
                });
            }
            if (released) scor4ScreenDragStart = null;
            return true;
        }
    }
}

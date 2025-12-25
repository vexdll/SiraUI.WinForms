using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SiraUI
{
    public class SiraUILabel : Control
    {
        #region --- ????????? ?????? ???????? ---
        private int _borderRadius = 8;
        private Color _borderColor = Color.MediumSlateBlue;
        private int _borderThickness = 2;
        private Color _fillColor = Color.FromArgb(30, 30, 30);

        private Image _leftIcon;
        private Size _leftIconSize = new Size(24, 24);

        private Image _rightIcon;
        private Size _rightIconSize = new Size(24, 24);
        #endregion

        #region --- ????? ??????? ??? ??????? ???? ---
        [Browsable(false)]
        public override Color BackColor { get; set; }
        [Browsable(false)]
        public override Image BackgroundImage { get; set; }
        [Browsable(false)]
        public override ImageLayout BackgroundImageLayout { get; set; }
        #endregion

        #region --- ??????? ??????? ???????? (SiraUI) ---
        [Category("SiraUI - Appearance")]
        public Color FillColor
        {
            get { return _fillColor; }
            set { _fillColor = value; this.Invalidate(); }
        }

        [Category("SiraUI - Appearance")]
        public int BorderRadius
        {
            get { return _borderRadius; }
            set { _borderRadius = value; this.Invalidate(); }
        }

        [Category("SiraUI - Border")]
        public Color BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; this.Invalidate(); }
        }

        [Category("SiraUI - Border")]
        public int BorderThickness
        {
            get { return _borderThickness; }
            set { _borderThickness = value; this.Invalidate(); }
        }

        [Category("SiraUI - Left Icon")]
        public Image LeftIcon
        {
            get { return _leftIcon; }
            set { _leftIcon = value; this.Invalidate(); }
        }

        [Category("SiraUI - Left Icon")]
        public Size LeftIconSize
        {
            get { return _leftIconSize; }
            set { _leftIconSize = value; this.Invalidate(); }
        }

        [Category("SiraUI - Right Icon")]
        public Image RightIcon
        {
            get { return _rightIcon; }
            set { _rightIcon = value; this.Invalidate(); }
        }

        [Category("SiraUI - Right Icon")]
        public Size RightIconSize
        {
            get { return _rightIconSize; }
            set { _rightIconSize = value; this.Invalidate(); }
        }

        [Category("SiraUI - Text")]
        [Browsable(true)]
        public override string Text
        {
            get { return base.Text; }
            set { base.Text = value; this.Invalidate(); }
        }
        #endregion

        #region --- ???????????? (Constructor) ---
        public SiraUILabel()
        {
            // === ??????? ?????: ????????? ??????? ???????? ?????? ?????? ===
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.UserPaint, true); // ??? ???? ?? ??? ???????
            this.BackColor = Color.Transparent;
            // ==========================================================

            this.DoubleBuffered = true;
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 10F);
            this.Size = new Size(150, 40);
        }
        #endregion

        #region --- ???? ????? ???????? (OnPaint) ---
        protected override void OnPaint(PaintEventArgs e)
        {
            // === ??????? ??????: ?? ?????? ?????? ???????? ??? ??????? ===
            // base.OnPaint(e); // <<--- ??? ????? ?? ??? ??????? ??? ????

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            Rectangle clientRect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);

            // 1. ??? ??????? ???????
            DrawBackgroundAndBorder(g, clientRect);

            // 2. ??? ???????? ??????
            int leftPadding = DrawLeftIcon(g);

            // 3. ??? ???????? ??????
            int rightPadding = DrawRightIcon(g);

            // 4. ??? ????
            DrawText(g, leftPadding, rightPadding);
        }
        #endregion

        #region --- ???? ????? ???????? ---
        private void DrawBackgroundAndBorder(Graphics g, Rectangle rect)
        {
            using (GraphicsPath path = GetRoundedPath(rect, _borderRadius))
            {
                // Use virtual DrawBackground so derived classes can override behavior
                DrawBackground(g, path);
            }

            if (_borderThickness > 0)
            {
                // draw border using a centered pen on an inset rectangle to avoid corner artifacts
                float halfThickness = _borderThickness / 2f;
                RectangleF borderRect = new RectangleF(rect.X + halfThickness, rect.Y + halfThickness, rect.Width - _borderThickness, rect.Height - _borderThickness);
                using (GraphicsPath borderPath = GetRoundedPath(borderRect, Math.Max(0, _borderRadius - (int)halfThickness)))
                {
                    using (Pen borderPen = new Pen(_borderColor, _borderThickness))
                    {
                        borderPen.Alignment = PenAlignment.Center;
                        borderPen.LineJoin = LineJoin.Round;
                        g.DrawPath(borderPen, borderPath);
                    }
                }
            }
        }

        // Make this protected virtual so derived classes can override background painting
        protected virtual void DrawBackground(Graphics g, GraphicsPath path)
        {
            if (_fillColor.A > 0)
            {
                using (SolidBrush fillBrush = new SolidBrush(_fillColor))
                {
                    g.FillPath(fillBrush, path);
                }
            }
        }

        private int DrawLeftIcon(Graphics g)
        {
            if (_leftIcon == null) return BorderThickness + 5;
            int y = (this.Height - _leftIconSize.Height) / 2;
            Rectangle iconRect = new Rectangle(BorderThickness + 5, y, _leftIconSize.Width, _leftIconSize.Height);
            g.DrawImage(_leftIcon, iconRect);
            return iconRect.Right + 5;
        }

        private int DrawRightIcon(Graphics g)
        {
            if (_rightIcon == null) return BorderThickness + 5;
            int y = (this.Height - _rightIconSize.Height) / 2;
            int x = this.Width - _rightIconSize.Width - (BorderThickness + 5);
            Rectangle iconRect = new Rectangle(x, y, _rightIconSize.Width, _rightIconSize.Height);
            g.DrawImage(_rightIcon, iconRect);
            return this.Width - iconRect.Left + 5;
        }

        private void DrawText(Graphics g, int leftPadding, int rightPadding)
        {
            Rectangle textRect = new Rectangle(
                leftPadding,
                0,
                this.Width - leftPadding - rightPadding,
                this.Height);

            TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak;
            TextRenderer.DrawText(g, this.Text, this.Font, textRect, this.ForeColor, flags);
        }

        protected GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            // forward to RectangleF overload for higher precision
            return GetRoundedPath(new RectangleF(rect.X, rect.Y, rect.Width, rect.Height), radius);
        }

        protected GraphicsPath GetRoundedPath(RectangleF rect, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (radius <= 0f)
            {
                path.AddRectangle(Rectangle.Round(rect));
                return path;
            }
            float r = radius * 2f;
            // Ensure radius does not exceed half of width/height
            float maxR = Math.Min(rect.Width, rect.Height);
            if (r > maxR) r = maxR;

            // build the rounded rectangle path
            path.AddArc(rect.X, rect.Y, r, r, 180, 90);
            path.AddArc(rect.X + rect.Width - r, rect.Y, r, r, 270, 90);
            path.AddArc(rect.X + rect.Width - r, rect.Y + rect.Height - r, r, r, 0, 90);
            path.AddArc(rect.X, rect.Y + rect.Height - r, r, r, 90, 90);
            path.CloseFigure();
            return path;
        }
        #endregion
    }
}

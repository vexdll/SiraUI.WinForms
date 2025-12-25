using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SiraUIPackage
{
    public class SiraUICircleButton : SiraUIButton
    {
        #region --- ????? ???????? ???????? ---
        private Image _centerIcon;
        private Size _centerIconSize = new Size(24, 24);
        private string _circleText = string.Empty;
        private bool _showText = true;

        [Category("SiraUI - Circle Button")]
        [Description("The icon displayed in the center of the button.")]
        public Image CenterIcon
        {
            get { return _centerIcon; }
            set { _centerIcon = value; this.Invalidate(); }
        }

        [Category("SiraUI - Circle Button")]
        [Description("The size of the center icon.")]
        public Size CenterIconSize
        {
            get { return _centerIconSize; }
            set { _centerIconSize = value; this.Invalidate(); }
        }

        [Category("SiraUI - Circle Button")]
        [Description("Text displayed in the center of the button.")]
        public string CircleText
        {
            get { return _circleText; }
            set { _circleText = value; this.Invalidate(); }
        }

        [Category("SiraUI - Circle Button")]
        [Description("Show or hide the text in the center of the button.")]
        public bool ShowText
        {
            get { return _showText; }
            set { _showText = value; this.Invalidate(); }
        }
        #endregion

        #region --- ?????/????? ????? ??????? ??? ??????? ???? ---
        [Browsable(false)]
        public new Image LeftIcon
        {
            get { return base.LeftIcon; }
            set { base.LeftIcon = value; }
        }

        [Browsable(false)]
        public new Image RightIcon
        {
            get { return base.RightIcon; }
            set { base.RightIcon = value; }
        }

        [Browsable(false)]
        public override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }
        #endregion

        #region --- ???? ???? BorderRadius ???????? ---
        private bool _autoCircle = true;

        [Category("SiraUI - Circle Button")]
        [Description("When enabled the control will force itself to be square and set BorderRadius = Width / 2 to make a perfect circle.")]
        public bool AutoCircle
        {
            get { return _autoCircle; }
            set { _autoCircle = value; UpdateCircleRadius(); }
        }

        [Category("SiraUI - Appearance")]
        [Description("Border radius for the control. If AutoCircle is enabled this is managed automatically.")]
        public new int BorderRadius
        {
            get { return base.BorderRadius; }
            set { base.BorderRadius = value; this.Invalidate(); }
        }
        #endregion

        #region --- ???????????? ---
        public SiraUICircleButton()
        {
            // ????? ???? ??? ?? ???? ?????? ?????? ?????? ??? ??? ???????
            this.Size = new Size(50, 50);
            UpdateCircleRadius();
        }
        #endregion

        #region --- ??????????????? ??????? ---
        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);
            if (AutoCircle)
            {
                int s = Math.Min(this.Width, this.Height);
                if (this.Width != s || this.Height != s)
                {
                    this.Size = new Size(s, s);
                }
                UpdateCircleRadius();
            }
        }

        private void UpdateCircleRadius()
        {
            if (AutoCircle)
            {
                try
                {
                    int r = Math.Max(0, Math.Min(this.Width, this.Height) / 2);
                    base.BorderRadius = r;
                }
                catch { }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var path = new GraphicsPath())
            {
                path.AddEllipse(0, 0, this.Width - 1, this.Height - 1);
                g.SetClip(path);

                using (var brush = new SolidBrush(this.FillColor))
                {
                    g.FillPath(brush, path);
                }

                if (this.BorderThickness > 0)
                {
                    using (var pen = new Pen(this.BorderColor, this.BorderThickness))
                    {
                        pen.Alignment = PenAlignment.Inset;
                        pen.LineJoin = LineJoin.Round;
                        g.DrawPath(pen, path);
                    }
                }

                if (_centerIcon != null)
                {
                    int x = (this.Width - _centerIconSize.Width) / 2;
                    int y = (this.Height - _centerIconSize.Height) / 2;
                    Rectangle iconRect = new Rectangle(x, y, _centerIconSize.Width, _centerIconSize.Height);
                    g.DrawImage(_centerIcon, iconRect);
                }

                if (_showText && !string.IsNullOrEmpty(_circleText))
                {
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        Rectangle textRect = new Rectangle(0, 0, this.Width, this.Height);
                        using (var textBrush = new SolidBrush(this.ForeColor))
                        {
                            g.DrawString(_circleText, this.Font, textBrush, textRect, sf);
                        }
                    }
                }

                base.OnPaint(e);
            }
        }
        #endregion
    }
}

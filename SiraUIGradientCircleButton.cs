using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SiraUIPackage
{
    public class SiraUIGradientCircleButton : SiraUICircleButton
    {
        private Color _color1 = Color.FromArgb(161, 0, 96);
        private Color _color2 = Color.FromArgb(59, 12, 107);
        private float _angle = 90f;
        private string _circleText = string.Empty;
        private bool _showText = true;

        [Category("SiraUI - Gradient")]
        public Color Color1 { get => _color1; set { _color1 = value; this.Invalidate(); } }

        [Category("SiraUI - Gradient")]
        public Color Color2 { get => _color2; set { _color2 = value; this.Invalidate(); } }

        [Category("SiraUI - Gradient")]
        public float Angle { get => _angle; set { _angle = value; this.Invalidate(); } }

        [Category("SiraUI - Circle Button")]
        [Description("Text displayed in the center of the button.")]
        public string CircleText { get => _circleText; set { _circleText = value; this.Invalidate(); } }

        [Category("SiraUI - Circle Button")]
        [Description("Show or hide the text in the center of the button.")]
        public bool ShowText { get => _showText; set { _showText = value; this.Invalidate(); } }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var path = GetRoundedPath(this.ClientRectangle, this.BorderRadius))
            {
                g.SetClip(path);

                using (var gradientBrush = new LinearGradientBrush(this.ClientRectangle, this.Color1, this.Color2, this.Angle))
                {
                    g.FillPath(gradientBrush, path);
                }

                if (this.BorderThickness > 0)
                {
                    using (var pen = new Pen(this.BorderColor, this.BorderThickness))
                    {
                        pen.LineJoin = LineJoin.Round;
                        g.DrawPath(pen, path);
                    }
                }
            }

            if (this.CenterIcon != null)
            {
                int x = (this.Width - this.CenterIconSize.Width) / 2;
                int y = (this.Height - this.CenterIconSize.Height) / 2;
                Rectangle iconRect = new Rectangle(x, y, this.CenterIconSize.Width, this.CenterIconSize.Height);
                g.DrawImage(this.CenterIcon, iconRect);
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
}

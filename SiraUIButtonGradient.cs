using SiraUI;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SiraUIPackage
{
    public class SiraUIGradientButton : SiraUIButton
    {
        private Color _color1 = Color.FromArgb(161, 0, 96);
        private Color _color2 = Color.FromArgb(59, 12, 107);
        private float _angle = 90f;

        [Category("SiraUI - Gradient")]
        public Color Color1 { get => _color1; set { _color1 = value; this.Invalidate(); } }

        [Category("SiraUI - Gradient")]
        public Color Color2 { get => _color2; set { _color2 = value; this.Invalidate(); } }

        [Category("SiraUI - Gradient")]
        public float Angle { get => _angle; set { _angle = value; this.Invalidate(); } }

        protected override void DrawBackground(Graphics g, GraphicsPath path)
        {
            using (var gradientBrush = new LinearGradientBrush(this.ClientRectangle, this.Color1, this.Color2, this.Angle))
            {
                g.FillPath(gradientBrush, path);
            }
        }
    }
}

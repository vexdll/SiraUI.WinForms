using SiraUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SiraUIPackage
{
    public class SiraUICustomGradientButton : SiraUIGradientButton // ??? ?? ???? ???????
    {
        private Color _color3 = Color.FromArgb(253, 29, 29);
        private Color _color4 = Color.FromArgb(252, 176, 69);

        [Category("SiraUI - Gradient")]
        [Description("The third color in the custom gradient.")]
        public Color Color3
        {
            get { return _color3; }
            set { _color3 = value; this.Invalidate(); }
        }

        [Category("SiraUI - Gradient")]
        [Description("The fourth color in the custom gradient.")]
        public Color Color4
        {
            get { return _color4; }
            set { _color4 = value; this.Invalidate(); }
        }

        protected override void DrawBackground(Graphics g, GraphicsPath path)
        {
            using (var gradientBrush = new LinearGradientBrush(this.ClientRectangle, Color.Black, Color.Black, this.Angle))
            {
                ColorBlend colorBlend = new ColorBlend();
                colorBlend.Colors = new Color[] { this.Color1, this.Color2, this.Color3, this.Color4 };
                colorBlend.Positions = new float[] { 0.0f, 0.33f, 0.66f, 1.0f };
                gradientBrush.InterpolationColors = colorBlend;
                g.FillPath(gradientBrush, path);
            }
        }
    }
}

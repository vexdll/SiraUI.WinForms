using SiraUI;
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
    public class SiraUIButton : SiraUILabel
    {
        #region --- ??????? ????????? ---
        private Timer animationTimer;
        private float animationProgress = 0f;
        private Point clickLocation;

        private bool _enableAnimation = false;
        private Color _glowColor = Color.White; // ????? ???????? ??????????
        private int _animationSpeed = 5;
        #endregion

        #region --- ????? ????????? ---
        [Category("SiraUI - Animation")]
        public bool EnableAnimation
        {
            get { return _enableAnimation; }
            set { _enableAnimation = value; }
        }

        [Category("SiraUI - Animation")]
        [Description("The base color of the ripple/glow effect. The alpha (transparency) will be managed automatically.")]
        public Color GlowColor
        {
            get { return _glowColor; }
            set { _glowColor = value; }
        }

        [Category("SiraUI - Animation")]
        [Description("Controls the speed of the click animation. Higher value = faster animation.")]
        public int AnimationSpeed
        {
            get { return _animationSpeed; }
            set { _animationSpeed = (value < 1) ? 1 : value; }
        }
        #endregion

        #region --- ???????????? ---
        public SiraUIButton()
        {
            this.Cursor = Cursors.Hand;

            if (!this.DesignMode)
            {
                animationTimer = new Timer();
                animationTimer.Interval = 15;
                animationTimer.Tick += (sender, e) => {
                    animationProgress += _animationSpeed / 100f;
                    if (animationProgress >= 1f)
                    {
                        animationProgress = 0;
                        animationTimer.Stop();
                    }
                    this.Invalidate();
                };
            }
        }
        #endregion

        #region --- ??????????????? ???????????? ??????? ---
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (!this.DesignMode && _enableAnimation && e.Button == MouseButtons.Left)
            {
                // ??? ??? ????????? ???? ??????? ??? ?????? ?? ???????
                if (animationTimer.Enabled)
                {
                    animationProgress = 0;
                }
                clickLocation = e.Location;
                animationTimer?.Start();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // --- ???? ????: ?? ????? ????? ????? ---
            // ??? ???? ?? ?? ?? ????? (??? ?? ??? ??????) ?? ???? ?? ?????? ????????
            using (var path = GetRoundedPath(this.ClientRectangle, this.BorderRadius))
            {
                e.Graphics.SetClip(path);
            }

            // ?? ?????? ???? ???? ?? ??? (???????? ??????? ?????????? ????)
            base.OnPaint(e);

            // --- ??? ???? ??? ?????? ?????? ---
            if (!this.DesignMode && _enableAnimation && animationTimer != null && animationTimer.Enabled)
            {
                // ???? ??? ????? ????? ??? ???? ?????????
                float radius = Math.Max(this.Width, this.Height) * animationProgress;

                // --- ??? ?? ?? ????? Guna ---
                // ???? ????????: ???? ???? (120) ?? ?????? ??? ?????
                int alpha = (int)(120 * (1 - animationProgress));
                if (alpha < 0) alpha = 0;

                // ????? ??? ?????? ?? ???????? ???????????
                Color dynamicGlowColor = Color.FromArgb(alpha, _glowColor);

                // --- ????? ??????? ??????? ???????? 'using' ---
                using (var glowBrush = new SolidBrush(dynamicGlowColor))
                {
                    // ????? ???? ?????? ?????? ???? ????? ????
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    // ??? ????? ??????
                    e.Graphics.FillEllipse(glowBrush,
                        clickLocation.X - radius,
                        clickLocation.Y - radius,
                        2 * radius,
                        2 * radius);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                animationTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}

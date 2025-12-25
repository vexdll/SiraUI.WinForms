using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Reflection;

namespace SiraUIPackage
{
    public class SiraUIBorderless : Component
    {
        private Form _targetForm;
        private bool _enable = true;
        private bool _resizeForm = true;
        private bool _dragForm = true;
        private int _resizeBorder = 6;
        private int _borderRadius = 12;
        private bool _hasShadow = true;
        private Color _shadowColor = Color.Black;
        private bool _dockForm = true;
        private Color _dockIndicatorColor = Color.White;
        private int _dockIndicatorTransparency = 60;

        [Category("SiraUI - Borderless")]
        public bool Enable { get => _enable; set => _enable = value; }

        [Category("SiraUI - Borderless")]
        public bool ResizeForm { get => _resizeForm; set => _resizeForm = value; }

        [Category("SiraUI - Borderless")]
        public bool DragForm { get => _dragForm; set => _dragForm = value; }

        [Category("SiraUI - Borderless")]
        public int ResizeBorder { get => _resizeBorder; set => _resizeBorder = Math.Max(1, value); }

        [Category("SiraUI - Borderless")]
        public int BorderRadius { get => _borderRadius; set { _borderRadius = Math.Max(0, value); ApplyRadius(); } }

        [Category("SiraUI - Shadow")]
        public bool HasFormShadow { get => _hasShadow; set => _hasShadow = value; }

        [Category("SiraUI - Shadow")]
        public Color ShadowColor { get => _shadowColor; set => _shadowColor = value; }

        [Category("SiraUI - Dock")]
        public bool DockForm { get => _dockForm; set => _dockForm = value; }

        [Category("SiraUI - Dock")]
        public Color DockIndicatorColor { get => _dockIndicatorColor; set => _dockIndicatorColor = value; }

        [Category("SiraUI - Dock")]
        public int DockIndicatorTransparencyValue { get => _dockIndicatorTransparency; set => _dockIndicatorTransparency = Math.Max(0, Math.Min(100, value)); }

        [Category("SiraUI - Borderless")]
        public Form TargetForm
        {
            get => _targetForm;
            set
            {
                if (_targetForm != null) Unsubscribe(_targetForm);
                _targetForm = value;
                if (_targetForm != null) Subscribe(_targetForm);
            }
        }

        private void Subscribe(Form f)
        {
            f.Load += Form_Load;
            f.Paint += Form_Paint;
            f.MouseDown += Form_MouseDown;
            f.MouseMove += Form_MouseMove;
            f.MouseUp += Form_MouseUp;
            f.FormClosing += Form_FormClosing;
            f.Resize += Form_Resize;
            f.FormBorderStyle = FormBorderStyle.None;
            try
            {
                var prop = typeof(Control).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
                prop?.SetValue(f, true, null);
            }
            catch { }
            ApplyRadius();
        }

        private void Unsubscribe(Form f)
        {
            try
            {
                f.Load -= Form_Load;
                f.Paint -= Form_Paint;
                f.MouseDown -= Form_MouseDown;
                f.MouseMove -= Form_MouseMove;
                f.MouseUp -= Form_MouseUp;
                f.FormClosing -= Form_FormClosing;
                f.Resize -= Form_Resize;
            }
            catch { }
        }

        private Point _dragOffset;
        private bool _dragging = false;

        private void Form_Load(object sender, EventArgs e)
        {
            ApplyRadius();
        }

        private void Form_Paint(object sender, PaintEventArgs e)
        {
            var f = sender as Form;
            if (_borderRadius > 0)
            {
                using (GraphicsPath path = GetRoundedPath(new Rectangle(0, 0, f.Width, f.Height), _borderRadius))
                {
                    f.Region = new Region(path);
                    using (Pen pen = new Pen(Color.FromArgb(60, f.ForeColor), 1))
                    {
                        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            }
            else
            {
                f.Region = null;
            }
        }

        private void ApplyRadius()
        {
            if (_targetForm == null) return;
            if (_borderRadius > 0)
            {
                using (GraphicsPath path = GetRoundedPath(new Rectangle(0, 0, _targetForm.Width, _targetForm.Height), _borderRadius))
                {
                    _targetForm.Region = new Region(path);
                }
            }
            else
            {
                _targetForm.Region = null;
            }
        }

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (!_enable || !_dragForm) return;
            var f = sender as Form;
            if (e.Button == MouseButtons.Left)
            {
                _dragging = true;
                _dragOffset = new Point(e.X, e.Y);
            }
        }

        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_enable) return;
            var f = sender as Form;
            if (_dragging && _dragForm && e.Button == MouseButtons.Left)
            {
                f.Location = new Point(f.Left + e.X - _dragOffset.X, f.Top + e.Y - _dragOffset.Y);
            }
            else if (_resizeForm)
            {
                var c = f.PointToClient(Cursor.Position);
                var w = f.ClientSize.Width;
                var h = f.ClientSize.Height;
                int margin = _resizeBorder;
                bool right = c.X >= w - margin;
                bool bottom = c.Y >= h - margin;
                bool left = c.X <= margin;
                bool top = c.Y <= margin;
                if (left || right || top || bottom)
                {
                    Cursor.Current = Cursors.SizeAll;
                }
                else
                {
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        private void Form_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }

        private void Form_Resize(object sender, EventArgs e)
        {
            ApplyRadius();
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            Unsubscribe((Form)sender);
        }

        public void ShowFormSafely()
        {
            if (_targetForm == null) return;
            if (Application.OpenForms.Count > 0)
            {
                var owner = Application.OpenForms[0];
                if (owner != _targetForm)
                {
                    _targetForm.Show(owner);
                    return;
                }
            }
            _targetForm.Show();
        }

        private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            float r = radius * 2f;
            float maxR = Math.Min(rect.Width, rect.Height);
            if (r > maxR) r = maxR;
            path.AddArc(rect.X, rect.Y, r, r, 180, 90);
            path.AddArc(rect.Right - r, rect.Y, r, r, 270, 90);
            path.AddArc(rect.Right - r, rect.Bottom - r, r, r, 0, 90);
            path.AddArc(rect.X, rect.Bottom - r, r, r, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}

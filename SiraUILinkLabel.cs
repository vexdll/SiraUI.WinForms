using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace SiraUIPackage
{
    public enum IconAlignment
    {
        Left,
        Right
    }

    public class SiraUILinkLabel : LinkLabel
    {
        private Color _activeLinkColor = Color.DeepSkyBlue;
        private Color _visitedLinkColor = Color.MediumPurple;
        private Color _hoverColor = Color.Orange;
        private bool _underlineOnHover = true;

        private Image _icon;
        private Size _iconSize = new Size(16, 16);
        private IconAlignment _iconAlignment = IconAlignment.Left;

        private bool _alwaysUnderline = false;
        private string _link = string.Empty;

        [Category("SiraUI - Appearance")]
        public Color ActiveLinkColor
        {
            get => _activeLinkColor;
            set { _activeLinkColor = value; base.ActiveLinkColor = value; this.Invalidate(); }
        }

        [Category("SiraUI - Appearance")]
        public Color VisitedLinkColor
        {
            get => _visitedLinkColor;
            set { _visitedLinkColor = value; base.VisitedLinkColor = value; this.Invalidate(); }
        }

        [Category("SiraUI - Appearance")]
        public Color HoverColor
        {
            get => _hoverColor;
            set { _hoverColor = value; this.Invalidate(); }
        }

        [Category("SiraUI - Appearance")]
        public bool UnderlineOnHover
        {
            get => _underlineOnHover;
            set
            {
                _underlineOnHover = value;
                this.LinkBehavior = _underlineOnHover ? LinkBehavior.HoverUnderline : LinkBehavior.NeverUnderline;
            }
        }

        [Category("SiraUI - Appearance")]
        [Description("Always show underline even when not hovered.")]
        public bool AlwaysUnderline
        {
            get => _alwaysUnderline;
            set
            {
                _alwaysUnderline = value;
                UpdateFontUnderline();
            }
        }

        [Category("SiraUI - Link")]
        [Description("The URL that will be opened when the label is clicked. If empty the label behaves like a normal LinkLabel.")]
        public string Link
        {
            get => _link;
            set => _link = value ?? string.Empty;
        }

        [Category("SiraUI - Icon")]
        public Image Icon
        {
            get => _icon;
            set { _icon = value; UpdatePadding(); this.Invalidate(); }
        }

        [Category("SiraUI - Icon")]
        public Size IconSize
        {
            get => _iconSize;
            set { _iconSize = value; UpdatePadding(); this.Invalidate(); }
        }

        [Category("SiraUI - Icon")]
        public IconAlignment IconAlignmentProperty
        {
            get => _iconAlignment;
            set { _iconAlignment = value; UpdatePadding(); this.Invalidate(); }
        }

        public SiraUILinkLabel()
        {
            base.ActiveLinkColor = _activeLinkColor;
            base.VisitedLinkColor = _visitedLinkColor;
            this.LinkColor = Color.DodgerBlue;
            this.Font = new Font("Segoe UI", 10F, FontStyle.Underline);
            this.LinkBehavior = LinkBehavior.HoverUnderline;
            UpdatePadding();
        }

        private void UpdateFontUnderline()
        {
            try
            {
                if (_alwaysUnderline)
                    this.Font = new Font(this.Font, FontStyle.Underline);
                else
                    this.Font = new Font(this.Font, FontStyle.Regular);
            }
            catch { }
        }

        private void UpdatePadding()
        {
            // adjust padding to accommodate icon
            int pad = 0;
            if (_icon != null)
            {
                pad = _iconSize.Width + 4;
            }
            if (_iconAlignment == IconAlignment.Left)
            {
                this.Padding = new Padding(pad, this.Padding.Top, this.Padding.Right, this.Padding.Bottom);
            }
            else
            {
                this.Padding = new Padding(this.Padding.Left, this.Padding.Top, pad, this.Padding.Bottom);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            this.LinkColor = _hoverColor;
            if (_underlineOnHover && !_alwaysUnderline)
                this.Font = new Font(this.Font, FontStyle.Underline);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.LinkColor = _activeLinkColor;
            if (_underlineOnHover && !_alwaysUnderline)
                this.Font = new Font(this.Font, FontStyle.Regular);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            // if Link property is set, open it
            if (!string.IsNullOrWhiteSpace(_link))
            {
                OpenLink(_link);
                this.LinkVisited = true;
                // allow customizing visited color via property
                base.VisitedLinkColor = _visitedLinkColor;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // draw icon if present
            if (_icon != null)
            {
                Rectangle imgRect;
                if (_iconAlignment == IconAlignment.Left)
                {
                    imgRect = new Rectangle(0, (this.Height - _iconSize.Height) / 2, _iconSize.Width, _iconSize.Height);
                }
                else
                {
                    imgRect = new Rectangle(this.Width - _iconSize.Width, (this.Height - _iconSize.Height) / 2, _iconSize.Width, _iconSize.Height);
                }
                try { e.Graphics.DrawImage(_icon, imgRect); } catch { }
            }

            base.OnPaint(e);
        }

        private void OpenLink(string link)
        {
            try
            {
                string url = link.Trim();
                if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) && !url.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase))
                {
                    url = "http://" + url;
                }
                var psi = new ProcessStartInfo(url) { UseShellExecute = true };
                Process.Start(psi);
            }
            catch { /* swallow exceptions to avoid breaking host app */ }
        }
    }
}

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SiraUIPackage
{
    public enum SiraAnimationType
    {
        AW_HOR_POSITIVE = 0x00000001,
        AW_HOR_NEGATIVE = 0x00000002,
        AW_VER_POSITIVE = 0x00000004,
        AW_VER_NEGATIVE = 0x00000008,
        AW_CENTER = 0x00000010,
        AW_HIDE = 0x00010000,
        AW_ACTIVATE = 0x00020000,
        AW_SLIDE = 0x00040000,
        AW_BLEND = 0x00080000
    }

    public class SiraUIAnimatedWindow : Component
    {
        private Form _targetForm;
        private SiraAnimationType _showType = SiraAnimationType.AW_BLEND;
        private SiraAnimationType _closeType = SiraAnimationType.AW_BLEND;
        private int _interval = 300;
        private bool _enable = true;

        [Category("SiraUI - Animation")]
        public bool Enable { get { return _enable; } set { _enable = value; } }

        [Category("SiraUI - Animation")]
        public int Interval { get { return _interval; } set { _interval = Math.Max(0, value); } }

        [Category("SiraUI - Animation")]
        public SiraAnimationType ShowAnimation { get { return _showType; } set { _showType = value; } }

        [Category("SiraUI - Animation")]
        public SiraAnimationType CloseAnimation { get { return _closeType; } set { _closeType = value; } }

        [Category("SiraUI - Animation")]
        public Form TargetForm
        {
            get => _targetForm;
            set
            {
                if (_targetForm != null)
                {
                    Unsubscribe(_targetForm);
                }
                _targetForm = value;
                if (_targetForm != null)
                {
                    Subscribe(_targetForm);
                }
            }
        }

        private void Subscribe(Form f)
        {
            f.Load += Target_Load;
            f.FormClosing += Target_FormClosing;
        }

        private void Unsubscribe(Form f)
        {
            f.Load -= Target_Load;
            f.FormClosing -= Target_FormClosing;
        }

        private void Target_Load(object sender, EventArgs e)
        {
            if (!_enable) return;
            try
            {
                AnimateWindow(((Form)sender).Handle, _interval, (int)(_showType | SiraAnimationType.AW_ACTIVATE));
            }
            catch
            {
                Fader.FadeIn((Form)sender, _interval);
            }
        }

        private void Target_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_enable) return;
            var f = (Form)sender;
            e.Cancel = true;
            try
            {
                bool ok = AnimateWindow(f.Handle, _interval, (int)(_closeType | SiraAnimationType.AW_HIDE));
                if (ok)
                {
                    Unsubscribe(f);
                    f.Close();
                }
                else
                {
                    Unsubscribe(f);
                    f.Close();
                }
            }
            catch
            {
                Fader.FadeOutAndClose(f, _interval);
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool AnimateWindow(IntPtr hwnd, int dwTime, int dwFlags);

        private static class Fader
        {
            public static async void FadeIn(Form f, int duration)
            {
                try
                {
                    f.Opacity = 0;
                    int steps = Math.Max(1, duration / 15);
                    double step = 1.0 / steps;
                    for (int i = 0; i < steps; i++)
                    {
                        f.Opacity = Math.Min(1.0, f.Opacity + step);
                        await System.Threading.Tasks.Task.Delay(15);
                    }
                    f.Opacity = 1;
                }
                catch { }
            }

            public static async void FadeOutAndClose(Form f, int duration)
            {
                try
                {
                    int steps = Math.Max(1, duration / 15);
                    double step = 1.0 / steps;
                    for (int i = 0; i < steps; i++)
                    {
                        f.Opacity = Math.Max(0.0, f.Opacity - step);
                        await System.Threading.Tasks.Task.Delay(15);
                    }
                    f.Opacity = 0;
                    f.BeginInvoke(new Action(() => { try { f.Close(); } catch { } }));
                }
                catch
                {
                    try { f.BeginInvoke(new Action(() => { try { f.Close(); } catch { } })); } catch { }
                }
            }
        }
    }
}

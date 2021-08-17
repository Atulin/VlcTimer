using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using VlcTimer.Models;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace VlcTimer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Process? _vlc;
        private readonly System.Timers.Timer _timer;
        private DateTime _timerStart;

        public VideoTimer VideoTimer { get; set; } = new();

        public MainWindow()
        {
            InitializeComponent();

            TimerBox.DataContext = VideoTimer;
            
            _vlc = Process
                .GetProcesses()
                .FirstOrDefault(p => p.ProcessName.Contains("VLC", StringComparison.OrdinalIgnoreCase));

            _timer = new System.Timers.Timer
            {
                Interval = 10d,
                Enabled = true,
                AutoReset = true
            };
            
            _timer.Elapsed += (_, args) =>
            {
                VideoTimer.Elapsed = (args.SignalTime - _timerStart).ToString("mm:ss.fff");
            };
        }

        private void MainWindow_OnKeyUp(object sender, KeyEventArgs e)
        {
            Action handler = e.Key switch
            {
                Key.Space => StartStopPlayback,
                Key.Escape => () =>
                {
                    _timer.Stop();
                    Close();
                },
                _ => () => { }
            };
            handler();
        }

        private bool _isPlaying;
        private void StartStopPlayback()
        {
            [DllImport("User32.dll")]
            static extern int SetForegroundWindow(IntPtr point);

            if (_vlc is null) return;
            
            var handle = _vlc.MainWindowHandle;
            var foregroundWindow = SetForegroundWindow(handle);
            
            if (foregroundWindow == 0) return;

            SendKeys.SendWait(" ");
            if (_isPlaying)
            {
                _isPlaying = false;
                _timer.Stop();
            }
            else
            {
                _isPlaying = true;
                _timerStart = DateTime.Now;
                _timer.Start();
            }

            _ = SetForegroundWindow(Process.GetCurrentProcess().MainWindowHandle);
        }

        private void MainWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }
    }
}
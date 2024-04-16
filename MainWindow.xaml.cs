using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Drawing.Color;
using Point = System.Windows.Point;

namespace lilguy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> heads = new List<string>
        {
            "^_^",
            "^°^",
            "^o^",
            "°-°",
            "+_+",
            "O.O",
            "◉‿◉",
            "◉‿◉",
            "◕‿‿◕",
            "◕ヮ◕",
            "≧︿≦"
        };
        private Thread updateThread;
        private bool running = true;
        public MainWindow()
        {
            InitializeComponent();
            this.Topmost = true;

            this.MouseDown += MouseDownHandler;

            updateThread = new Thread(new ThreadStart(UpdateLilGuy));
            updateThread.Start();
        }

        private void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if(e.RightButton == MouseButtonState.Pressed)
            {
                this.Close();
            }

            if(e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void UpdateLilGuy()
        {
            Random rand = new Random();
            Animation lilguySnoresAnimation = new Animation(new List<int> {
                0,      // Closed mouth
                1400,    // Transition mouth
                1500,    // Open mouth
                2800,    // Transition mouth
                3000    // Last Keyframe
            });
            lilguySnoresAnimation.Start();

            while (running)
            {
                // Check if the window is on top
                if (!this.IsActive)
                {
                    this.Activate(); // Bring the window to the foreground
                }

                // Check if it's time to display Pause message
                DateTime now = DateTime.Now;
                if ((now.Hour == 10 && now.Minute == 30) || (now.Hour == 16 && now.Minute == 0))
                {
                    Dispatcher.Invoke(() =>
                    {
                        lilguyTextBox.Text = heads[rand.Next(heads.Count)] + $"  -- {((rand.Next(0,2) == 0) ? "Pause" : "c[_]")} ?";
                    });

                    // Pause for 15 minutes
                    Thread.Sleep(15 * 60 * 1000);
                }
                else if((now.Hour == 12 && now.Minute == 10))
                {
                    Dispatcher.Invoke(() =>
                    {
                        lilguyTextBox.Text = heads[rand.Next(heads.Count)] + "  -- On mange ?";
                    });

                    // Pause for 50 minutes
                    Thread.Sleep(50 * 60 * 1000);
                }
                else
                {
                    // Animate the lilguy snoring
                    Dispatcher.Invoke(() =>
                    {
                        switch (lilguySnoresAnimation.GetKeyframe())
                        {
                            case 1:
                                lilguyTextBox.Text = "U.U";
                                break;
                            case 2:
                                lilguyTextBox.Text = "UoU";
                                break;
                            case 3:
                                lilguyTextBox.Text = "UOU";
                                break;
                            case 4:
                                lilguyTextBox.Text = "UoU";
                                break;
                            default:
                                lilguyTextBox.Text = "U.U";
                                break;
                        }
                    });

                    // Wait for 50ms
                    Thread.Sleep(50);
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            running = false;
            updateThread.Join();
        }
    }
}
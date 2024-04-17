using lilguy.Tools;
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

namespace lilguy;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    // Application Variables
    private Thread updateThread;
    private bool running = true;

    // Lil Guy Variables
    private FaceGenerator faceGenerator = FaceGenerator.GetInstance();

    Random rand = new Random();

    TextAnimation lilguySnoresAnimation = new TextAnimation(new Dictionary<int, string>
            {
                { 0, "U.U"},
                { 1400, "UoU"},
                { 1500, "UOU"},
                { 2800, "UoU"},
                { 3000, "U.U"},
            }, loop: true);

    TextAnimation lilguyAngryAnimation = new TextAnimation(new Dictionary<int, string>
            {
                { 0, "•`_´•"},
                { 500, "•`_´•#"},
                { 1000, "•`_´•"},
                { 1500, "•`_´•#"},
                { 2000, "•`_´•"},
                { 2500, "•`_´•"},
            });

    TextAnimation lilguyFridayAnimation = new TextAnimation(new Dictionary<int, string>
            {
                { 0,    "ᕕ(⌐■_■)ᕗ   "},
                { 1000, "  (ᕗ■_■)   "},
                { 2000, "   ᕕ(⌐■_■)ᕗ"},
                { 3000, "     (ᕗ■_■)"},
                { 4000, "     \\(⌐■_■"},
                { 5000, "     \\(⌐■_■"}
            });

    TextAnimation lilguyGoAwayAnimation = new TextAnimation(new Dictionary<int, string>
            {
                { 0,    "ᕕ( ᐛ )ᕗ      "},
                { 1000, "  (ᕗᐛ )      "},
                { 2000, "   ᕕ( ᐛ )ᕗ   "},
                { 3000, "     (ᕗᐛ )   "},
                { 5000, "      ᕕ( ᐛ )ᕗ"},
                { 6000, "Ciao -- \\( ᐛ )"},
                { 7000, "  Ciao -- \\( ᐛ"}
            });

    private Humor humor = Humor.Sleep;
    private bool isEndOfDay = false;
    private int anger = 0;

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
        if (e.RightButton == MouseButtonState.Pressed)
        {
            this.Close();
        }

        if (e.LeftButton == MouseButtonState.Pressed)
        {
            anger += 1;
            if(rand.Next(20) <= anger)
            {
                anger = 0;
                lilguyAngryAnimation.Restart();
                humor = Humor.Angry;
            }
        }

        this.DragMove();
    }

    private void UpdateLilGuy()
    {
        lilguySnoresAnimation.Start();

        while (running)
        {
            // Check if it's time to display Pause message
            DateTime now = DateTime.Now;
            if ((now.Hour == 10 && now.Minute == 30) || (now.Hour == 16 && now.Minute == 0))
            {
                Dispatcher.Invoke(() =>
                {
                    this.humor = Humor.Awake;
                    lilguyTextBox.Text = faceGenerator.GetFace() + $"  -- {((rand.Next(0, 2) == 0) ? "Pause" : "c[_]")} ?";
                });

                // Pause for 15 minutes
                Thread.Sleep(15 * 60 * 1000);
            }
            else if ((now.Hour == 12 && now.Minute == 10))
            {
                Dispatcher.Invoke(() =>
                {
                    this.humor = Humor.Awake;
                    lilguyTextBox.Text = faceGenerator.GetFace() + "  -- On mange ?";
                });

                // Pause for 50 minutes
                Thread.Sleep(50 * 60 * 1000);
            }
            else if((now.Hour == 17 && now.Minute >= 50))
            {
                Dispatcher.Invoke(() =>
                {
                    // Start animation
                    if(!isEndOfDay)
                    {
                        isEndOfDay = true;
                        if (now.DayOfWeek == DayOfWeek.Friday)
                        {
                            lilguyFridayAnimation.Start();
                        }
                        else
                        {
                            lilguyGoAwayAnimation.Start();
                        }
                    }
                    // Display animation
                    if(now.DayOfWeek == DayOfWeek.Friday)
                    {
                        lilguyTextBox.Text = lilguyFridayAnimation.GetKeyframe();
                        if(!lilguyFridayAnimation.IsRunning())
                        {
                            Close();
                        }
                    }
                    else
                    {
                        lilguyTextBox.Text = lilguyGoAwayAnimation.GetKeyframe();
                        if (!lilguyGoAwayAnimation.IsRunning())
                        {
                            Close();
                        }
                    }
                });
                
                // Wait for 50ms
                Thread.Sleep(50);
            }
            else
            {
                // Animate the lilguy snoring
                Dispatcher.Invoke(() =>
                {
                    if(humor != Humor.Angry)
                    {
                        humor = Humor.Sleep;
                        lilguyTextBox.Text = lilguySnoresAnimation.GetKeyframe();
                    }
                    else
                    {
                        lilguyTextBox.Text = lilguyAngryAnimation.GetKeyframe();
                        if(!lilguyAngryAnimation.IsRunning())
                        {
                            humor = Humor.Sleep;
                        }
                    }
                });

                // Wait for 50ms
                Thread.Sleep(50);
            }
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        running = false;
        updateThread.Join();
        base.OnClosed(e);
    }
}

public enum Humor
{
    Sleep,
    Awake,
    Angry
}
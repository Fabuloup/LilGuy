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
using Microsoft.Extensions.Configuration;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;
using System.IO;
using System.Windows.Media.Effects;
using System.Configuration;
using Microsoft.Extensions.Primitives;

namespace lilguy;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    // Application Variables
    private IChangeToken configChangeToken;
    private IConfiguration config;
    private Thread updateThread;
    private bool running = true;

    // Lil Guy Variables
    private FaceGenerator faceGenerator = FaceGenerator.GetInstance();

    Random rand = new Random();

    LilGuyAnimation lilguySnoresAnimation = new LilGuyAnimation(new Dictionary<int, LilGuyKeyFrame>
            {
                { 0,    new LilGuyKeyFrame{text = "U.U", blurRadius = 0} },
                { 1400, new LilGuyKeyFrame{text = "UoU", blurRadius = 50} },
                { 1500, new LilGuyKeyFrame{text = "UOU", blurRadius = 100} },
                { 2800, new LilGuyKeyFrame{text = "UoU", blurRadius = 50} },
                { 3000, new LilGuyKeyFrame{text = "U.U", blurRadius = 50} },
            }, loop: true);

    LilGuyAnimation lilguyAngryAnimation = new LilGuyAnimation(new Dictionary<int, LilGuyKeyFrame>
            {
                { 0,    new LilGuyKeyFrame{text = "•`_´•", blurRadius = 0 }},
                { 500,  new LilGuyKeyFrame{text = "•`_´•#", blurRadius = 300 }},
                { 1000, new LilGuyKeyFrame{text = "•`_´•", blurRadius = 0 }},
                { 1500, new LilGuyKeyFrame{text = "•`_´•#", blurRadius = 300 }},
                { 2000, new LilGuyKeyFrame{text = "•`_´•", blurRadius = 0 }},
                { 2500, new LilGuyKeyFrame{text = "•`_´•", blurRadius = 0 }},
            });

    LilGuyAnimation lilguyFridayAnimation = new LilGuyAnimation(new Dictionary<int, LilGuyKeyFrame>
            {
                { 0,    new LilGuyKeyFrame{text = "ᕕ(⌐■_■)ᕗ         ", blurRadius = 400 }},
                { 1000, new LilGuyKeyFrame{text = "  (ᕗ■_■)         ", blurRadius = 360 }},
                { 2000, new LilGuyKeyFrame{text = "   ᕕ(⌐■_■)ᕗ      ", blurRadius = 320 }},
                { 3000, new LilGuyKeyFrame{text = "     (ᕗ■_■)      ", blurRadius = 280 }},
                { 4000, new LilGuyKeyFrame{text = "      ᕕ(⌐■_■)ᕗ   ", blurRadius = 240 }},
                { 5000, new LilGuyKeyFrame{text = "        (ᕗ■_■)   ", blurRadius = 200 }},
                { 6000, new LilGuyKeyFrame{text = "         ᕕ(⌐■_■)ᕗ", blurRadius = 160 }},
                { 7000, new LilGuyKeyFrame{text = "           \\(⌐■_■", blurRadius = 120 }},
                { 8000, new LilGuyKeyFrame{text = "             \\(⌐■", blurRadius = 80 }},
                { 9000, new LilGuyKeyFrame{text = "             \\(⌐■", blurRadius = 0 }}
            });

    LilGuyAnimation lilguyGoAwayAnimation = new LilGuyAnimation(new Dictionary<int, LilGuyKeyFrame>
            {
                { 0,        new LilGuyKeyFrame { text = "ᕕ( ᐛ )ᕗ          ", blurRadius = 400 }},
                { 1000,     new LilGuyKeyFrame { text = "  (ᕗᐛ )          ", blurRadius = 360 }},
                { 2000,     new LilGuyKeyFrame { text = "   ᕕ( ᐛ )ᕗ       ", blurRadius = 320 }},
                { 3000,     new LilGuyKeyFrame { text = "     (ᕗᐛ )       ", blurRadius = 280 }},
                { 4000,     new LilGuyKeyFrame { text = "      ᕕ( ᐛ )ᕗ    ", blurRadius = 240 }},
                { 5000,     new LilGuyKeyFrame { text = "        (ᕗᐛ )    ", blurRadius = 200 }},
                { 6000,     new LilGuyKeyFrame { text = "         ᕕ( ᐛ )ᕗ ", blurRadius = 160 }},
                { 7000,     new LilGuyKeyFrame { text = "           (ᕗᐛ ) ", blurRadius = 120 }},
                { 8000,     new LilGuyKeyFrame { text = "   Ciao -- \\( ᐛ )", blurRadius = 80 }},
                { 9000,     new LilGuyKeyFrame { text = "     Ciao -- \\( ᐛ", blurRadius = 40 }},
                { 10000,    new LilGuyKeyFrame { text = "       Ciao -- \\(", blurRadius = 0 }},
                { 11000,    new LilGuyKeyFrame { text = "       Ciao -- \\(", blurRadius = 0 }}
            });

    private Humor humor = Humor.Sleep;
    private bool isEndOfDay = false;
    private int anger = 0;

    private List<Time> breaksTime = new List<Time>();
    private List<Time> mealsTime = new List<Time>();
    private Time shutdownTime = new Time();

    private double defaultBlurRadius;

    public MainWindow()
    {
        InitializeComponent();

        // Read Configuration
        var builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        config = builder.Build();
        configChangeToken = config.GetReloadToken();

        LoadConfiguration();

        // Register events
        this.Topmost = true;

        this.MouseDown += MouseDownHandler;

        // Get default values
        defaultBlurRadius = (lilguyTextBox.Effect as DropShadowEffect)!.BlurRadius;

        // Start the update thread
        updateThread = new Thread(new ThreadStart(UpdateLilGuy));
        updateThread.Start();
    }

    private void MouseDownHandler(object sender, MouseButtonEventArgs e)
    {
        if (e.RightButton == MouseButtonState.Pressed)
        {
            App.Current.Shutdown();
        }

        if (e.LeftButton == MouseButtonState.Pressed)
        {
            anger += 1;
            if (rand.Next(20) <= anger)
            {
                anger = 0;
                lilguyAngryAnimation.Restart();
                humor = Humor.Angry;
            }

            this.DragMove();
        }
    }

    private void LoadConfiguration()
    {
        if (config["shutdownTime"] == null)
        {
            shutdownTime = new Time(17, 52);
        }
        else
        {
            shutdownTime = new Time(config["shutdownTime"]!);
        }

        breaksTime.Clear();
        if (!config.GetSection("breaksTime").Exists())
        {
            breaksTime.Add(new Time(10, 30));
            breaksTime.Add(new Time(16, 00));
        }
        else
        {
            List<string> times = config.GetSection("breaksTime").Get<List<string>>() ?? new List<string>();
            foreach (string t in times)
            {
                breaksTime.Add(new Time(t));
            }
        }

        mealsTime.Clear();
        if (!config.GetSection("mealsTime").Exists())
        {
            mealsTime.Add(new Time(12, 10));
        }
        else
        {
            List<string> times = config.GetSection("mealsTime").Get<List<string>>() ?? new List<string>();
            foreach (string t in times)
            {
                mealsTime.Add(new Time(t));
            }
        }

        // Set colors
        Dispatcher.Invoke(() =>
        {
            this.lilguyTextBox.Foreground = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString(config["color"] ?? "#000"));
            (this.lilguyTextBox.Effect as DropShadowEffect)!.Color = (Color)System.Windows.Media.ColorConverter.ConvertFromString(config["halo"] ?? "#fff");
        });

        // Manage run on startup
        bool isRunOnStartupEnabled = config.GetValue<bool>("runOnStartup");

        if (isRunOnStartupEnabled && !RunOnStartup.Startup.IsInStartup())
        {
            RunOnStartup.Startup.RunOnStartup();
        }
        else if (!isRunOnStartupEnabled && RunOnStartup.Startup.IsInStartup())
        {
            RunOnStartup.Startup.RemoveFromStartup();
        }
    }

    private void UpdateLilGuy()
    {
        lilguySnoresAnimation.Start();
        DateTime realShutdownTime = shutdownTime.SetTime(DateTime.Now);

        if (DateTime.Now > realShutdownTime)
        {
            realShutdownTime = realShutdownTime.AddDays(1);
        }

        while (running)
        {
            DateTime now = DateTime.Now;

            // Check if configuration has changed
            if (configChangeToken != config.GetReloadToken())
            {
                configChangeToken = config.GetReloadToken();
                LoadConfiguration();
            }

            // Check if it's time to display Pause message
            foreach (Time t in breaksTime)
            {
                DateTime breakTimeStart = t.SetTime(now);
                DateTime breakTimeEnd = breakTimeStart.AddMinutes(15);
                if ((now >= breakTimeStart && now <= breakTimeEnd) || (now >= breakTimeStart.AddDays(-1) && now <= breakTimeEnd.AddDays(-1)))
                {
                    Dispatcher.Invoke(() =>
                    {
                        this.humor = Humor.Awake;
                        lilguyTextBox.Text = faceGenerator.GetFace() + $"  -- {((rand.Next(0, 2) == 0) ? "Pause" : "c[_]")} ?";
                        (lilguyTextBox.Effect as DropShadowEffect)!.BlurRadius = defaultBlurRadius;
                    });

                    // Pause for 15 minutes
                    Thread.Sleep(15 * 60 * 1000);
                }
            }

            // Check if it's time to display Meal message
            foreach (Time t in mealsTime)
            {
                DateTime mealTimeStart = t.SetTime(now);
                DateTime mealTimeEnd = mealTimeStart.AddMinutes(15);
                if ((now >= mealTimeStart && now <= mealTimeEnd) || (now >= mealTimeStart.AddDays(-1) && now <= mealTimeEnd.AddDays(-1)))
                {
                    Dispatcher.Invoke(() =>
                    {
                        this.humor = Humor.Awake;
                        lilguyTextBox.Text = faceGenerator.GetFace() + "  -- On mange ?";
                        (lilguyTextBox.Effect as DropShadowEffect)!.BlurRadius = defaultBlurRadius;
                    });

                    // Pause for 15 minutes
                    Thread.Sleep(15 * 60 * 1000);
                }
            }

            if (now >= realShutdownTime)
            {
                Dispatcher.Invoke(() =>
                {
                    // Start animation
                    if (!isEndOfDay)
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
                    if (now.DayOfWeek == DayOfWeek.Friday)
                    {
                        LilGuyKeyFrame keyframe = lilguyFridayAnimation.GetKeyframe();
                        lilguyTextBox.Text = keyframe.text;
                        (lilguyTextBox.Effect as DropShadowEffect)!.BlurRadius = defaultBlurRadius + keyframe.blurRadius;
                        if (!lilguyFridayAnimation.IsRunning())
                        {
                            this.Close();
                            //App.Current.Shutdown();
                        }
                    }
                    else
                    {
                        LilGuyKeyFrame keyframe = lilguyGoAwayAnimation.GetKeyframe();
                        lilguyTextBox.Text = keyframe.text;
                        (lilguyTextBox.Effect as DropShadowEffect)!.BlurRadius = defaultBlurRadius + keyframe.blurRadius;
                        if (!lilguyGoAwayAnimation.IsRunning())
                        {
                            this.Close();
                            //App.Current.Shutdown();
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
                    if (humor != Humor.Angry)
                    {
                        humor = Humor.Sleep;
                        LilGuyKeyFrame keyframe = lilguySnoresAnimation.GetKeyframe();
                        lilguyTextBox.Text = keyframe.text;
                        (lilguyTextBox.Effect as DropShadowEffect)!.BlurRadius = defaultBlurRadius + keyframe.blurRadius;
                    }
                    else
                    {
                        LilGuyKeyFrame keyframe = lilguyAngryAnimation.GetKeyframe();
                        lilguyTextBox.Text = keyframe.text;
                        (lilguyTextBox.Effect as DropShadowEffect)!.BlurRadius = defaultBlurRadius + keyframe.blurRadius;
                        if (!lilguyAngryAnimation.IsRunning())
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
        App.Current.Shutdown();
    }
}

public enum Humor
{
    Sleep,
    Awake,
    Angry
}
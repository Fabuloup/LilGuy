﻿using System;
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
    private Humor humor = Humor.Sleep;
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
                    lilguyTextBox.Text = heads[rand.Next(heads.Count)] + $"  -- {((rand.Next(0, 2) == 0) ? "Pause" : "c[_]")} ?";
                });

                // Pause for 15 minutes
                Thread.Sleep(15 * 60 * 1000);
            }
            else if ((now.Hour == 12 && now.Minute == 10))
            {
                Dispatcher.Invoke(() =>
                {
                    this.humor = Humor.Awake;
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
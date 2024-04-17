using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lilguy.Tools;

public sealed class FaceGenerator
{
    private Random rand = new Random();

    private List<string> eyes = new List<string>
    {
        "^^",
        "><",
        "°°",
        "++",
        "OO",
        "◉◉",
        "◕◕",
        "UU",
        "≧≦"
    };

    private List<string> mouth = new List<string>
    {
        "-",
        "_",
        "°",
        "o",
        ".",
        "‿",
        "‿‿",
        "ヮ",
        "︿",
        "w"
    };

    private FaceGenerator() { }

    private static FaceGenerator _instance;

    public static FaceGenerator GetInstance()
    {
        if (_instance == null)
        {
            _instance = new FaceGenerator();
        }
        return _instance;
    }

    public string GetFace()
    {
        bool sameEye = rand.NextSingle() <= 0.9;
        int firstEyeIndex = rand.Next(eyes.Count);
        int secondEyeIndex = firstEyeIndex;
        if(!sameEye)
        {
            secondEyeIndex = rand.Next(eyes.Count);
        }
        return eyes[firstEyeIndex].Substring(0, (int)double.Floor(eyes[firstEyeIndex].Length / 2))
            + mouth[rand.Next(mouth.Count)]
            + eyes[secondEyeIndex].Substring((int)double.Floor(eyes[secondEyeIndex].Length / 2));
    }
}

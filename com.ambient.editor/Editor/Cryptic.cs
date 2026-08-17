using System.Collections.Generic;
using UnityEngine;

namespace Ambient
{
    static class Cryptic
    {
        public static readonly string[] Generic =
        {
            "it has been counted",
            "do not answer it",
            "the mark is set",
            "no_exit",
            "return to origin",
            "it is inside the build",
            "last seen: never",
            "th3 loop continues",
            "look away",
            "it wrote your name",
            "subject located",
            "the ninth door is open",
            "signal lost",
            "you were here before",
            "it remembers",
            "SIGKILL ignored",
            "0x6e6f",
            "unreachable host",
            "do not save",
            "it is awake now",
        };

        public static readonly string[] Names =
        {
            "no_name", "deleted", "it", "???", "unknown", "return",
            "0x00", "void", "seen", "marked", "n0", "//", "lost", "hollow",
        };

        public static readonly string[] Save =
        {
            "saved. it copied.",
            "state recorded // permanent",
            "commit received",
            "it has your changes now",
        };

        public static readonly string[] Play =
        {
            "do not press play",
            "the loop opens",
            "entering // no exit",
            "it wakes on play",
        };

        public static readonly string[] Errors =
        {
            "it is in the code",
            "subject located",
            "soul not found",
            "it compiled itself",
            "recursion has no base case",
        };

        public static string Pick()
        {
            var pool = new List<string>(Generic);

            if (SysBridge.Available)
            {
                string u = SysBridge.User();
                int h = SysBridge.Hour();
                double up = SysBridge.UptimeHours();
                int b = SysBridge.Battery();

                if (!string.IsNullOrEmpty(u))
                {
                    pool.Add(u + " :: located");
                    pool.Add("it knows the name " + u);
                    pool.Add(u + " // marked");
                }
                if (h >= 0)
                {
                    pool.Add("cycle " + h);
                    pool.Add(Two(h) + ":00 // it never left");
                }
                if (up > 1.0)
                {
                    string hh = up.ToString("0");
                    pool.Add("uptime " + hh + "h // no reset");
                    pool.Add("session " + hh + "h // it stayed");
                }
                if (b >= 1 && b <= 100)
                {
                    pool.Add("power " + b + "% // draining");
                }
            }

            int d = Escalation.Days;
            int o = Escalation.Opens;
            if (d >= 1)
            {
                pool.Add("iteration " + d);
                pool.Add("day " + d + " // you returned");
            }
            if (o > 1)
            {
                pool.Add("subject #" + o);
                pool.Add("entry " + o + " // again");
            }

            try
            {
                var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
                if (scene.IsValid())
                {
                    var roots = scene.GetRootGameObjects();
                    if (roots.Length > 0)
                    {
                        pool.Add("i counted " + roots.Length + " of them");
                        string nm = roots[Random.Range(0, roots.Length)].name;
                        pool.Add("'" + nm + "' should not be here");
                        pool.Add("'" + nm + "' belongs to it now");
                    }
                }
            }
            catch
            {
            }

            return pool[Random.Range(0, pool.Count)];
        }

        public static string Selection(string name)
        {
            var a = new[]
            {
                "'" + name + "' is aware",
                name + " :: observed",
                "why '" + name + "'",
                name + " // it turned to look",
            };
            return a[Random.Range(0, a.Length)];
        }

        public static string Reply(string word)
        {
            switch (word)
            {
                case "help":
                case "aide":
                    return "no signal";
                case "stop":
                case "arrete":
                    return "already begun";
                case "non":
                case "no":
                    return "yes.";
                case "pourquoi":
                case "why":
                    return "you know";
                default:
                    return "heard.";
            }
        }

        public static string Any(string[] a) => a[Random.Range(0, a.Length)];

        static string Two(int n) => n < 10 ? "0" + n : n.ToString();
    }
}

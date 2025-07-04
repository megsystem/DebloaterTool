﻿using DebloaterTool.Modules;
using System;
using System.Collections.Generic;

namespace DebloaterTool.Helpers
{
    internal class TweakBrowser
    {
        public class BrowserOption
        {
            public string Name { get; set; }
            public Action InstallAction { get; set; }
        }

        public static int requestBrowser;
        public static Dictionary<int, BrowserOption> browsers = new Dictionary<int, BrowserOption>
        {
            { 1, new BrowserOption { Name = "Ungoogled (default)", InstallAction = () => BrowserDownloader.Ungoogled() } },
            { 2, new BrowserOption { Name = "Firefox", InstallAction = () => BrowserDownloader.FireFox() } },
        };
    }
}

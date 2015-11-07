﻿using System;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class CustomDateTimeFormatSettings
    {
        [DateTimeFormat("ddMMMyy")]
        public static DateTime DateTimeField { get; set; }
    }
}
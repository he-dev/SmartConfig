﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class SettingNameAttribute : Attribute
    {
        public SettingNameAttribute(string settingName)
        {
            SettingName = settingName;
        }

        public string SettingName { get; }
    }
}
using SmartConfig.Data;
using SmartConfig.DataAnnotations;
using SmartConfig.Filters;

namespace SmartConfig.Examples
{
    public class CustomSetting : BasicSetting
    {
        [SettingFilter(typeof(StringFilter))]
        public string Environment { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SmartConfig
{
    public class Color32
    {
        private const int AlphaShift = 24;
        private const int RedShift = 16;
        private const int GreenShift = 8;
        private const int BlueShift = 0;

        private int value;

        private string name;

        private bool hasAlpha = false;

        public Color32() { }

        public Color32(Color color)
        {
            value = color.ToArgb();
        }

        public Color32(int alpha, int red, int green, int blue)
        {
            value = Encode(alpha, red, green, blue);
        }

        #region Properties.

        [JsonIgnore]
        public byte A
        {
            get { return (byte)Extract(value, AlphaShift); }
        }

        [JsonIgnore]
        public byte R
        {
            get { return (byte)Extract(value, RedShift); }
        }

        [JsonIgnore]
        public byte G
        {
            get { return (byte)Extract(value, GreenShift); }
        }

        [JsonIgnore]
        public byte B
        {
            get { return (byte)Extract(value, BlueShift); }
        }

        /// <summary>
        /// Gets a string value that was used to create this color.
        /// </summary>
        [JsonIgnore]
        public string Name
        {
            get { return name; }
        }

        #endregion

        public static Color32 Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException("value", "Color value must not be 'null'.");
            }

            // Remove white spaces:
            value = Regex.Replace(value, @"\s", "");

            var color32 =
                ParseName(value)
                ?? ParseDec(value)
                ?? ParseHex(value);

            return color32;
        }

        private static Color32 ParseName(string value)
        {
            var color = Color.FromName(value);
            if (!color.IsKnownColor)
            {
                return null;
            }
            var color32 = new Color32(color)
            {
                name = value
            };
            return color32;
        }

        private static Color32 ParseDec(string value)
        {
            string colorPattern = @"\d{1,2}|[1][0-9][0-9]|[2][0-5][0-5]";
            string delimiterPattern = @"[,;:]";
            string pattern = string.Format(@"^(?:(?<A>{0}){1})?(?<R>{0}){1}(?<G>{0}){1}(?<B>{0})$", colorPattern, delimiterPattern);

            Match match = Regex.Match(value, pattern);
            if (!match.Success)
            {
                return null;
            }

            int alpha = match.Groups["A"].Success ? int.Parse(match.Groups["A"].Value) : 255;
            int red = int.Parse(match.Groups["R"].Value);
            int green = int.Parse(match.Groups["G"].Value);
            int blue = int.Parse(match.Groups["B"].Value);

            var color = Color.FromArgb(alpha, red, green, blue);
            return new Color32(color);
        }

        private static Color32 ParseHex(string value)
        {
            Match match = Regex.Match(value, @"^#(?<A>[A-F0-9]{2})?(?<R>[A-F0-9]{2})(?<G>[A-F0-9]{2})(?<B>[A-F0-9]{2})$", RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                return null;
            }

            int alpha = match.Groups["A"].Success ? int.Parse(match.Groups["A"].Value, System.Globalization.NumberStyles.HexNumber) : 255;
            int red = int.Parse(match.Groups["R"].Value, System.Globalization.NumberStyles.HexNumber);
            int green = int.Parse(match.Groups["G"].Value, System.Globalization.NumberStyles.HexNumber);
            int blue = int.Parse(match.Groups["B"].Value, System.Globalization.NumberStyles.HexNumber);

            var color = Color.FromArgb(alpha, red, green, blue);
            return new Color32(color);
        }

        public static int Encode(int alpha, int red, int green, int blue)
        {
            return alpha << AlphaShift | red << RedShift | green << GreenShift | blue << BlueShift;
        }

        public static int Extract(int color, int shift)
        {
            return (color >> shift) & 0xFF;
        }

        /// <summary>
        /// Implicitly converts <c>SmartConfig.Color32</c> to <c>System.Drawing.Color</c>.
        /// </summary>
        public static implicit operator Color(Color32 color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        /// <summary>
        /// Implicitly converts <c>System.Drawing.Color</c> to <c>SmartConfig.Color32</c>.
        /// </summary>
        public static implicit operator Color32(Color color)
        {
            return new Color32(color);
        }

        /// <summary>
        /// Supports JSON deserialize.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Color32(string value)
        {
            return Color32.Parse(value);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return ((Color)this).Equals(obj);
        }

        public override int GetHashCode()
        {
            return ((Color)this).GetHashCode();
        }

        public override string ToString()
        {
            return ToHex();
        }

        private string ToHex()
        {
            var color = new StringBuilder()
                .Append("#")
                .Append(hasAlpha ? A.ToString("X") : string.Empty)
                .Append(R.ToString("X"))
                .Append(G.ToString("X"))
                .Append(B.ToString("X"))
                .ToString();
            return color;
        }

        public static bool operator ==(Color32 x, Color32 y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return true;
            }
            if ((object)x == null || (object)y == null)
            {
                return false;
            }
            return x.A == y.A && x.R == y.R && x.G == y.G && x.B == y.B;
        }

        public static bool operator !=(Color32 x, Color32 y)
        {
            return !(x == y);
        }
    }
}
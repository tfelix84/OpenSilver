

/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO.Ports;

namespace DotNetForHtml5.Compiler
{
    /// <summary>
    /// Copied from Runtime
    /// </summary>
    public class Color
    {
        internal static string ConvertToHtmlColor(string source)
        {
            const int s_zeroChar = (int)'0';
            const int s_aLower = (int)'a';
            const int s_aUpper = (int)'A';

            string MatchColor(string colorString, out bool isKnownColor, out bool isNumericColor, out bool isScRgbColor)
            {
                string trimmedString = colorString.Trim();

                if (((trimmedString.Length == 4) ||
                    (trimmedString.Length == 5) ||
                    (trimmedString.Length == 7) ||
                    (trimmedString.Length == 9)) &&
                    (trimmedString[0] == '#'))
                {
                    isNumericColor = true;
                    isScRgbColor = false;
                    isKnownColor = false;
                }
                else if (trimmedString.StartsWith("sc#", StringComparison.Ordinal))
                {
                    isNumericColor = false;
                    isScRgbColor = true;
                    isKnownColor = false;
                }
                else
                {
                    isNumericColor = false;
                    isScRgbColor = false;
                    isKnownColor = true;
                }

                return trimmedString;
            }

            int ParseHexChar(char c)
            {
                int intChar = (int)c;

                if ((intChar >= s_zeroChar) && (intChar <= (s_zeroChar + 9)))
                {
                    return (intChar - s_zeroChar);
                }

                if ((intChar >= s_aLower) && (intChar <= (s_aLower + 5)))
                {
                    return (intChar - s_aLower + 10);
                }

                if ((intChar >= s_aUpper) && (intChar <= (s_aUpper + 5)))
                {
                    return (intChar - s_aUpper + 10);
                }
                throw new FormatException("Token is not valid.");
            }

            string ParseHexColor(string trimmedColor)
            {
                int a, r, g, b;
                a = 255;

                if (trimmedColor.Length > 7)
                {
                    a = ParseHexChar(trimmedColor[1]) * 16 + ParseHexChar(trimmedColor[2]);
                    r = ParseHexChar(trimmedColor[3]) * 16 + ParseHexChar(trimmedColor[4]);
                    g = ParseHexChar(trimmedColor[5]) * 16 + ParseHexChar(trimmedColor[6]);
                    b = ParseHexChar(trimmedColor[7]) * 16 + ParseHexChar(trimmedColor[8]);
                }
                else if (trimmedColor.Length > 5)
                {
                    r = ParseHexChar(trimmedColor[1]) * 16 + ParseHexChar(trimmedColor[2]);
                    g = ParseHexChar(trimmedColor[3]) * 16 + ParseHexChar(trimmedColor[4]);
                    b = ParseHexChar(trimmedColor[5]) * 16 + ParseHexChar(trimmedColor[6]);
                }
                else if (trimmedColor.Length > 4)
                {
                    a = ParseHexChar(trimmedColor[1]);
                    a = a + a * 16;
                    r = ParseHexChar(trimmedColor[2]);
                    r = r + r * 16;
                    g = ParseHexChar(trimmedColor[3]);
                    g = g + g * 16;
                    b = ParseHexChar(trimmedColor[4]);
                    b = b + b * 16;
                }
                else
                {
                    r = ParseHexChar(trimmedColor[1]);
                    r = r + r * 16;
                    g = ParseHexChar(trimmedColor[2]);
                    g = g + g * 16;
                    b = ParseHexChar(trimmedColor[3]);
                    b = b + b * 16;
                }

                if (a == 255) {
                    return string.Format(
                        CultureInfo.InvariantCulture,
                        "rgb({0}, {1}, {2})",
                        r, g, b
                    );
                }

                return string.Format(
                    CultureInfo.InvariantCulture,
                    "rgba({0}, {1}, {2}, {3})",
                    r, g, b, (a / 255d).ToString(CultureInfo.InvariantCulture)
                );
            }

            string ParseScRgbColor(string trimmedColor)
            {
                if (!trimmedColor.StartsWith("sc#", StringComparison.Ordinal))
                {
                    throw new FormatException("Token is not valid.");
                }

                string tokens = trimmedColor.Substring(3, trimmedColor.Length - 3);

                char[] separator = new char[2] { ',', ' ' };
                string[] split = tokens.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                if (split.Length == 3)
                {
                    return string.Format(
                        CultureInfo.InvariantCulture,
                        "rgb({0}, {1}, {2})",
                        Convert.ToSingle(split[0], CultureInfo.InvariantCulture) * 255d,
                        Convert.ToSingle(split[1], CultureInfo.InvariantCulture) * 255d,
                        Convert.ToSingle(split[2], CultureInfo.InvariantCulture) * 255d
                    );
                }
                else if (split.Length == 4)
                {
                    return string.Format(
                        CultureInfo.InvariantCulture,
                        "rgba({0}, {1}, {2}, {3})",
                        Convert.ToSingle(split[1], CultureInfo.InvariantCulture) * 255d,
                        Convert.ToSingle(split[2], CultureInfo.InvariantCulture) * 255d,
                        Convert.ToSingle(split[3], CultureInfo.InvariantCulture) * 255d,
                        Convert.ToSingle(split[0], CultureInfo.InvariantCulture)
                    );
                }

                throw new FormatException("Token is not valid.");
            }

            string ParseColor(string colorString)
            {
                string trimmedColor = MatchColor(
                    colorString, out bool isPossibleKnowColor, out bool isNumericColor, out bool isScRgbColor
                );

                //Is it a number?
                if (isNumericColor)
                {
                    return ParseHexColor(trimmedColor);
                }
                else if (isScRgbColor)
                {
                    return ParseScRgbColor(trimmedColor);
                }
                else
                {
                    Debug.Assert(isPossibleKnowColor);

                    if (Enum.TryParse(trimmedColor, true, out ColorsEnum namedColor))
                    {
                        int color = (int)namedColor;

                        return string.Format(
                            CultureInfo.InvariantCulture,
                            "rgba({1}, {2}, {3}, {0})",
                            (color >> 0x18) & 0xff,
                            (color >> 0x10) & 0xff,
                            (color >> 8) & 0xff,
                            color & 0xff
                        );
                    }
                }

                throw GetConvertException(colorString);
            }

            return ParseColor(source);
        }

        private static Exception GetConvertException(string value)
        {
            return new XamlParseException(
                $"Cannot convert '{value}' to Color."
            );
        }
    }
}
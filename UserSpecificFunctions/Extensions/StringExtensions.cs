using System;

namespace UserSpecificFunctions.Extensions
{
    /// <summary>Provides extension methods for the <see cref="String"/> type.</summary>
    public static class StringExtensions
    {
        /// <summary>Returns a Color representation of the given string.</summary>
        /// <param name="str">The color string.</param>
        /// <returns>A <see cref="Color"/> object.</returns>
        public static Color ToColor(this string str)
        {
            byte r, g, b;
            string[] color = str.Split(',');
            if (color.Length != 3)
            {
                throw new ArgumentException("The color provided was not in the correct format (r,g,b)", nameof(str));
            }

            if (!byte.TryParse(color[0], out r))
            {
                throw new ArgumentException("The color provided was not in the correct format; cannot parse 'red' value.", nameof(str));
            }

            if (!byte.TryParse(color[1], out g))
            {
                throw new ArgumentException("The color provided was not in the correct format; cannot parse 'green' value.", nameof(str));
            }

            if (!byte.TryParse(color[2], out b))
            {
                throw new ArgumentException("The color provided was not in the correct format; cannot parse 'blue' value.", nameof(str));
            }

            return new Color(r, g, b);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ASCII_Assault_App
{
    public class AsciiRenderer
    {
        private int cellWidth;
        private int cellHeight;
        private Font renderFont;
        private Dictionary<char, Color> glyphColors;

        public AsciiRenderer(int cellW = 12, int cellH = 16)
        {
            cellWidth = cellW;
            cellHeight = cellH;
            renderFont = new Font("Consolas", 12F, FontStyle.Bold);
            glyphColors = new Dictionary<char, Color>
            {
                { '#', Color.Gray },
                { '.', Color.FromArgb(60, 60, 60) },
                { '@', Color.Yellow },
                { 'D', Color.Red },
                { '~', Color.DodgerBlue },
                { '*', Color.Gold },
                { '+', Color.SaddleBrown },
                { '>', Color.White },
                { '<', Color.White }
            };
        }

        public void SetGlyphColor(char glyph, Color color)
        {
            glyphColors[glyph] = color;
        }

        public Color GetGlyphColor(char glyph)
        {
            if (glyphColors.ContainsKey(glyph))
                return glyphColors[glyph];
            return Color.LimeGreen;
        }

        public void RenderMap(Graphics g, char[,] map)
        {
            int rows = map.GetLength(0);
            int cols = map.GetLength(1);

            g.Clear(Color.Black);

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    char glyph = map[row, col];
                    if (glyph == ' ')
                        continue;

                    Color color = GetGlyphColor(glyph);
                    using (Brush brush = new SolidBrush(color))
                    {
                        float x = col * cellWidth;
                        float y = row * cellHeight;
                        g.DrawString(glyph.ToString(), renderFont, brush, x, y);
                    }
                }
            }
        }

        public static char[,] ParseMapString(string raw)
        {
            string[] lines = raw.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length == 0)
                return new char[0, 0];

            int maxCols = 0;
            foreach (string line in lines)
            {
                if (line.Length > maxCols)
                    maxCols = line.Length;
            }

            char[,] map = new char[lines.Length, maxCols];
            for (int r = 0; r < lines.Length; r++)
            {
                for (int c = 0; c < maxCols; c++)
                {
                    map[r, c] = c < lines[r].Length ? lines[r][c] : ' ';
                }
            }
            return map;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Art
{
    public static class Methods
    {
        public static int Clamp(this int val, int min, int max)
        {
            return Math.Min(Math.Max(val, min), max);
        }

        public static int FloorDivide(this int numerator, int denominator)
        {
            return (int)(Math.Floor(numerator / (double)denominator));
        }

        public static int FloorDivide(this int numerator, double denominator)
        {
            return (int)(Math.Floor(numerator / denominator));
        }

        public static int FloorDivide(this double numerator, int denominator)
        {
            return (int)(Math.Floor(numerator / denominator));
        }

        public static int FloorDivide(this double numerator, double denominator)
        {
            return (int)(Math.Floor(numerator / denominator));
        }

        public static double Divide(this int numerator, int denominator)
        {
            return (double)numerator / denominator;
        }

        public static int FloorTimes(this int num, double factor)
        {
            return (int)(Math.Floor(num * factor));
        }

        public static List<Coord> EachPoint(this Coord range)
        {
            var result = new List<Coord>();
            for (int row = 0; row < range.row; row++)
            {
                for (int col = 0; col < range.col; col++)
                {
                    result.Add(new Coord(row, col));
                }
            }
            return result;
        }

        public static List<List<T>> InitializeRect<T>(this List<List<T>> rect, Coord size) where T : new()
        {
            var result = new List<List<T>>();
            for (int row = 0; row < size.row; row++)
            {
                var r = new List<T>();
                for (int col = 0; col < size.col; col++)
                {
                    r.Add(new T());
                }
                result.Add(r);
            }
            return result;
        }

        public static Grid<T> ToGrid<T>(this List<T> list, Coord size) where T : new()
        {
            var grid = new Grid<T>(size);
            foreach (var C in grid.EachCell())
            {
                C.value = list[(C.loc.row * size.col) + C.loc.col];
            }
            return grid;
        }

        public static List<ArtColor> ColorSplitter(ArtColor c1, ArtColor c2, int numDivisions)
        {
            var result = new List<ArtColor>();
            result.Add(c1);
            for (int y = 0; y < numDivisions - 1; y++)
            {
                var blendPercent = 100.FloorDivide(numDivisions) * (y + 1);
                result.Add(c1.Blend(c2, blendPercent));
            }
            return result;
        }

        public static List<ArtColor> PaletteExploder(List<ArtColor> input, int numDivisions, bool circular = false)
        {
            var result = new List<ArtColor>();
            for (int x = 1; x < input.Count; x++)
            {
                var color2 = input[x];
                var color1 = input[x - 1];
                result.AddRange(ColorSplitter(color1, color2, numDivisions));
            }
            if (!circular)
            {
                result.Add(input.Last());
            }
            else
            {
                result.AddRange(ColorSplitter(input.Last(), input.First(), numDivisions));
            }
            return result;
        }
    }
}

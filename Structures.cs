﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Art
{
    public class Coord
    {
        public int row, col;

        public Coord(int _row, int _col)
        {
            row = _row;
            col = _col;
        }

        public Coord(int _square)
        {
            row = _square;
            col = _square;
        }

        public bool InRange(Coord topLeft, Coord bottomRight)
        {
            if (topLeft.row <= row && bottomRight.row >= row
                && topLeft.col <= col && bottomRight.col >= col)
            {
                return true;
            }
            return false;
        }
        public bool Clamp(Coord lowerBound, Coord upperBound)
        {
            var result = false;
            if (row < lowerBound.row)
            {
                result = true;
                row = lowerBound.row;
            }
            if (col < lowerBound.col)
            {
                result = true;
                col = lowerBound.col;
            }
            if (row > upperBound.row)
            {
                result = true;
                row = upperBound.row;
            }
            if (col > upperBound.col)
            {
                result = true;
                col = upperBound.col;
            }
            return result;
        }
        public Coord Times(int factor)
        {
            return new Coord(row * factor, col * factor);
        }
        public Coord Plus(Coord offset)
        {
            return new Coord(row + offset.row, col + offset.col);
        }
        public Coord FloorDivide(int factor)
        {
            return new Coord(row.FloorDivide(factor), col.FloorDivide(factor));
        }
    }

    public class ArtColor
    {
        private int _red, _green, _blue, colorRange, _opacity;
        public int red { get => _red; set { _red = value.Clamp(0, colorRange); } }
        public int green { get => _green; set { _green = value.Clamp(0, colorRange); } }
        public int blue { get => _blue; set { _blue = value.Clamp(0, colorRange); } }
        public int opacity { get => _opacity; set { _opacity = value.Clamp(0, 100); } }

        public ArtColor(int r, int g, int b, int o = 100, int _colorRange = 256)
        {
            colorRange = _colorRange - 1;
            red = r;
            green = g;
            blue = b;
            opacity = o;
        }

        public ArtColor()
        {
            colorRange = 255;
            red = colorRange;
            green = colorRange;
            blue = colorRange;
        }

        public System.Drawing.Color Render()
        {
            return System.Drawing.Color.FromArgb(red, green, blue);
        }

        public static readonly ArtColor Black = new ArtColor(0, 0, 0);
        public static readonly ArtColor White = new ArtColor(255, 255, 255);
        public static readonly ArtColor Red = new ArtColor(255, 0, 0);
        public static readonly ArtColor Green = new ArtColor(0, 255, 0);
        public static readonly ArtColor Blue = new ArtColor(0, 0, 255);

        public ArtColor Blend(ArtColor mixer, int mixerPercent = 50)
        {
            if (mixerPercent <= 0)
            {
                return this;
            } else if (mixerPercent >= 100)
            {
                return mixer;
            }
            var thisPercent = 100 - mixerPercent;
            var mixRed = Methods.FloorDivide(((red * (thisPercent * (opacity.Divide(100))))) + (mixer.red * (mixerPercent * (mixer.opacity.Divide(100)))), 100);
            var mixGreen = Methods.FloorDivide(((green * (thisPercent * (opacity.Divide(100))))) + (mixer.green * (mixerPercent * (mixer.opacity.Divide(100)))), 100);
            var mixBlue = Methods.FloorDivide(((blue * (thisPercent * (opacity.Divide(100))))) + (mixer.blue * (mixerPercent * (mixer.opacity.Divide(100)))), 100);
            return new ArtColor(mixRed, mixGreen, mixBlue);
        }
    }

    public class Cell<T> where T : new()
    {
        private Coord _loc;
        public Coord loc { get { return _loc; } set { } }
        public T value;
        public Cell(Coord c)
        {
            _loc = c;
            value = new T();
        }
    }

    public class Layerbook
    {
        private List<ArtLayer> Layers;
        public readonly Coord Size;
        public Layerbook(Coord _size)
        {
            Layers = new List<ArtLayer>();
            Size = _size;
            Layers.Add(new ArtLayer(Size));
        }

        public void Add(ArtLayer layer, int opacity = -1)
        {
            if (opacity >= 0 && opacity <= 100)
            {
                layer.opacity = opacity;
            }
            Layers.Add(layer);
        }

        public Grid<ArtColor> Render()
        {
            List<ArtLayer> temp = new List<ArtLayer>();
            foreach (var layer in Layers)
            {
                temp.Add(layer);
            }
            temp.Reverse();
            var result = temp.Last();
            temp.Remove(temp.Last());
            while (temp.Count > 0)
            {
                var next = temp.Last();
                temp.Remove(temp.Last());
                result = Mix(result, next);
            }
            return result.grid;
        }

        private ArtLayer Mix(ArtLayer baseLayer, ArtLayer overlay)
        {
            var result = new ArtLayer(baseLayer.size);
            var coords = baseLayer.grid.EachPoint();
            foreach (var coord in coords)
            {
                var basePoint = baseLayer.grid.GetCell(coord);
                var overlayPoint = overlay.grid.GetCell(coord);
                result.grid.SetCell(coord, basePoint.Blend(overlayPoint, overlay.opacity));
            }

            return result;
        }
    }

    public class Layer<T> where T : new()
    {
        public Grid<T> grid;
        public readonly Coord size;
        public Layer(Coord _size)
        {
            size = _size;
            grid = new Grid<T>(size);
        }
    }

    public class ArtLayer : Layer<ArtColor>
    {
        public int opacity;
        public ArtLayer(Coord _size, int _opacity = 100) : base(_size)
        {
            opacity = _opacity;
        }
    }


    public class Grid<T> where T : new()
    {
        private Coord _gridSize;
        public Coord gridSize { get { return _gridSize; } set { } }
        List<List<Cell<T>>> grid;
        public Grid(Coord param_gridSize)
        {
            _gridSize = param_gridSize;
            grid = new List<List<Cell<T>>>();
            for (int row = 0; row < gridSize.row; row++)
            {
                var gridRow = new List<Cell<T>>();
                for (int col = 0; col < gridSize.col; col++)
                {
                    gridRow.Add(new Cell<T>(new Coord(row, col)));
                }
                grid.Add(gridRow);
            }
        }

        public bool SetCell(Coord loc, T value)
        {
            if (InFrame(loc))
            {
                grid[loc.row][loc.col].value = value;
                return true;
            }
            return false;
        }

        public T GetCell(Coord loc)
        {
            if (InFrame(loc))
            {
                return grid[loc.row][loc.col].value;
            }
            throw new Exception("Coordinates out of range");
        }

        public bool InFrame(Coord loc)
        {
            return loc.InRange(new Coord(0,0), gridSize.Plus(new Coord(-1,-1)));
        }

        public Grid<T> Crop(Coord topLeft, Coord size)
        {
            var result = new Grid<T>(size);
            foreach (var C in Methods.EachPoint(size))
            {
                var offset = new Coord(topLeft.row + C.row, topLeft.col + C.col);
                if (!InFrame(offset))
                {
                    result.SetCell(C, new T());
                }
                else
                {
                    result.SetCell(C, GetCell(offset));
                }
            }     
            return result;
        }

        public List<Cell<T>> EachCell()
        {
            return grid.SelectMany(x => x).ToList();
        }

        public List<Coord> EachPoint()
        {
            return EachCell().Select(x => x.loc).ToList();
        }

        // to do: "blend" action that's like stamp, but calls a method on T for resolving the interaction
        //      also: revisit this method when adding opacity
        public bool Stamp(Coord topLeft, Grid<T> grid)
        {
            foreach (var C in grid.EachPoint())
            {
                Coord offset = topLeft.Plus(C);
                if (InFrame(offset))
                {
                    SetCell(offset, grid.GetCell(C));
                }
            }
            return true;
        }
    }

    // Should modify a layer
    public interface Filter
    {
        Layer<ArtColor> Filter(Layer<ArtColor> input);
    }

    // should generate layers at a high level
    public interface Generator
    {
        Layer<ArtColor> Generate(CommonInfo info);
    }

    // should generate colors at a low level. Separate from Generator to promote reuse
    public interface Fill
    {
        ArtColor Fill(CommonInfo info);
    }

    // Should handle an entire image's creation with no parameters
    public interface Workflow
    {
        System.Drawing.Bitmap Create();
    }

    // this should be populated with any properties that are useful in a lot of different workflows
    // it will be up to each connection between classes to ensure that the necessary data is being passed in,
    //      as not all info is relevant to every workflow
    public class CommonInfo
    {
        public Coord Size;
        public int PatternScale;
        public List<ArtColor> Palette;
        public int Phase; // Phase should have meaning at the Generator-level
        public Fill Fill;
        public Coord Position;

        public CommonInfo()
        {
            Palette = new List<ArtColor>();
        }
    }

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
    }
}
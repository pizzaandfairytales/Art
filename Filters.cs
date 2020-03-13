using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Art
{
    public class FishEye : Filter
    {
        public Layer<ArtColor> Filter(Layer<ArtColor> input)
        {
            var result = new Layer<ArtColor>(input.size);
            var buffer = new Grid<ArtColor>(input.size);
            foreach (var C in input.size.EachPoint())
            {
                var fishbowl = C.FloorDivide(20).Plus(C);
                buffer.SetCell(C, input.grid.GetCell(fishbowl));
            }
            result.grid = buffer;
            return result;
        }
    }

    public class Blur : Filter
    {
        public Layer<ArtColor> Filter(Layer<ArtColor> input)
        {
            var result = new Layer<ArtColor>(input.size);
            var buffer = new Grid<ArtColor>(input.size);
            foreach (var C in input.size.EachPoint())
            {
                var neighbors = new List<Coord>();
                if (C.row > 0)
                {
                    neighbors.Add(C.Plus(new Coord(-1, 0)));
                }
                if (C.row < input.size.row - 1)
                {
                    neighbors.Add(C.Plus(new Coord(1, 0)));
                }
                if (C.col > 0)
                {
                    neighbors.Add(C.Plus(new Coord(0, -1)));
                }
                if (C.col < input.size.col - 1)
                {
                    neighbors.Add(C.Plus(new Coord(0, 1)));
                }

                List<ArtColor> neighborColors = new List<ArtColor>();
                foreach (var n in neighbors)
                {
                    neighborColors.Add(input.grid.GetCell(n));
                }
                int red = neighborColors.Select(z => z.red).Cast<int>().Sum() / neighborColors.Count();
                int green = neighborColors.Select(z => z.green).Cast<int>().Sum() / neighborColors.Count();
                int blue = neighborColors.Select(z => z.blue).Sum() / neighborColors.Count();
                buffer.SetCell(C, new ArtColor(red, green, blue));
            }
            result.grid = buffer;
            return result;
        }
    }
}

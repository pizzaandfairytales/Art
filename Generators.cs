using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Art
{
    public class Hilbert : Generator
    {
        CommonInfo Info;
        public ArtLayer Generate(CommonInfo _Info)
        {
            Info = _Info;
            var result = new ArtLayer(Info.Size);
            var curve = HilbertCurve.GenerateHilbertCurve(HilbertIterationsRequired());
            // inflate curve to gridSize (this could be a grid method eventually)
            var temp = new Grid<bool>(curve.gridSize.Times(Info.PatternScale));
            foreach (Cell<bool> C in curve.EachCell())
            {
                bool value = curve.GetCell(C.loc);
                foreach (var C2 in Methods.EachPoint(new Coord(Info.PatternScale)))
                {
                    temp.SetCell(C.loc.Times(Info.PatternScale).Plus(C2), value);
                }
            }
            curve = temp;

            curve = curve.Crop(new Coord(0, 0), Info.Size);

            foreach (var cell in curve.EachCell())
            {
                if (cell.value == true)
                {
                    Info.Phase = 0;
                } else
                {
                    Info.Phase = 1;
                }
                Info.Position = cell.loc;
                ArtColor color = Info.Fill.Fill(Info);
                result.grid.SetCell(cell.loc, color);
            }
            return result;
        }

        private int HilbertIterationsRequired()
        {
            // find number of recursions required
            int iterations = 1;
            int hilbertsize = 1;

            int maxDimension = (int)Math.Floor(Math.Max(Info.Size.row, Info.Size.col) / (double)Info.PatternScale);
            while (hilbertsize < maxDimension)
            {
                hilbertsize *= 4;
                iterations++;
            }
            return iterations;
        }

        // curve is not correct
        private static class HilbertCurve
        {
            public enum HilbertCurveComponent
            {
                A, B, C, D
            }

            private static Grid<bool> ResolveComponent(HilbertCurveComponent val)
            {

                var result = new Grid<bool>(new Coord(3, 3));
                // corners are always true
                result.SetCell(new Coord(0, 0), true); // top left
                result.SetCell(new Coord(0, 2), true); // top right
                result.SetCell(new Coord(2, 0), true); // bottom left
                result.SetCell(new Coord(2, 2), true); // bottom right
                if (val == HilbertCurveComponent.A) // bottom missing
                {
                    result.SetCell(new Coord(0, 1), true); // top middle
                    result.SetCell(new Coord(1, 0), true); // left middle
                    result.SetCell(new Coord(1, 2), true); // right middle
                }
                else if (val == HilbertCurveComponent.B) // right missing
                {
                    result.SetCell(new Coord(0, 1), true); // top middle
                    result.SetCell(new Coord(1, 0), true); // left middle
                    result.SetCell(new Coord(2, 1), true); // bottom middle
                }
                else if (val == HilbertCurveComponent.C) // top missing
                {
                    result.SetCell(new Coord(1, 0), true); // left middle
                    result.SetCell(new Coord(1, 2), true); // right middle
                    result.SetCell(new Coord(2, 1), true); // bottom middle
                }
                else if (val == HilbertCurveComponent.D) // left missing
                {
                    result.SetCell(new Coord(0, 1), true); // top middle
                    result.SetCell(new Coord(1, 2), true); // right middle
                    result.SetCell(new Coord(2, 1), true); // bottom middle
                }
                return result;
            }

            private static Grid<HilbertCurveComponent> IterateCurveComponent(HilbertCurveComponent component)
            {
                List<HilbertCurveComponent> list;
                switch (component)
                {
                    case HilbertCurveComponent.A:
                        list = new List<HilbertCurveComponent> { HilbertCurveComponent.A, HilbertCurveComponent.A,
                                                             HilbertCurveComponent.D, HilbertCurveComponent.B };
                        break;
                    case HilbertCurveComponent.B:
                        list = new List<HilbertCurveComponent> { HilbertCurveComponent.B, HilbertCurveComponent.C,
                                                             HilbertCurveComponent.B, HilbertCurveComponent.A };
                        break;
                    case HilbertCurveComponent.C:
                        list = new List<HilbertCurveComponent> { HilbertCurveComponent.D, HilbertCurveComponent.B,
                                                             HilbertCurveComponent.C, HilbertCurveComponent.C };
                        break;
                    case HilbertCurveComponent.D:
                        list = new List<HilbertCurveComponent> { HilbertCurveComponent.C, HilbertCurveComponent.D,
                                                             HilbertCurveComponent.A, HilbertCurveComponent.D };
                        break;
                    default:
                        throw new Exception("Hilbert Curve component " + component.ToString() + " not handled in IterateCurveComponent()");
                }
                return list.ToGrid(new Coord(2));
            }

            private static Grid<HilbertCurveComponent> IterateHilbertCurve(Grid<HilbertCurveComponent> start)
            {
                var finish = new Grid<HilbertCurveComponent>(start.gridSize.Times(2));
                foreach (var C in start.EachCell())
                {
                    finish.Stamp(C.loc.Times(2), IterateCurveComponent(C.value));
                }
                return finish;
            }

            private static Grid<HilbertCurveComponent> initialState(HilbertCurveComponent orientation)
            {
                var result = new Grid<HilbertCurveComponent>(new Coord(1, 1));
                result.SetCell(new Coord(0, 0), orientation);
                return result;
            }

            private static Grid<bool> ResolveCurve(Grid<HilbertCurveComponent> curve)
            {
                // 1. render components to bools
                var result = new Grid<bool>(curve.gridSize.Times(4));
                foreach (var C in curve.EachCell())
                {
                    result.Stamp(C.loc.Times(4), ResolveComponent(C.value));
                }

                // 2. connect the blocks
                //Coord potentialConnection;
                //foreach (var C in result.EachCell())
                //{
                //    if (C.value == true && CountNeighbors(result, C.loc) == 1)
                //    {
                //        if (C.loc.row > 1) // check left connection
                //        {
                //            potentialConnection = C.loc.Plus(new Coord(-2, 0));
                //            if (result.InFrame(potentialConnection) && result.GetCell(potentialConnection) == true && CountNeighbors(result, potentialConnection) == 1 && curve.GetCell(C.loc.FloorDivide(4)) != HilbertCurveComponent.D)
                //            {
                //                result.SetCell(C.loc.Plus(new Coord(-1, 0)), true);
                //            }
                //        }
                //        if (C.loc.row < result.gridSize.row - 2) // check right connection
                //        {
                //            potentialConnection = C.loc.Plus(new Coord(2, 0));
                //            if (result.InFrame(potentialConnection) && result.GetCell(potentialConnection) == true && CountNeighbors(result, potentialConnection) == 1 && curve.GetCell(C.loc.FloorDivide(4)) != HilbertCurveComponent.B)
                //            {
                //                result.SetCell(C.loc.Plus(new Coord(1, 0)), true);
                //            }
                //        }
                //        if (C.loc.col > 1) // check up connection
                //        {
                //            potentialConnection = C.loc.Plus(new Coord(0, -2));
                //            if (result.InFrame(potentialConnection) && result.GetCell(potentialConnection) == true && CountNeighbors(result, potentialConnection) == 1 && curve.GetCell(C.loc.FloorDivide(4)) != HilbertCurveComponent.C)
                //            {
                //                result.SetCell(C.loc.Plus(new Coord(0, -1)), true);
                //            }
                //        }
                //        if (C.loc.col < result.gridSize.col - 2) // check down connection
                //        {
                //            potentialConnection = C.loc.Plus(new Coord(0, 2));
                //            if (result.InFrame(potentialConnection) && result.GetCell(potentialConnection) == true && CountNeighbors(result, potentialConnection) == 1 && curve.GetCell(C.loc.FloorDivide(4)) != HilbertCurveComponent.A)
                //            {
                //                result.SetCell(C.loc.Plus(new Coord(0, 1)), true);
                //            }
                //        }
                //    }
                //}
                return result;
            }

            private static int CountNeighbors(Grid<bool> grid, Coord loc)
            {
                int count = 0;
                if (loc.row > 0)
                {
                    count += grid.GetCell(loc.Plus(new Coord(-1, 0))) == true ? 1 : 0;
                }
                if (loc.row < grid.gridSize.row - 1)
                {
                    count += grid.GetCell(loc.Plus(new Coord(1, 0))) == true ? 1 : 0;
                }
                if (loc.col > 0)
                {
                    count += grid.GetCell(loc.Plus(new Coord(0, -1))) == true ? 1 : 0;
                }
                if (loc.col < grid.gridSize.col - 1)
                {
                    count += grid.GetCell(loc.Plus(new Coord(0, 1))) == true ? 1 : 0;
                }
                return count;
            }

            public static Grid<bool> GenerateHilbertCurve(int size, HilbertCurveComponent orientation = HilbertCurveComponent.A)
            {
                var result = initialState(orientation);
                for (int x = 0; x < size; x++)
                {
                    result = IterateHilbertCurve(result);
                }
                return ResolveCurve(result);
            }
        }
    }

    public class Flat : Generator
    {
        public ArtLayer Generate(CommonInfo info)
        {
            var result = new ArtLayer(info.Size);
            foreach (var point in info.Size.EachPoint())
            {
                var fillInfo = new CommonInfo();
                fillInfo.Position = point;
                result.grid.SetCell(point, info.Fill.Fill(info));
            }
            return result;
        }
    }
}

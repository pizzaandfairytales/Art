using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Art
{
    public class TestWorkflow : Workflow
    {
        public void Create(string filename)
        {
            var size = new Coord(512, 512);
            Layerbook layers = new Layerbook(size);
            var info = new CommonInfo();
            info.Fill = new CheckerColor();
            info.Size = size;
            info.Palette.Add(ArtColor.Red);
            info.Palette.Add(ArtColor.White);
            info.PatternScale = 32;
            layers.Add(new Hilbert().Generate(info));
            layers.saveBMP(filename);
        }
    }
}

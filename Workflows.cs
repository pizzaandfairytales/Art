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
            var size = new Coord(1024, 1024);
            Layerbook layers = new Layerbook(size);
            var info = new CommonInfo();
            info.Fill = new CheckerColor();
            info.Size = size;
            info.Palette.Add(ArtColor.Cyan);
            info.Palette.Add(ArtColor.Yellow);
            info.Palette.Add(ArtColor.Magenta);
            info.Palette.Add(ArtColor.Black);
            info.Palette = Methods.PaletteExploder(info.Palette, 8, true);
            info.PatternScale = 8;
            layers.Add(new Flat().Generate(info));
            layers.saveBMP(filename);
        }

        
    }
}

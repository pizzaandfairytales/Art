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
            var size = new Coord(2048, 2048);
            Layerbook layers = new Layerbook(size);
            var info = new CommonInfo();
            info.Fill = new CheckerColor();
            info.Size = size;
            info.Palette.Add(ArtColor.Cyan);
            info.Palette.Add(ArtColor.Yellow);
            info.Palette.Add(ArtColor.Magenta);
            info.Palette.Add(ArtColor.White);
            info.Palette = Methods.PaletteExploder(info.Palette, 8, true);
            info.PatternScale = 8;
            var layer = new Flat().Generate(info);
            layers.Add(new FishEye().Filter(layer));
            layers.saveBMP(filename);
        }

        
    }
}

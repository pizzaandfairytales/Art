using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Art
{
    // ideas:
    //      - semantic color (object association, image-processing tags like edge, and image composition tags (background element, focus)
    //          - "smart filters" or semantic filters can then perform localized, specialized, or complex image processing

    class Main
    {
        Bitmap img;
        Layerbook layerbook;
        protected Coord size;
        protected int colorDepth, gridSize;
        System.Drawing.Imaging.PixelFormat pixelFormat;
        
        public Main()
        {
            new TestWorkflow().Create("test");
        }

        private Tuple<int, int, int> spinColor(int red, int green, int blue)
        {
            int temp = red;
            red = green;
            green = blue;
            blue = temp;

            return new Tuple<int, int, int>(red, green, blue);
        }

        private ArtColor funColor2(Coord loc, bool isHilbert)
        {
            int red = colorDepth.FloorDivide(size.row) * loc.row;
            int green = (loc.row % gridSize * 8);
            int blue = (loc.col % gridSize * 8);



            if (green % gridSize == 24)
            {
                //green = (green % colorDepth) + (colorDepth - (green % colorDepth) / 2);
            }
            if (blue % gridSize == 24)
            {
                //blue = (blue % colorDepth) + (colorDepth - (blue % colorDepth) / 2);
            }

            if (isHilbert)
            {
                var T = spinColor(red, green, blue);
                //T = spinColor(T.Item1, T.Item2, T.Item3);
                red = T.Item1;
                green = T.Item2;
                blue = T.Item3;
            }
            red %= 256;
            green %= 256;
            blue %= 256;
            green = green.Clamp(100, 200);
            return new ArtColor(red % 256, green % 256, blue % 256);
        }

        private ArtColor funColor(int row, int col)
        {
            int red = colorDepth.FloorDivide(size.col) * row;
            int green = (row * 8);
            int blue = (col * 8);

            

            if (green % 32 == 24)
            {
                green = (green % colorDepth) + (colorDepth - (green % colorDepth) / 2);
            }
            if (blue % 32 == 24)
            {
                blue = (blue % colorDepth) + (colorDepth - (blue % colorDepth) / 2);
            }

            if (testCoord(row, col))
            {
                var T = spinColor(red, green, blue);
                //T = spinColor(T.Item1, T.Item2, T.Item3);
                red = T.Item1;
                green = T.Item2;
                blue = T.Item3;
            }

            return new ArtColor(red, green, blue);
        }

        

        private void CYMFill()
        {
            Color cyan = Color.FromArgb(0, 255, 255);
            Color yellow = Color.FromArgb(255, 255, 0);
            Color magenta = Color.FromArgb(255, 0, 255);
            int cellSize = 8;
            int quadSize = 8;
            int pixelWidth = 2;
            int pixelHeight = 2;


        }

        private bool testCoord(int row, int col)
        {
            int smallRow = row.FloorDivide(32);
            int smallCol = col.FloorDivide(32);
            if (smallRow % 2 != smallCol % 2)
            {
                return true;
            }
            return false;
        }
    }   
}

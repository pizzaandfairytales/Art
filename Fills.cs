using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Art
{
    class PhaseColor : Fill
    {
        public ArtColor Fill(CommonInfo info)
        {
            if (info.Phase >= 0 && info.Palette != null && info.Palette.Count > 0)
            {
                return info.Palette[info.Phase % info.Palette.Count];
            }
            else
            {
                throw new Exception("Insufficient or bad data");
            }
        }
    }

    class CheckerColor : Fill
    {
        public ArtColor Fill(CommonInfo info)
        {
            if (info.Palette.Count < 2)
            {
                throw new Exception("Insufficient or bad data");
            }
            if ((info.Position.row.FloorDivide(info.PatternScale) % 2) != (info.Position.col.FloorDivide(info.PatternScale) % 2))
            {
                return info.Palette[0];
            } else
            {
                return info.Palette[1];
            }
                
        }
    }
}

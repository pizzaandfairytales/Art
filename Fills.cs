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
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OxyPlot;
using System.Windows.Media;

namespace Atreyu.Models
{
    public class OxyPaletteMap
    {
        private const int NUM_COLORS = 210;

        public string Name { get; set; }
        public LinearGradientBrush Brush { get; set; }
        public OxyPalette Palette { get; set; }

        public OxyPaletteMap(string name, LinearGradientBrush brush, OxyPalette palette)
        {
            Name = name;
            Brush = brush;
            Palette = palette;
        }

        public static OxyPaletteMap CreateFromName(string name)
        {
            OxyPalette palette = null;
            var numPoints = 2;

            switch (name)
            {
                case "BlackWhiteRed":
                {
                    numPoints = 3;
                    palette = OxyPalettes.BlackWhiteRed(NUM_COLORS);
                    break;
                }
                case "BlueWhiteRed":
                {
                    numPoints = 3;
                    palette = OxyPalettes.BlueWhiteRed(NUM_COLORS);
                    break;
                }
                case "Cool":
                {
                    numPoints = 3;
                    palette = OxyPalettes.Cool(NUM_COLORS);
                    break;
                }
                case "Gray":
                {
                    numPoints = 2;
                    palette = OxyPalettes.Gray(NUM_COLORS);
                    break;
                }
                case "Hot":
                {
                    numPoints = 5;
                    palette = OxyPalettes.Hot(NUM_COLORS);
                    break;
                }
                case "Hue":
                {
                    numPoints = 7;
                    palette = OxyPalettes.Hue(NUM_COLORS);
                    break;
                }
                case "Jet":
                {
                    numPoints = 5;
                    palette = OxyPalettes.Jet(NUM_COLORS);
                    break;
                }
                case "Rainbow":
                {
                    numPoints = 7;
                    palette = OxyPalettes.Rainbow(NUM_COLORS);
                    break;
                }
                default:
                {
                    return null;
                    break;
                }
            }
            var colorPoints = new Color[7];
            var brush = new LinearGradientBrush();
            brush.StartPoint = new Point(0, 0.5);
            brush.EndPoint = new Point(1, 0.5);
            var division = (NUM_COLORS/(numPoints - 1))-1;
            for (int i = 0; i < numPoints; i++)
            {
                var oxyColor =  palette.Colors[(division*i)];
                colorPoints[i] = Color.FromArgb(oxyColor.A, oxyColor.R, oxyColor.G, oxyColor.B);
                brush.GradientStops.Add(new GradientStop(colorPoints[i], ((double)i/(numPoints-1))));
            }

            return new OxyPaletteMap(name, brush, palette);
        }
    }
}

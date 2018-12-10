using Highsoft.Web.Mvc.Stocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.ViewModels
{
    public class ROMViewModel
    {
        public string dateValue { get; set; }
        public int Flexion { get; set; }
        public int Extension { get; set; }
        public int Pain { get; set; }
    }

    public class ROMChartFlexion
    {
        public decimal dateValue { get; set; }
        public int? Flexion { get; set; }
    }
    public class ROMChartExtension
    {
        public decimal dateValue { get; set; }
        public int? Extension { get; set; }
    }

    public class ROMChartViewModel
    {
        public List<ROMChartFlexion> ROMFlextion { get; set; }
        public List<ROMChartExtension> ROMExtension { get; set; }
        public int ymin { get; set; }
        public int ymax { get; set; }
    }
    public class FlexionViewModel
    {
        public string dateValue { get; set; }
        public int Flexion { get; set; }
        public int Pain { get; set; }
    }
    public class ExtensionViewModel
    {
        public string dateValue { get; set; }
        public int Extension { get; set; }
        public int Pain { get; set; }
    }
    public class ShoulderViewModel
    {
        public string dateValue { get; set; }
        public int Flexion { get; set; }
        public int Pain { get; set; }
    }

    public class HightStockShoulderViewModel
    {
        public List<ColumnrangeSeriesData> ROM { get; set; }
        public List<ColumnSeriesData> Volume { get; set; }

        public List<ColumnSeriesData> PainVolume { get; set; }
    }

    public class ColumnRange
    {
        public double X { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
    }
    public class Volume
    {
        public double X { get; set; }
        public double Y { get; set; }
    }
    public class HightStockShoulderViewModelForJavaScript
    {
        public double X { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Pain { get; set; }
        public double Session { get; set; }

    }
}

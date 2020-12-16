// David Wahid
using System.Collections.Generic;
using Microcharts;
using mobile.Models;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace mobile.Views
{
    public partial class ResultDetailPage : ContentPage
    {
        public ResultDetailPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var result = BindingContext as Result;

            var entries = new List<Microcharts.ChartEntry>()
            {
                new Microcharts.ChartEntry(result.EyeAssymetry)
                {
                    Color = Color.DarkOrchid.ToSKColor(),
                    Label = "Eye",
                    ValueLabel = result.EyeAssymetry.ToString()
                },
                new Microcharts.ChartEntry(result.MouthAssymetry)
                {
                    Color = Color.Blue.ToSKColor(),
                    Label = "Mouth",
                    ValueLabel = result.MouthAssymetry.ToString()
                },
                new Microcharts.ChartEntry(result.NoseAssymetry)
                {
                    Color = Color.DarkOrange.ToSKColor(),
                    Label = "Nose",
                    ValueLabel = result.NoseAssymetry.ToString()
                },
                new Microcharts.ChartEntry(result.JawAssymetry)
                {
                    Color = Color.DarkGray.ToSKColor(),
                    Label = "Jaw",
                    ValueLabel = result.JawAssymetry.ToString()
                },
                new Microcharts.ChartEntry(result.FaceAssymetry)
                {
                    Color = Color.DarkOliveGreen.ToSKColor(),
                    Label = "Face",
                    ValueLabel = result.FaceAssymetry.ToString()
                }
            };

            chart.Chart = new RadarChart
            {
                Entries = entries,
                MaxValue = 10,
                MinValue = 0,
                LabelTextSize = 48,
                Margin = 120,
                BackgroundColor = Color.Transparent.ToSKColor()
            };
        }
    }
}

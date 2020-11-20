// David Wahid
using System;
namespace mobile.Models
{
    public class Result
    {
        public string Id { get; set; }
        public byte[] ImageData { get; set; }
        public double AssymetryScore { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string Landmarks { get; set; }
        public string Status { get; set; }
    }
}

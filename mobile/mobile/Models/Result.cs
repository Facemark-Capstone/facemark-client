// David Wahid
using System;
using shared.Models.Analysis;

namespace mobile.Models
{
    public class Result
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int? Id { get; set; }
        public string OrderId { get; set; }
        public byte[] ImageData { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public OrderStatus Status { get; set; }

        public bool IsSymmetry { get; set; }
        public int AiScore { get; set; }
        public int FaceAssymetry { get; set; }
        public int EyeAssymetry { get; set; }
        public int MouthAssymetry { get; set; }
        public int NoseAssymetry { get; set; }
        public int JawAssymetry { get; set; }
    }
}

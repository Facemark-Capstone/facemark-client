// David Wahid
namespace mobile.Models.Face
{
    public class FaceAttributes
    {
        public double Age { get; set; }
        public string Gender { get; set; }
        public HeadPose HeadPose { get; set; }
        public double Smile { get; set; }
    }

    public class HeadPose
    {
        public double Roll { get; set; }
        public double Yaw { get; set; }
        public double Pitch { get; set; }

        public bool IsCorrect(double max = 10, double min = 10)
        {
            return Roll <= max && Roll >= min
                && Yaw <= max && Yaw >= min
                && Pitch <= max && Pitch >= min;
        }

    }
}

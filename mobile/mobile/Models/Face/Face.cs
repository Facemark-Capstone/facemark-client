// David Wahid
using System;
namespace mobile.Models.Face
{
    public class Face
    {
        public Guid FaceId { get; set; }
        public FaceRectangle FaceRectangle { get; set; }
        public FaceLandmarks FaceLandmarks { get; set; }
        public FaceAttributes FaceAttributes { get; set; }

        public bool IsCorrectPose(double max = 10, double min = 10)
        {
            return FaceAttributes.HeadPose.IsCorrect(max, min);
        }
    }
}

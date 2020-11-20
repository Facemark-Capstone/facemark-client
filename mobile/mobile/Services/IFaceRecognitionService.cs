// David Wahid
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using mobile.Models.Face;

namespace mobile.Services
{
    public interface IFaceRecognitionService
    {
        Task<Face[]> DetectAsync(Stream imageStream, bool returnFaceId, bool returnFaceLandmarks, IEnumerable<FaceAttributeType> returnFaceAttributes);
    }
}

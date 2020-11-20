// David Wahid
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using mobile.Exceptions;
using mobile.Models.Face;
using mobile.Services;
using MvvmHelpers;
using MvvmHelpers.Commands;
using Plugin.Media;
using Plugin.Media.Abstractions;
using shared.Models.Analysis;
using Xamarin.Forms;

namespace mobile.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        IFaceRecognitionService mFaceService { get; }
        IAnalysisService mAnalysisService { get; }

        string TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJkYXZpZC53YWhpZEBnbHV3YS5jb20iLCJyb2xlIjoiQWRtaW4iLCJuYmYiOjE2MDU4MTAwNTgsImV4cCI6MTYwNjQxNDg1OCwiaWF0IjoxNjA1ODEwMDU4LCJpc3MiOiJodHRwczovL2FwaS5mYWNlbWFyay5jb20iLCJhdWQiOiJodHRwczovL2ZhY2VtYXJrLmNvbSJ9.uy8lfMbCLTkHtMyACf3i6zWrY-MMF0DfS2DU7pz0fZg";
        string USERID = "f1c2c97f-7b8e-4c9a-9b2a-2c96ea2a37bb";

        public HomeViewModel()
        {
            mFaceService = Startup.ServiceProvider.GetService<IFaceRecognitionService>();
            mAnalysisService = Startup.ServiceProvider.GetService<IAnalysisService>();
            IsBusy = false;
            IsSuccess = false;
            IsError = false;
        }

        ImageSource image = ImageSource.FromUri(new Uri("https://avatars.dicebear.com/api/human/emily.svg?mood[]=sad"));
        public ImageSource Image
        {
            get => image;
            set => SetProperty(ref image, value);
        }

        public ICommand TakePhotoCommand => new AsyncCommand(async () =>
        {
            await TakePhoto();
        });

        public ICommand CancelCommand => new AsyncCommand(async () =>
       {
           IsBusy = false;
           swapState(false, false, false);
           //photo.Dispose();
       });

        public ICommand AnalyzeCommand => new AsyncCommand(async () =>
        {
            return;
        });

        bool isLoading = false;
        public bool IsLoading
        {
            get => isLoading;
            set => SetProperty(ref isLoading, value);
        }

        bool isError = false;
        public bool IsError
        {
            get => isError;
            set => SetProperty(ref isError, value);
        }

        bool isSuccess = false;
        public bool IsSuccess
        {
            get => isSuccess;
            set => SetProperty(ref isSuccess, value);
        }

        string errorMessage = "We could not detect head pose! Please try again...";
        public string ErrorMessage
        {
            get => errorMessage;
            set => SetProperty(ref errorMessage, value);
        }

        string yaw = string.Empty;
        public string Yaw
        {
            get => yaw;
            set => SetProperty(ref yaw, value);
        }

        string roll = string.Empty;
        public string Roll
        {
            get => roll;
            set => SetProperty(ref roll, value);
        }

        string pitch = string.Empty;
        public string Pitch
        {
            get => pitch;
            set => SetProperty(ref pitch, value);
        }

        bool historyExpanded = false;
        public bool HistoryExpanded
        {
            get => historyExpanded;
            set
            {
                SetProperty(ref historyExpanded, value);
                if (value)
                {
                    HistoryRevealChevron = "\uf078";
                }
                else
                {
                    HistoryRevealChevron = "\uf077";
                }
            }
        }

        string historyRevealChevron = "\uf077";
        public string HistoryRevealChevron
        {
            get => historyRevealChevron;
            set => SetProperty(ref historyRevealChevron, value);
        }

        MediaFile photo;

        private async Task TakePhoto()
        {
            //// Take photo
            if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
            {
                photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    Name = "face.jpg",
                    PhotoSize = PhotoSize.Medium
                });
            }
            else
            {
                IsBusy = true;
                await Application.Current.MainPage.DisplayAlert("No Camera", ":( No camera available.", "OK");
                IsBusy = false;
                return;
            }

            // Take photo
            //if (!CrossMedia.Current.IsPickPhotoSupported)
            //{

            //}
            //var photo = await CrossMedia.Current.PickPhotoAsync();
            Image = ImageSource.FromStream(photo.GetStream);

            try
            {
                if (photo != null)
                {
                    IsBusy = true;
                    var faceAttributes = new FaceAttributeType[] { FaceAttributeType.HeadPose, FaceAttributeType.Age, FaceAttributeType.Smile, FaceAttributeType.Gender };
                    using (var photoStream = photo.GetStream())
                    {
                        swapState(true, false, false);
                        Face[] faces = await mFaceService.DetectAsync(photoStream, true, false, faceAttributes);
                        if (faces.Any())
                        {
                            swapState(false, false, true);

                            if (faces[0].FaceAttributes.HeadPose.Yaw < 5 && faces[0].FaceAttributes.HeadPose.Yaw > -5 &&
                               faces[0].FaceAttributes.HeadPose.Roll < 5 && faces[0].FaceAttributes.HeadPose.Roll > -5 &&
                               faces[0].FaceAttributes.HeadPose.Pitch < 5 && faces[0].FaceAttributes.HeadPose.Pitch > -5)
                            {

                                var prop = Application.Current.Properties;

                                var result = await mAnalysisService.PlaceOrder(photo.GetStream(), (string)prop["hub-id"], USERID, TOKEN);

                                if (result == null)
                                {

                                }
                            }

                            Yaw = faces[0].FaceAttributes.HeadPose.Yaw.ToString("00");
                            Pitch = faces[0].FaceAttributes.HeadPose.Pitch.ToString("00");
                            Roll = faces[0].FaceAttributes.HeadPose.Roll.ToString("00");

                            return;
                        }
                        photo.Dispose();

                        swapState(false, true, false);
                        ErrorMessage = "We could not detect head pose! Please try again...";
                    }
                }
            }
            catch (FaceAPIException fx)
            {
                Debug.WriteLine(fx.Message);
                ErrorMessage = "We could not detect head pose! Please try again...";
                swapState(false, true, false);

            }
            catch (Exception ex)
            {
                swapState(false, true, false);
                ErrorMessage = "Unknow error occured! Please check your connectivity and camera permissions...";
                Debug.WriteLine(ex.Message);
            }
        }

        void swapState(bool isLoading, bool isError, bool isSuccess)
        {
            IsLoading = isLoading;
            IsError = isError;
            IsSuccess = isSuccess;
        }

    }
}

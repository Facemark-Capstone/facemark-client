// David Wahid
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using mobile.Exceptions;
using mobile.Models;
using mobile.Models.Face;
using mobile.Services;
using mobile.Services.SqlLite;
using mobile.Views;
using MvvmHelpers;
using MvvmHelpers.Commands;
using Plugin.Media;
using Plugin.Media.Abstractions;
using shared.Models.AI;
using shared.Models.Analysis;
using Xamarin.Forms;

namespace mobile.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        IFaceRecognitionService mFaceService { get; }
        IAnalysisService mAnalysisService { get; }
        IHubService mHub { get; }
        IAppService mApp { get; }
        IResultsRepo mResultsRepo { get; }

        private static int mRetryFailCount = 0;
        MediaFile photo;

        public HomeViewModel()
        {
            mFaceService = Startup.ServiceProvider.GetService<IFaceRecognitionService>();
            mAnalysisService = Startup.ServiceProvider.GetService<IAnalysisService>();
            mHub = Startup.ServiceProvider.GetService<IHubService>();
            mApp = Startup.ServiceProvider.GetService<IAppService>();
            mResultsRepo = Startup.ServiceProvider.GetService<IResultsRepo>();

            mHub.Connected += MHub_Connected;
            mHub.ConnectionFailed += MHub_ConnectionFailed;
            mHub.OrderStatus += MHub_OrderStatus;

            IsBusy = false;
            IsSuccess = false;
            IsError = false;


            RetryFunction();

            InitResults();
        }

        #region Properties

        public ObservableRangeCollection<Result> Results { get; } = new ObservableRangeCollection<Result>();

        Result selectedResult;
        public Result SelectedResult
        {
            get => selectedResult;
            set => SetProperty(ref selectedResult, value);
        }

        bool hubConnected = false;
        public bool HubConnected
        {
            get => hubConnected;
            set => SetProperty(ref hubConnected, value);
        }

        ImageSource image = ImageSource.FromUri(new Uri("https://avatars.dicebear.com/api/human/emily.svg?mood[]=sad"));
        public ImageSource Image
        {
            get => image;
            set => SetProperty(ref image, value);
        }

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

        #endregion

        private async void InitResults()
        {
            var results = await mResultsRepo.GetResultsAsync();
            if (results != null)
            {
                Results.AddRange(results);
                HistoryExpanded = Results.Count <= 5;
            }
        }

        void RetryFunction()
        {
            var span = TimeSpan.FromSeconds(3);

            Device.StartTimer(span, () =>
             {
                 if (mHub.IsConnected)
                 {
                     HubConnected = true;
                     return false;
                 }

                 Task.Factory.StartNew(async () =>
                 {
                     await mHub.ConnectAsync();
                     if (mRetryFailCount >= 5)
                     {
                         await Device.InvokeOnMainThreadAsync(async () =>
                          {
                              await mApp.ShowAlert("Connection failed", $"We tried to connect the Server {mRetryFailCount} times, but failed ㅠㅠ", "OK");
                          });
                     }
                 });

                 return mRetryFailCount < 5;
             });
        }

        private async void MHub_OrderStatus(object sender, string orderId, string status, string connectionId, OrderResult result)
        {
            var order = await mResultsRepo.GetResultAsyncByOrderId(orderId);
            if (order == null) return;

            order.Status = (OrderStatus)Enum.Parse(typeof(OrderStatus), status, true);
            order.ModifiedAt = DateTime.UtcNow;

            if (order.Status == OrderStatus.Completed && result != null)
            {
                order.IsSymmetry = result.IsSymmetry == 2;
                order.AiScore = (int)(result.Score);
                order.NoseAssymetry = (int)(result.Nose * 100);
                order.MouthAssymetry = (int)(result.Mouth * 100);
                order.JawAssymetry = (int)(result.Jaw * 100);
                order.FaceAssymetry = (int)(result.Face * 100);
                order.EyeAssymetry = (int)(result.Eye * 100);
            }

            await mResultsRepo.SaveResultAsync(order);

            var target = Results.FirstOrDefault(r => r.Id == order.Id);
            Results[Results.IndexOf(target)] = order;
        }

        private async void MHub_ConnectionFailed(object sender, bool successful, string message)
        {
            mRetryFailCount++;
            HubConnected = false;
        }

        private async void MHub_Connected(object sender, bool successful, string message)
        {
            HubConnected = mHub.IsConnected;
            mApp.SetProperty("hub-id", mHub.HubId);
        }

        #region Commands
        public ICommand TakePhotoCommand => new AsyncCommand(OnTakePhoto);


        public ICommand CancelCommand => new AsyncCommand(async () =>
        {
            IsBusy = false;
            swapState(false, false, false);
            //photo.Dispose();
        });

        public ICommand AnalyzeCommand => new AsyncCommand(async () =>
        {
            await PlaceOrder();
            return;
        });

        public ICommand ResultSelected => new AsyncCommand(async () =>
        {
            var page = Startup.ServiceProvider.GetService<ResultDetailPage>();
            if (SelectedResult == null) return;

            page.BindingContext = SelectedResult;
            await mApp.NavigateToAsync(page);

            SelectedResult = null;
        });

        #endregion

        private async Task OnTakePhoto()
        {
            await TakePhoto();
            if (HubConnected && photo != null)
            {
                //await DetectPose(photo);
                await PlaceOrder();
            }
        }

        private async Task PlaceOrder()
        {
            if (HubConnected)
            {
                var result = await mAnalysisService.PlaceOrder(
                        photo.GetStream(),
                        mApp.GetProperty<string>("hub-id"),
                        mApp.GetProperty<string>("user-id"),
                        mApp.GetProperty<string>("jwt-token"));

                if (result != null && !string.IsNullOrWhiteSpace(result.OrderId))
                {
                    byte[] imageBytes;
                    using (var stream = new MemoryStream())
                    {
                        await photo.GetStream().CopyToAsync(stream);
                        imageBytes = stream.ToArray();
                    }

                    Result aiResult = new Result()
                    {
                        OrderId = result.OrderId,
                        ImageData = imageBytes,
                        CreatedAt = DateTime.UtcNow,
                        ModifiedAt = DateTime.UtcNow,
                        Status = OrderStatus.Accepted
                    };

                    await mResultsRepo.SaveResultAsync(aiResult);
                    Results.Add(aiResult);
                }
            }
            else
            {
                await mApp.ShowAlert("Offline", "We could not establish socket with AI server :(", "OK");
            }
        }

        private async Task DetectPose(MediaFile photo)
        {
            if (photo == null || photo == default || photo.GetStream().Length < 1)
            {
                return;
            }

            IsBusy = true;

            try
            {
                var faceAttributes = new FaceAttributeType[] { FaceAttributeType.HeadPose };
                swapState(true, false, false);

                using (var photoStream = photo.GetStream())
                {
                    Face[] faces = await mFaceService.DetectAsync(photoStream, true, false, faceAttributes);

                    if (faces.Any())
                    {
                        if (faces[0].IsCorrectPose(max: 20, min: -20))
                        {
                            swapState(false, false, true);
                            Yaw = faces[0].FaceAttributes.HeadPose.Yaw.ToString("0");
                            Pitch = faces[0].FaceAttributes.HeadPose.Pitch.ToString("0");
                            Roll = faces[0].FaceAttributes.HeadPose.Roll.ToString("0");

                        }
                        else
                        {
                            swapState(false, true, false);
                            ErrorMessage = "Head pose is wrong! Please try again...";
                        }
                    }
                    else
                    {
                        swapState(false, true, false);
                        ErrorMessage = "We could not detect head pose! Please try again...";
                    }

                }
            }
            catch (FaceAPIException fx)
            {
                swapState(false, true, false);
                ErrorMessage = "We could not detect head pose! Please try again...";

            }
            catch (Exception ex)
            {
                swapState(false, true, false);
                ErrorMessage = "Unknow error occured! Please check your connectivity and camera permissions...";
            }

        }

        private async Task TakePhoto()
        {
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
                photo = await CrossMedia.Current.PickPhotoAsync();
                IsBusy = false;
            }

            if (photo != null)
            {
                Image = ImageSource.FromStream(photo.GetStream);
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

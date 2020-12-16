// David Wahid
using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using mobile.Options;
using mobile.Services;
using mobile.Services.SqlLite;
using mobile.Views;
using Xamarin.Essentials;

namespace mobile
{
    public static class Startup
    {
        public static IServiceProvider ServiceProvider { get; set; }

        public static App Init()
        {
            var a = Assembly.GetExecutingAssembly();
            var stream = a.GetManifestResourceStream("mobile.appsettings.json");

            var host = new HostBuilder()
            .ConfigureHostConfiguration(c =>
            {
                // Tell the host configuration where to file the file (this is required for Xamarin apps)
                c.AddCommandLine(new string[] { $"ContentRoot={FileSystem.AppDataDirectory}" });

                //read in the configuration file!
                c.AddJsonStream(stream);
            })
            .ConfigureServices((c, x) =>
            {
                // Configure our local services and access the host configuration
                ConfigureServices(c, x);
            })
            .Build();

            //Save our service provider so we can use it later.
            ServiceProvider = host.Services;

            return ServiceProvider.GetService<App>();
        }

        static void ConfigureServices(HostBuilderContext ctx, IServiceCollection services)
        {
            // The HostingEnvironment comes from the appsetting.json and could be optionally used to configure the mock service
            if (ctx.HostingEnvironment.IsDevelopment())
            {
                //// add as a singleton so only one ever will be created.
                //services.AddSingleton<IDataService, MockDataService>();
            }
            else
            {
                services.AddSingleton<App>();
                services.AddSingleton<IAccountService, AccountService>();
                services.AddSingleton<IFaceRecognitionService, FaceRecognitionService>();
                services.AddSingleton<IAnalysisService, AnalysisService>();
                services.AddSingleton<IResultsRepo, ResultsRepo>();
                services.AddSingleton<IHubService, HubService>();
                services.AddTransient<IAppService, AppService>();
                services.AddTransient<ResultDetailPage>();
                services.AddHttpClient();

                services.Configure<AccountOptions>(ctx.Configuration.GetSection("Facemark:Account"));
                services.Configure<SecurityOptions>(ctx.Configuration.GetSection("Facemark:Security"));
                services.Configure<CognitiveServiceOptions>(ctx.Configuration.GetSection("Facemark:CongnitiveService"));
                services.Configure<AnalysisOptions>(ctx.Configuration.GetSection("Facemark:Analysis"));
            }
        }
    }
}

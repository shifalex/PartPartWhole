using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace PartPartWhole;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseSentry(Options => { Options.Dsn= "https://334a9556e76947b0a3610dcbd62b281b@o4505189710364672.ingest.sentry.io/4505189712461824";})
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

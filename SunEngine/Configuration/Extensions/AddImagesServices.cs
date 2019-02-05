using Microsoft.Extensions.DependencyInjection;
using SunEngine.Services;

namespace SunEngine.Configuration
{
    public static class AddImagesServicesExtensions
    {
        public static void AddImagesServices(this IServiceCollection services)
        {
            services.AddSingleton<IImagesNamesService, ImagesNamesService>();
            services.AddSingleton<ImagesService>();
        }
    }
}
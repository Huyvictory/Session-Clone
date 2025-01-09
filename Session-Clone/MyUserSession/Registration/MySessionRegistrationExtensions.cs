using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Session_Clone.MyUserSession.Registration;

public static class MySessionRegistrationExtensions
{
    public static IServiceCollection AddMySession(this IServiceCollection services)
    {
        services.AddSingleton<IMySessionStorageEngine>(services =>
        {
            var FilePath = Path.Combine(services.GetRequiredService<IHostingEnvironment>().ContentRootPath, "Sessions");
            Directory.CreateDirectory(FilePath);

            return new FileMySessionStorageEngine(FilePath);
        });

        services.AddSingleton<IMySessionStorage, MySessionStorageDictImpl>();
        services.AddScoped<MySessionScopeContainer>();

        return services;
    }
}
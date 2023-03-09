using System.Runtime.CompilerServices;

namespace vite_api.Tests;

public static class VerifyInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifierSettings.UseStrictJson();
    }
}

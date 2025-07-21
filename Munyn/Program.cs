// Program.cs
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI; // Or whatever UI framework/platform extensions you use (e.g., Avalonia.Desktop.Linux)
using System;

namespace Munyn // Make sure this matches your project's root namespace
{
    class Program
    {
        // This is the entry point of your application.
        // It calls BuildAvaloniaApp() and then starts the UI lifetime.
        public static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args); // For desktop apps

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>() // 'App' refers to your App.axaml.cs class
                .UsePlatformDetect() // Detects the appropriate platform (Windows, Linux, macOS)
                .LogToTrace() // Enables logging to output window/console
                .UseReactiveUI(); // Example: if you're using ReactiveUI (otherwise omit or use another extension)
                                  // Add other .Use...() methods for any specific platform setups you need
                                  // e.g., .With(new X11PlatformOptions { Use=X11PlatformUse.Egl }) for specific Linux setups
    }
}
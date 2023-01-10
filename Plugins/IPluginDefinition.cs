using Microsoft.Extensions.DependencyInjection;
using System;

namespace Akiled.Plugins
{
    public interface IPluginDefinition
    {
        string Name { get; }
        string Author { get; }
        Version Version { get; }

        void ConfigureServices(IServiceCollection serviceCollection) { }
        void OnServicesConfigured() { }
        void OnServiceProviderBuild(IServiceProvider serviceProvider) { }

        Type PluginClass { get; }
    }
}
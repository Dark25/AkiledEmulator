using Akiled.Core;
using Akiled.Plugins;
using Akiled.Utilities.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;

namespace Akiled
{
    public static class Program
    {
        private static IEnumerable<Type> _transientTypes;
        [STAThread]
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        
        public static void Main()
        {
            var services = new ServiceCollection();
            _transientTypes = typeof(Program).Assembly.GetTypes().Where(t => t.IsInterface && t.GetCustomAttributes<TransientAttribute>().Any());

            // Plugins
            Directory.CreateDirectory("plugins");
            var pluginAssemblies = new DirectoryInfo("plugins").GetDirectories().Select(d => PluginLoadContext.LoadPlugin(Path.Join("plugins", d.Name), d.Name)).ToList();
            pluginAssemblies.AddRange(new DirectoryInfo("plugins").GetFiles().Where(f => Path.GetExtension(f.Name).Equals(".dll")).Select(f => PluginLoadContext.LoadPlugin(Path.Join("plugins"), Path.GetFileNameWithoutExtension(f.Name))));
            var pluginDefinitions = pluginAssemblies.SelectMany(pluginAssembly => AddPlugin(services, pluginAssembly)).ToList();

            // Dependency Injection
            services.AddDefaultRules(typeof(Program).Assembly);

            foreach (var plugin in pluginDefinitions)
                plugin.OnServicesConfigured();

            var configuration = new ConfigurationData(AkiledEnvironment.PatchDir + "Settings/configuration.ini");
            services.AddSingleton(configuration);

            var serviceProvider = services.BuildServiceProvider();

            foreach (var plugin in pluginDefinitions)
                plugin.OnServiceProviderBuild(serviceProvider);


            InitEnvironment();
            while (true)
            {
                if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                {
                    Console.Write("Akiled CMD> ");
                    string Input = Console.ReadLine();

                    if (Input.Length > 0)
                    {
                        string s = Input.Split(' ')[0];

                        ConsoleCommands.InvokeCommand(s);
                    }
                }
            }
        }

        public static IServiceCollection AddAssignableTo<T>(this IServiceCollection services, Assembly assembly) => services.AddAssignableTo(new[] { assembly }, typeof(T));

        public static IServiceCollection AddAssignableTo<T>(this IServiceCollection services, IEnumerable<Assembly> assemblies) => services.AddAssignableTo(assemblies, typeof(T));

        public static IServiceCollection AddAssignableTo(this IServiceCollection services, Assembly assembly, Type type) => services.AddAssignableTo(new[] { assembly }, type);

        public static IServiceCollection AddAssignableTo(this IServiceCollection services, IEnumerable<Assembly> assemblies, Type type) =>
            services.Scan(scan => scan.FromAssemblies(assemblies)
                .AddClasses(classes => classes.Where(t => t.IsAssignableTo(type) && !t.IsAbstract && !t.IsInterface))
                .UsingRegistrationStrategy(RegistrationStrategy.Append)
                .AsSelfWithInterfaces()
                .WithSingletonLifetime());

        private static IServiceCollection AddDefaultRules(this IServiceCollection services, Assembly assembly)
        {
            foreach (var type in _transientTypes)
                services.AddAssignableTo(assembly, type);

            services.Scan(scan => scan.FromAssemblies(assembly)
                .AddClasses(classes => classes.Where(c => c.GetInterface($"I{c.Name}") != null))
                .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                .AsSelfWithInterfaces()
                .WithSingletonLifetime());
            return services;
        }

        private static IEnumerable<IPluginDefinition> AddPlugin(IServiceCollection services, Assembly pluginAssembly)
        {
            var pluginDefinitions = new List<IPluginDefinition>();
            try
            {
                services.AddDefaultRules(pluginAssembly);

                foreach (var pluginDefinition in pluginAssembly.DefinedTypes.Where(t =>
                             t.ImplementedInterfaces.Contains(typeof(IPluginDefinition))))
                {
                    var plugin = (IPluginDefinition?)Activator.CreateInstance(pluginDefinition);
                    if (plugin != null)
                    {
                        plugin.ConfigureServices(services);
                        services.AddSingleton(plugin);
                        pluginDefinitions.Add(plugin);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to load plugin assembly { pluginAssembly.FullName}. Possibly outdated. {e.Message}");
            }
            return pluginDefinitions;
        }


        [MTAThread]
        public static void InitEnvironment()
        {
            Console.ForegroundColor = ConsoleColor.White;
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);
            AkiledEnvironment.InitiaStartlize();
        }


        private static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Logging.DisablePrimaryWriting(true);
            Logging.LogCriticalException("SYSTEM CRITICAL EXCEPTION: " + ((Exception)args.ExceptionObject).ToString());
            AkiledEnvironment.PreformShutDown(true);
        }
    }
}

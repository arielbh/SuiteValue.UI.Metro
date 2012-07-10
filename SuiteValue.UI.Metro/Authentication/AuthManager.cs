using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Storage;

namespace CodeValue.SuiteValue.UI.Metro.Authentication
{
    public class AuthManager
    {
        public void QueryProviders(IEnumerable<Assembly> assemblies)
        {
            var configuration = new ContainerConfiguration().WithAssemblies(assemblies);
            using (var container = configuration.CreateContainer())
            {
                container.SatisfyImports(this);
            }
        }

        public static async Task<IEnumerable<Assembly>> GetAssemblyListAsync()
        {
            var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;

            List<Assembly> assemblies = new List<Assembly>();
            foreach (StorageFile file in await folder.GetFilesAsync())
            {
                if (file.FileType == ".dll" || file.FileType == ".exe")
                {
                    var filename = file.Name.Substring(0, file.Name.Length - file.FileType.Length);
                    AssemblyName name = new AssemblyName() { Name = filename };
                    Assembly asm = Assembly.Load(name);
                    assemblies.Add(asm);
                }
            }
            return assemblies;
        }

        [ImportMany]
        public IAuthProvider[] Providers { get; protected set; }

        public void ConfigureProviders(dynamic configuration)
        {
            foreach (var provider in Providers)
            {
                provider.Configure(configuration);
            }
        }

        public IAuthProvider PrivderByName(string name)
        {
            return Providers.FirstOrDefault(p => Equals(p.Name, name));
        }
    }
}

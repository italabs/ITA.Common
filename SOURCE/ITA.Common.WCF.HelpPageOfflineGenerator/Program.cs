using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using ITA.Common.WCF.RestHelp;

namespace ITA.Common.WCF.HelpPageOfflineGenerator
{
    class Program
    {        
        private static string CONTENT_FOLDER_NAME = "Content";
        private static string INDEX_PAGE_NAME = "Help.html";
        private static string BaseFolder;

        static int Main(string[] args)
        {
            try
            {
                if (args.Length < 5)
                {
                    return PrintUsage();
                }
                var interfaceAssemblyPath = args[0];
                var interfaceTypeName = args[1];
                var implInterfaceAssemblyPath = args[2];
                var implInterfaceTypeName = args[3];
                var folderPath = args[4];

                var url = args.Length > 5 ? args[5] : null;
                var locale = args.Length > 6 ? args[6] : null;

                var culture = locale != null ? new CultureInfo(locale) : CultureInfo.InvariantCulture;
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;

                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

                BaseFolder = Path.GetDirectoryName(interfaceAssemblyPath);

                var interfaceAssembly = Assembly.LoadFile(interfaceAssemblyPath);
                var interfaceType = interfaceAssembly.GetType(interfaceTypeName);

                var implInterfaceAssembly = Assembly.LoadFile(implInterfaceAssemblyPath);
                var implInterfaceType = implInterfaceAssembly.GetType(implInterfaceTypeName);

                var provider = new HelpPageOfflineProvider();
                var settings = new HelpPageOfflineSettings
                {
                    BaseUrl = url,
                    HelpFolderPath = Path.Combine(folderPath, CONTENT_FOLDER_NAME + "\\"),
                    InterfaceType = interfaceType,
                    InterfaceImplementationType = implInterfaceType
                };
                provider.Generate(settings);

                CreateHtmlIndexFile(folderPath);
            }
            catch (ReflectionTypeLoadException ex)
            {
                var sb = new StringBuilder();
                foreach (var exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    var exFileNotFound = exSub as FileNotFoundException;
                    if (exFileNotFound != null)
                    {
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                Console.WriteLine(sb.ToString());
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            return 0;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyName = new AssemblyName(args.Name);
            var exists = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
            var assembly = exists ?? Assembly.LoadFile(Path.Combine(BaseFolder, assemblyName.Name + ".dll"));
            return assembly;
        }

        private static int PrintUsage()
        {
            Console.WriteLine(
                "Usage: {0}.exe interfaceAssemblyPath interfaceTypeName implInterfaceAssemblyPath implInterfaceTypeName helpFolderPath [url] [locale]",
                Assembly.GetExecutingAssembly().GetName().Name);
            Console.WriteLine("Parameters:");
            Console.WriteLine(" interfaceAssemblyPath  -- path to the service interface definition assembly");
            Console.WriteLine(" interfaceTypeName  -- service interface class name with namespace");
            Console.WriteLine(" implInterfaceAssemblyPath  -- path to the service implementation assembly");
            Console.WriteLine(" implInterfaceTypeName  -- service class name with namespace");
            Console.WriteLine(" helpFolderPath  -- path to the folder for saving the help files");
            Console.WriteLine(" url  -- service endpoint url (default {0})", HelpPageOfflineProvider.DEFAULT_ENDPOINT_URL);
            Console.WriteLine(" locale  -- locale (default InvariantCulture)");
            return 1;
        }

        private static void CreateHtmlIndexFile(string folderPath)
        {
            var text = string.Format("<script type='text/javascript'>window.location = './{0}/{1}'</script>",
                CONTENT_FOLDER_NAME, HelpPageOfflineProvider.CONTRACT_PAGE_NAME);

            File.WriteAllText(Path.Combine(folderPath, INDEX_PAGE_NAME), text);
        }
    }
}

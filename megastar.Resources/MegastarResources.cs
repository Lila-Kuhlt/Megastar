using System.IO;
using System.Linq;
using System.Reflection;

namespace megastar.Resources
{
    public static class MegastarResources
    {
        private static string resourcePrefix = "megastar.Resources";
        public static Assembly ResourceAssembly => typeof(MegastarResources).Assembly;

        /// <summary>
        /// Retrieves a resource from the ResourceAssembly.
        ///
        /// Note: The requested resource has to be included in the assembly. For this see project file.
        /// </summary>
        public static string ReadResouceFile(string fileName)
        {
            string resourceName = ResourceAssembly.GetManifestResourceNames()
                .Single(str => str.EndsWith(fileName));

            using Stream stream = ResourceAssembly.GetManifestResourceStream(resourceName);
            using StreamReader reader = new StreamReader(stream ??
                                                         throw new FileNotFoundException(
                                                             $"Could not load {resourceName} from ResourceAssembly. Does the File exist?"));

            return reader.ReadToEnd();
        }
    }
}

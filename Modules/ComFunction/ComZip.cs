using System.IO;
using System.IO.Packaging;

namespace DebloaterTool
{
    internal class ComZip
    {
        public static void ExtractZipFile(string zipFilePath, string extractPath)
        {
            using (Package package = Package.Open(zipFilePath, FileMode.Open, FileAccess.Read))
            {
                foreach (PackagePart part in package.GetParts())
                {
                    string filePath = Path.Combine(extractPath, part.Uri.OriginalString.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                    // Ensure the directory exists
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    using (Stream source = part.GetStream(FileMode.Open, FileAccess.Read))
                    using (FileStream target = File.Create(filePath))
                    {
                        source.CopyTo(target);
                    }
                }
            }
        }
    }
}

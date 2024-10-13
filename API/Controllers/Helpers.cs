using System.IO;

namespace API.Controllers
{
    public static class Helpers
    {
        public static string GetUniqueFileName(string directory, string originalFileName)
        {
            var fileName = Path.GetFileNameWithoutExtension(originalFileName);
            var extension = Path.GetExtension(originalFileName);
            var newFileName = originalFileName;
            int counter = 1;
            while (System.IO.File.Exists(Path.Combine(directory, newFileName)))
            {
                newFileName = $"{fileName}({counter++}){extension}";
            }

            return newFileName;
        }
    }
}

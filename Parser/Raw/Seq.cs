using System.Text;

namespace Parser.Raw
{
    public class Seq
    {
        public static Dictionary<string, string> ParseAllFiles(string path)
        {
            var importList = new Dictionary<string, string>();
            var files = Directory.GetFiles(path, "*.seq");
            foreach (var file in files)
            {
                var stream = Encoding.GetEncoding(28591).GetString(File.ReadAllBytes(file));

                importList.Add(Path.GetFileName(file), stream);
            }

            return importList;
        }
    }
}

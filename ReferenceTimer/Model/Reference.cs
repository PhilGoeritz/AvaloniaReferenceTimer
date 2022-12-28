namespace ReferenceTimer.Model
{
    public interface IReference
    {
        string Path { get; }
        string Title { get; }
    }

    internal class Reference : IReference
    {
        public string Title { get; }
        public string Path { get; }

        public Reference(string path)
        {
            Title = System.IO.Path.GetFileName(path);
            Path = path;
        }
    }
}

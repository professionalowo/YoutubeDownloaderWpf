namespace YoutubeDownloader.Core.Services.InternalDirectory
{
    public class ChildDirectory(IDirectory parent,string name) : IDirectory
    {
        public string FullPath => Path.Combine(parent.FullPath, name);
    }
}

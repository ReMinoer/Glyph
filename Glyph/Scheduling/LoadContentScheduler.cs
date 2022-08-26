using System.Threading.Tasks;
using Glyph.Scheduling.Base;

namespace Glyph.Scheduling
{
    public delegate void LoadContentDelegate(IContentLibrary contentLibrary);
    public delegate Task LoadContentAsyncDelegate(IContentLibrary contentLibrary);

    public interface ILoadContentTask
    {
        void LoadContent(IContentLibrary contentLibrary);
        Task LoadContentAsync(IContentLibrary contentLibrary);
    }

    public class LoadContentTask : AsyncDelegateTaskBase<LoadContentAsyncDelegate, LoadContentDelegate>, ILoadContentTask
    {
        public LoadContentTask(LoadContentAsyncDelegate asyncTaskDelegate, LoadContentDelegate taskDelegate)
            : base(asyncTaskDelegate, taskDelegate) { }

        public void LoadContent(IContentLibrary contentLibrary) => TaskDelegate(contentLibrary);
        public Task LoadContentAsync(IContentLibrary contentLibrary) => AsyncTaskDelegate(contentLibrary);
    }

    public class LoadContentScheduler : GlyphAsyncScheduler<ILoadContentTask, LoadContentAsyncDelegate, LoadContentDelegate, IContentLibrary>
    {
        public LoadContentScheduler()
            : base((asyncTaskDelegate, taskDelegate) => new LoadContentTask(asyncTaskDelegate, taskDelegate),
                taskDelegate => new LoadContentTask(contentLibrary => Task.Run(() => taskDelegate(contentLibrary)), taskDelegate),
                (x, contentLibrary, _) => x.LoadContentAsync(contentLibrary),
                (x, contentLibrary, _) => x.LoadContent(contentLibrary)) { }
    }
}
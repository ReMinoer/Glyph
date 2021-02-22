using System.Threading.Tasks;
using Glyph.Scheduling.Base;

namespace Glyph.Scheduling
{
    public delegate Task LoadContentDelegate(IContentLibrary contentLibrary);

    public interface ILoadContentTask
    {
        Task LoadContent(IContentLibrary contentLibrary);
    }

    public class LoadContentTask : DelegateTaskBase<LoadContentDelegate>, ILoadContentTask
    {
        public LoadContentTask(LoadContentDelegate taskDelegate)
            : base(taskDelegate) { }

        public Task LoadContent(IContentLibrary contentLibrary) => TaskDelegate(contentLibrary);
    }

    public class LoadContentScheduler : AsyncGlyphScheduler<ILoadContentTask, LoadContentDelegate, IContentLibrary>
    {
        public LoadContentScheduler()
            : base(x => new LoadContentTask(x), (x, contentLibrary, _) => x.LoadContent(contentLibrary)) { }
    }
}
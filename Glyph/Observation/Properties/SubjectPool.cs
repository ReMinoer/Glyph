using System.Collections.Concurrent;
using System.Reactive.Subjects;

namespace Glyph.Observation.Properties
{
    internal class SubjectPool
    {
        static private SubjectPool _instance;
        static public SubjectPool Instance => _instance ?? (_instance = new SubjectPool());
        
        private readonly ConcurrentBag<Subject<string>> _concurrentBag = new ConcurrentBag<Subject<string>>();

        public Subject<string> Get()
        {
            if (!_concurrentBag.TryTake(out Subject<string> item))
                item = new Subject<string>();
            return item;
        }

        public void Recondition(Subject<string> usedSubject)
        {
            _concurrentBag.Add(usedSubject);
        }
    }
}
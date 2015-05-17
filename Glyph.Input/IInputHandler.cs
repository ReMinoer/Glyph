using System.Collections.Generic;

namespace Glyph.Input
{
    public interface IInputHandler
    {
        string Name { get; }
        bool IsTriggered { get; }
        float Value { get; }
        InputSource InputSource { get; }
        void Update(InputManager inputManager);

        T GetComponent<T>(bool includeItself = false) where T : class, IInputHandler;
        List<T> GetAllComponents<T>(bool includeItself = false) where T : class, IInputHandler;
        T GetComponentInChildren<T>(bool includeItself = false) where T : class, IInputHandler;
        List<T> GetAllComponentsInChildren<T>(bool includeItself = false) where T : class, IInputHandler;
    }
}

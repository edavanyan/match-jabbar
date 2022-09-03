using DG.Tweening;
using Unity.VisualScripting;

public class CommandPool<T> : Pool<T> where T : ICommand, IPoolable
{
    public CommandPool(T prototype) : base(prototype)
    {
    }

    protected override T CreateItem(T prototype)
    {
        return (T)prototype.Clone();
    }
}

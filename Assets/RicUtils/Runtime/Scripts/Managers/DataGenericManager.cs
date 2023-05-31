using RicUtils.ScriptableObjects;

namespace RicUtils.Managers
{
    public abstract class DataGenericManager<T, D> : SingletonGenericManager<T> where T : DataGenericManager<T, D> where D : DataManagerScriptableObject
    {
        public D data;
    }
}

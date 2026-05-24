namespace MergeShelter.Save
{
    public interface ISaveService
    {
        void Save(GameSaveData saveData);
        bool TryLoad(out GameSaveData saveData);
        void Delete();
        void Reset();
        bool HasSave();
    }
}

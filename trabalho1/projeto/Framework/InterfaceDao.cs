public interface IDao<T>
{
    void Insert(T obj);
    void Update(T obj);
    void Delete(int id);
    T GetById(int id);
    List<T> GetAll();
}
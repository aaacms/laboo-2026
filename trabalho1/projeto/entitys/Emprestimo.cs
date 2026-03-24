public class Emprestimo
{
    public int Id { get; set; }
    public int Usuario_Id { get; set; }
    public int Livro_Id { get; set; }
    public DateOnly Data_emprestimo { get; set; }
}
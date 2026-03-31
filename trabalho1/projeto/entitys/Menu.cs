using Microsoft.VisualBasic;

public class Menu
{
    private BaseDao<Autor> daoAutor;
    private BaseDao<Livro> daoLivro;
    private BaseDao<Usuario> daoUsuario;
    private BaseDao<Emprestimo> daoEmprestimo;

    private List<Autor> listaAutores;
    private List<Livro> listaLivros;
    private List<Usuario> listaUsuarios;
    private List<Emprestimo> listaEmprestimos;

    public Menu(BaseDao<Autor> daoAutor, BaseDao<Livro> daoLivro, BaseDao<Usuario> daoUsuario, BaseDao<Emprestimo> daoEmprestimo)
    {

        this.daoAutor = daoAutor;
        this.daoLivro = daoLivro;
        this.daoUsuario = daoUsuario;
        this.daoEmprestimo = daoEmprestimo;

        this.listaAutores = daoAutor.GetAll();
        this.listaLivros = daoLivro.GetAll();
        this.listaUsuarios = daoUsuario.GetAll();
        this.listaEmprestimos = daoEmprestimo.GetAll();

    }

    public void mostrarMenu()
    {
        bool running = true;
        while (running)
        {
            Console.Clear();
            Console.WriteLine("=== MENU PRINCIPAL ===");
            Console.WriteLine("1. Menu Autor");
            Console.WriteLine("2. Menu Livro");
            Console.WriteLine("3. Menu Usuário");
            Console.WriteLine("4. Menu Empréstimo");
            Console.WriteLine("5. Sair");
            Console.Write("Escolha uma opção: ");

            string opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1":
                    MenuGenerico.ExecutarCrud(daoAutor, listaAutores, null, null);
                    break;
                case "2":
                    MenuGenerico.ExecutarCrud(daoLivro, listaLivros, listaAutores, null);
                    break;
                case "3":
                    MenuGenerico.ExecutarCrud(daoUsuario, listaUsuarios, null, null);
                    break;
                case "4":
                    MenuGenerico.ExecutarCrud(daoEmprestimo, listaEmprestimos, listaLivros, listaUsuarios);
                    break;
                case "5":
                    running = false;
                    Console.WriteLine("Encerrando...");
                    break;
                default:
                    Console.WriteLine("Opção inválida!");
                    Console.ReadKey();
                    break;
            }
        }

    }
}
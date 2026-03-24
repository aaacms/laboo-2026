public class Menu
{
    public Menu(BaseDao<Autor> daoAutor, BaseDao<Livro> daoLivro, BaseDao<Usuario> daoUsuario, BaseDao<Emprestimo> daoEmprestimo)
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
                    MenuGenerico.ExecutarCrud(daoAutor);
                    break;
                case "2":
                    MenuGenerico.ExecutarCrud(daoLivro);
                    break;
                case "3":
                    MenuGenerico.ExecutarCrud(daoUsuario);
                    break;
                case "4":
                    MenuGenerico.ExecutarCrud(daoEmprestimo);
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
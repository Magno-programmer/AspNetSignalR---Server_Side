namespace AspNetSignalIR.ClientListen;

internal class Client
{
    public int Id { get; private set; }
    public string Nome { get; set; }

    private static readonly string Login = "admin";
    private static readonly string Senha = "123456";

    private static int _idIncrement = 1;
    private static readonly Stack<int> availableIds = new();

    public Client(string name)
    {
        Nome = name;

        if (availableIds.Count > 0)
        {
            this.Id = availableIds.Pop(); // Reutiliza ID disponível
        }
        else
        {
            this.Id = _idIncrement++;
        }

    }

    public bool ValidarCredenciais(string login, string senha)
    {
        return Login == login && (Senha == senha && senha.Length >= 6);
    }


    public static void RemoverCliente(Client client)
    {
        // Adiciona o ID do cliente removido para reutilização
        availableIds.Push(client.Id);
    }
}

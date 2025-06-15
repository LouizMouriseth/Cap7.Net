using Core.SeedWork;

namespace Core;

public class Acao : GenericModel
{
    public Acao()
    {
        
    }
    
    public Acao(string descricao, string categoria)
    {
        Descricao = descricao;
        Categoria = categoria;
    }

    public string Descricao { get; private set; }
    public string Categoria { get; private set; }

    public IReadOnlyCollection<UnidadeAcao> UnidadesAcoes => _unidadesAcoes;
    private readonly List<UnidadeAcao> _unidadesAcoes = [];
}
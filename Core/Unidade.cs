using Core.SeedWork;

namespace Core;

public class Unidade : GenericModel
{
    public Unidade()
    {
        
    }
    
    public Unidade(string nome, string endereco, string estado, float area, DateTime inicioOperacao, Guid idEmpresa)
    {
        Nome = nome;
        Endereco = endereco;
        Estado = estado;
        Area = area;
        InicioOperacao = inicioOperacao;
        IdEmpresa = idEmpresa;
    }

    public string Nome { get; private set; }
    public string Endereco { get; private set; }
    public string Estado { get; private set; }
    public float Area { get; private set; }
    public DateTime InicioOperacao { get; private set; }
    
    public Guid IdEmpresa { get; private set; }
    
    public virtual Empresa Empresa { get; private set; }

    public IReadOnlyCollection<UnidadeAcao> UnidadesAcoes => _unidadesAcoes;
    private readonly List<UnidadeAcao> _unidadesAcoes = [];

    public IReadOnlyCollection<Consumo> Consumos => _consumos;
    private readonly List<Consumo> _consumos = [];

}
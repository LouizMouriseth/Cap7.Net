using Core.SeedWork;

namespace Core;

public class Empresa : GenericModel
{
    public Empresa()
    {
        
    }
    
    public Empresa(string nome, string cnpj, string segmento)
    {
        Nome = nome;
        Cnpj = cnpj;
        Segmento = segmento;
    }

    public string Nome { get; private set; }
    public string Cnpj { get; private set; }
    public string Segmento { get; private set; }

    public IReadOnlyCollection<Unidade> Unidades => _unidades;
    private readonly List<Unidade> _unidades = [];
}
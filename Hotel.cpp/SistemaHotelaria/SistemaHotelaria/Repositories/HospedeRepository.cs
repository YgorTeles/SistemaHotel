using SistemaHotelaria.Entities;

namespace SistemaHotelaria.Repositories;

public class HospedeRepository
{
    private List<Hospede> _hospedes = new List<Hospede>();

    public void Adicionar(Hospede hospede)
    {
        hospede.Id = Guid.NewGuid();
        _hospedes.Add(hospede);
    }

    public List<Hospede> ListarTodos()
    {
        return _hospedes;
    }

    public Hospede? BuscarPorCpf(string cpf)
    {
        foreach (Hospede hospede in _hospedes)
        {
            if (hospede.Cpf == cpf)
                return hospede;
        }

        return null;
    }
}
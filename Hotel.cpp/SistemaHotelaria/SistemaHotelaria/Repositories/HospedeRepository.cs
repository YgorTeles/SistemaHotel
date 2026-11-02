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

    public void Atualizar(Hospede hospedeAtualizado)
    {
        for (int i = 0; i < _hospedes.Count; i++)
        {
            if (_hospedes[i].Id == hospedeAtualizado.Id)
            {
                _hospedes[i] = hospedeAtualizado;
                return;
            }
        }
    }
    public void Remover(Guid id)
    {
        for (int i = 0; i < _hospedes.Count; i++)
        {
            if (_hospedes[i].Id == id)
            {
                _hospedes.RemoveAt(i);
                return;
            }
        }
    }
}
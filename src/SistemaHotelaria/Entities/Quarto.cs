namespace SistemaHotelaria.Entities;

public class Quarto
{
    public Guid Id { get; private set; }
    public int Numero { get; private set; }
    public string Tipo { get; private set; }
    public int Capacidade { get; private set; }
    public decimal ValorDiaria { get; private set; }
    public bool Ocupado { get; private set; }

    // Novo quarto: o Id é gerado aqui, na criação do objeto
    public Quarto(int numero, string tipo, int capacidade, decimal valorDiaria)
    {
        Id = Guid.NewGuid();
        Numero = numero;
        Tipo = tipo;
        Capacidade = capacidade;
        ValorDiaria = valorDiaria;
        Ocupado = false;
    }

    // Quarto vindo do banco: recebe o Id que já existe
    public Quarto(Guid id, int numero, string tipo, int capacidade, decimal valorDiaria, bool ocupado)
    {
        Id = id;
        Numero = numero;
        Tipo = tipo;
        Capacidade = capacidade;
        ValorDiaria = valorDiaria;
        Ocupado = ocupado;
    }

    public void Atualizar(string tipo, int capacidade, decimal valorDiaria)
    {
        Tipo = tipo;
        Capacidade = capacidade;
        ValorDiaria = valorDiaria;
    }

    public void Ocupar()
    {
        Ocupado = true;
    }

    public void Liberar()
    {
        Ocupado = false;
    }
}

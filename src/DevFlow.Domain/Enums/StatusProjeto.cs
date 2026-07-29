// DevFlow.Domain/Enums/Enums.cs
namespace DevFlow.Domain.Enums;

public enum StatusProjeto
{
    EmDia = 0,
    Atencao = 1,
    Critico = 2
}

public enum PrioridadeChamado
{
    Baixa = 0,
    Media = 1,
    Alta = 2,
    Urgente = 3
}

public enum StatusChamado
{
    Aberto = 0,
    EmAndamento = 1,
    Resolvido = 2,
    Fechado = 3
}

public enum NivelTarefa
{
    Facil = 0,
    Media = 1,
    Dificil = 2
}

public enum StatusTarefa
{
    AFazer = 0,
    Fazendo = 1,
    Concluida = 2
}
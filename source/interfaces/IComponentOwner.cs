using Godot;

// ReSharper disable once CheckNamespace
namespace Interfaces;

public interface IComponentOwner
{
    // Método genérico para buscar qualquer componente no objeto
    T GetComponent<T>() where T : class;
}
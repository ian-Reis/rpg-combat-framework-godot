using Godot;

namespace RPGFramework.Helpers;

public static class DictionaryExtensions
{
    // Estende qualquer dicionário de string/Node para buscar e tipar automaticamente
    public static bool TryGetNode<T>(this Godot.Collections.Dictionary<StringName, Node> dict, string key, out T typedNode) where T : Node
    {
        if (dict.TryGetValue(key, out var node) && node is T result)
        {
            typedNode = result;
            return true;
        }
        
        typedNode = null;
        return false;
    }
}

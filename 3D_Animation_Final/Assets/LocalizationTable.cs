using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizationTable", menuName = "Localization/Table")]
public class LocalizationTable : ScriptableObject
{
    [Serializable]
    public class Entry
    {
        public string key;
        [TextArea] public string en;
        [TextArea] public string fr;
    }

    public List<Entry> entries = new();

    private Dictionary<string, Entry> _map;

    private void OnEnable()
    {
        _map = entries?.Where(e => !string.IsNullOrEmpty(e.key))
                       .GroupBy(e => e.key)
                       .ToDictionary(g => g.Key, g => g.First());
    }

    public string Get(string key)
    {
        if (_map == null) OnEnable();
        if (!_map.TryGetValue(key, out var e)) return $"[{key}]";
        var lang = LanguageService.Instance != null ? LanguageService.Instance.CurrentLanguage : Language.English;
        return lang == Language.French ? e.fr : e.en;
    }
}

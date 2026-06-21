using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace megastar.Game;

public static class FunFacts
{
    public static string RandomFunFact()
    {
        string json = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "funfacts.json"));
        var facts = JsonSerializer.Deserialize<List<string>>(json) ?? [];

        Random random = new Random();

        return facts[random.Next(facts.Count)];
    }
}

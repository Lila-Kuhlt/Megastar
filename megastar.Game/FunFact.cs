using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;

namespace megastar.Game;

public class FunFact
{
    public string Text { get; set; }


    public static string GetCowFunfact()
    {
        string json = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "funfacts.json"));

        List<FunFact> facts = JsonSerializer.Deserialize<List<FunFact>>(json) ?? [];
        Random random = new Random();
        FunFact randomFact = facts[random.Next(facts.Count)];

        return randomFact.Text;
    }
}

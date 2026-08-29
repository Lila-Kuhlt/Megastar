using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.Json;

namespace megastar.Game;

public class FunFact
{
    public string Text { get; set; }


    public static string GetCowFunfact()
    {
        Assembly assembly = Assembly.Load("megastar.Resources");
        using Stream? stream = assembly.GetManifestResourceStream("megastar.Resources.funfacts.json");

        if (stream != null)
        {
            using StreamReader reader = new StreamReader(stream);
            string json = reader.ReadToEnd();

            List<FunFact> facts = JsonSerializer.Deserialize<List<FunFact>>(json) ?? [];
            Random random = new Random();
            FunFact randomFact = facts[random.Next(facts.Count)];

            return randomFact.Text;
        }

        return "";
    }
}

using System.Globalization;
using System.Text.Json;

static class Program
{
    static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
    static double Numeric(string value)
    {
        double result = double.Parse(value, Culture);
        Require(double.IsFinite(result), "Finite numeric input required");
        return result;
    }
    static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }
    static List<string[]> Input(int columns)
    {
        var rows = new List<string[]>();
        string? line;
        while ((line = Console.ReadLine()) is not null)
        {
            Require(rows.Count < 100000, "Input capacity exceeded");
            string[] fields = line.Split(',');
            Require(fields.Length == columns && fields.All(f => !string.IsNullOrWhiteSpace(f)), "Invalid CSV row");
            rows.Add(fields);
        }
        Require(rows.Count > 0, "Missing input");
        return rows;
    }
    static int Main(string[] args)
    {
        try
        {
            if (args.SequenceEqual(new[] { "--self-test" })) { Test(); Console.WriteLine("Tests passed"); }
            else Run();
            return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error.Message); return 2; }
    }

    record Load(string Id, double Kw, double Cost, bool Protected);
    record Plan(List<string> Shed, double ReleasedKw, double Cost);
    static Plan Optimize(List<Load> loads, double required)
    {
        Require(required >= 0 && loads.Count <= 24, "Invalid demand or too many loads");
        Require(loads.Select(l => l.Id).Distinct().Count() == loads.Count, "Duplicate load");
        foreach (var load in loads) Require(load.Kw > 0 && load.Cost >= 0, "Invalid load");
        var candidates = loads.Where(l => !l.Protected).OrderByDescending(l => l.Kw).ToList();
        Plan? best = null;
        var selected = new List<string>();
        double[] remaining = new double[candidates.Count + 1];
        for (int i = candidates.Count - 1; i >= 0; i--) remaining[i] = remaining[i + 1] + candidates[i].Kw;
        void Search(int index, double released, double cost)
        {
            if (best is not null && cost > best.Cost) return;
            if (released >= required)
            {
                if (best is null || cost < best.Cost || (cost == best.Cost && released < best.ReleasedKw)) best = new Plan(new List<string>(selected), released, cost);
                return;
            }
            if (index == candidates.Count || released + remaining[index] < required) return;
            Search(index + 1, released, cost);
            selected.Add(candidates[index].Id); Search(index + 1, released + candidates[index].Kw, cost + candidates[index].Cost); selected.RemoveAt(selected.Count - 1);
        }
        Search(0, 0, 0);
        return best ?? throw new InvalidDataException("Insufficient nonessential capacity");
    }
    static void Test()
    {
        var plan = Optimize(new List<Load> { new("A", 10, 8, false), new("B", 6, 2, false), new("C", 4, 2, false), new("D", 100, 0, true) }, 10);
        Require(plan.Cost == 4 && plan.ReleasedKw == 10 && !plan.Shed.Contains("D"), "Optimal subset mismatch");
        Require(Optimize(new List<Load>(), 0).Shed.Count == 0, "Zero deficit mismatch");
    }
    static void Run()
    {
        var loads = Input(4).Select(r => { Require(r[3] is "0" or "1", "Invalid protected flag"); return new Load(r[0], Numeric(r[1]), Numeric(r[2]), r[3] == "1"); }).ToList();
        Console.WriteLine(JsonSerializer.Serialize(Optimize(loads, 80)));
    }

}

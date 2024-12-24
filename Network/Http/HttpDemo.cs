using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Web;

namespace Network.Http;

public class HttpDemo
{
    private string url = "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json";
    
    public void Run()
    {
        Console.WriteLine("Http Demo");
        Task.Run(ShowRates).Wait();
    }

    private async Task ShowRates()
    {
        HttpClient client = new();
        var res = await client.GetAsync(url);
        Console.WriteLine("HTTP/{0} {1} {2}", res.Version, (int)res.StatusCode, res.ReasonPhrase);
        foreach (var header in res.Headers)
        {
            Console.WriteLine("{0}: {1}", header.Key, HttpUtility.UrlDecode(header.Value.First(), Encoding.Latin1));
        }
        Console.WriteLine();
        // !! хотя пакет получен, получение тела - снова асинхронная операция
        string body = await res.Content.ReadAsStringAsync();
        //Console.WriteLine(body);
        
        // !! Работа с сетью также требует системных процессов, из-за чего автоматически ресурсы не выспобождаются
        // и неободимо использовать или Dispose() или using (при создании)
        client.Dispose();
        
        // ORM
        var rates = JsonSerializer.Deserialize<NbuRate[]>(body);
        //var rate = rates.Where()
        foreach (var rate in rates.OrderBy(r => r.cc))
        {
            Console.WriteLine(rate);
        }
    }
}

internal class NbuRate
{
    public int r030 { get; set; }
    public string txt { get; set; }
    public double rate { get; set; }
    public string cc { get; set; }
    public string exchangeDate { get; set; }
    public override string ToString()
    {
        return $"{cc} ({txt}) {rate}";
    }
}

// bank.gov.ua/NBUStatService/v1/statdirectory/exchange?date=20200302&json
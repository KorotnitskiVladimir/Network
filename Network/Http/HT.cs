using System.Text;
using System.Text.Json;
using System.Web;

namespace Network;

public class HT
{
    private string? url;
    private List<string> curr;
    private string? date;
    public void Run()
    {
        Console.WriteLine("enter codes of currencies to show (e.g USD, EUR, etc. empty input - all currencies)");
        string? c = Console.ReadLine();
        CurrencyParser(c, out curr);
        Console.WriteLine("please enter date");
        date = Console.ReadLine();
        url = "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?date=" + DateModifier(date) +
                         "&json";
        Task.Run(ShowRates).Wait();
    }

    private async Task ShowRates()
    {
        HttpClient client = new();
        var res = await client.GetAsync(url);
        string body = await res.Content.ReadAsStringAsync();
        client.Dispose();
        var rates = JsonSerializer.Deserialize<NbuRate[]>(body);

        Console.WriteLine("Exchange rates for {0}", date);
        if (curr.Count > 0)
        {
            foreach (var rate in rates.Where(r => curr.Contains(r.cc)))
            {
                Console.WriteLine(rate);
            }
        }
        else
        {
            foreach (var rate in rates)
            {
                Console.WriteLine(rate);
            }
        }
        Console.WriteLine("would you like to sort results - y/n?");
        char choice = Convert.ToChar(Console.ReadLine());
        switch (choice)
        {
            case 'y':
                Sorter(rates);
                break;
            case 'n':
                Console.WriteLine("Good bye!");
                break;
            default:
                Console.WriteLine("Wrong input");
                break;
        }

    }

    private string DateModifier(string input)
    {
        string sub1 = "", sub2 = "", sub3 = "";
        string res = "";
        int i = 0;
        while (true)
        {
            if (Char.IsDigit(input[i]))
            {
                sub1 += input[i];
                i++;
            }
            else
            {
                i++;
                break;
            }
        }
        while (true)
        {
            if (Char.IsDigit(input[i]))
            {
                sub2 += input[i];
                i++;
            }
            else
            {
                i++;
                break;
            }
        }
        while (i < input.Length)
        {
            if (char.IsDigit(input[i]))
            {
                sub3 += input[i];
                i++;
            }
            else
            {
                i++;
                break;
            }
        }
        if (sub1.Length == 2)
            res = sub3 + sub2 + sub1;
        else
            res = sub1 + sub2 + sub3;

        return res;
    }

    private void CurrencyParser(string currencies, out List<string> list)
    {
        int i = 0;
        string sub = "";
        list = new();
        while (i < currencies.Length)
        {
            if (Char.IsLetter(currencies[i]))
            {
                sub += currencies[i];
                i++;
            }
            else
            {
                list.Add(sub);
                sub = "";
                i++;
            }

            if (i == currencies.Length - 1)
            {
                sub += currencies[i];
                list.Add(sub);
            }
        }
    }

    private void Sorter(NbuRate[] arr)
    {
        int choice = -1;
        Console.WriteLine("Please select how to sort:\n1 - code ascending\n2 - code descending\n" +
                          "3 - rate ascending\n4 - rate descending\n0 - exit");
        if (curr.Count == 0)
        {
            while (choice != 0)
            {
                choice = Convert.ToInt32(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        foreach (var rate in arr.OrderBy(r => r.cc))
                        {
                            Console.WriteLine(rate);
                        }

                        break;
                    case 2:
                        foreach (var rate in arr.OrderByDescending(r => r.cc))
                        {
                            Console.WriteLine(rate);
                        }

                        break;
                    case 3:
                        foreach (var rate in arr.OrderBy(r => r.rate))
                        {
                            Console.WriteLine(rate);
                        }

                        break;
                    case 4:
                        foreach (var rate in arr.OrderByDescending(r => r.rate))
                        {
                            Console.WriteLine(rate);
                        }

                        break;
                    case 0:
                        Console.WriteLine("Good bye");
                        break;
                    default:
                        Console.WriteLine("Wrong input");
                        break;
                }
            }
        }
        else
        {
            var rates = (arr.Where(a => curr.Contains(a.cc)));
            while (choice != 0)
            {
                choice = Convert.ToInt32(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        foreach (var rate in rates.OrderBy(r => r.cc))
                        {
                            Console.WriteLine(rate);
                        }

                        break;
                    case 2:
                        foreach (var rate in rates.OrderByDescending(r => r.cc))
                        {
                            Console.WriteLine(rate);
                        }

                        break;
                    case 3:
                        foreach (var rate in rates.OrderBy(r => r.rate))
                        {
                            Console.WriteLine(rate);
                        }

                        break;
                    case 4:
                        foreach (var rate in rates.OrderByDescending(r => r.rate))
                        {
                            Console.WriteLine(rate);
                        }
                        break;
                    case 0:
                        Console.WriteLine("Good bye");
                        break;
                    default:
                        Console.WriteLine("Wrong input");
                        break;
                }
            }
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
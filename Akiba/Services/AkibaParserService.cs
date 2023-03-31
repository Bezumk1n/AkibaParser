using Akiba.Extensions;
using Akiba.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Akiba.Services
{
    internal class AkibaParserService
    {
        public async Task Parse()
        {
            Console.WriteLine("Начинаю парсинг Akiba.su");

            using (var client = new HttpClient())
            {
                bool done = false;
                int page = 0;

                var links = new List<Link>();
                try
                {
                    while (done != true)
                    {
                        Console.WriteLine($"Собираю ссылки со страницы: {page}");
                        var response = await client.GetStringAsync($"https://akiba.su/shop/?p={page}");
                        var foundedLinks = response.Split("<a  class=\"information__description\"");
                        var filteredLinks = foundedLinks.Where(q => q.Contains("/shop/")).Skip(1).ToArray();

                        if (!filteredLinks.Any())
                        {
                            done = true;
                            break;
                        }

                        foreach (var link in filteredLinks)
                        {
                            var normalizedLink = link.GetValue("\"", "/\"");
                            var url = $"https://akiba.su{normalizedLink}";

                            links.Add(new Link()
                            {
                                Url = url,
                                IsParsed = false
                            });
                        }
                        page++;
                    }
                    Console.WriteLine($"Найдено ссылок: {links.Count()}");
                    await ParseLinks(client, links);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка поиска ссылок. Продолжение невозможно");
                }
                finally
                {
                    Console.WriteLine($"Поиск завершён");
                }
            }
        }
        private async Task ParseLinks(HttpClient client, List<Link> links)
        {
            var items = new List<AkibaItemInformation>();
            try
            {
                int total = links.Count;
                int counter = 1;
                foreach (var link in links)
                {
                    Console.WriteLine($"Ссылка {counter++} из {total}: {link.Url}");
                    var result = await ScrapInformation(link.Url, client);
                    items.Add(result);
                    link.IsParsed = true;
                }
            }
            catch (Exception ex)
            {
            }
            SaveLinks(links);
            SaveItemsInformation(items);
        }
        private async Task<AkibaItemInformation> ScrapInformation(string url, HttpClient client)
        {
            var response = await client.GetStringAsync(url);
            var filteredDiv = response.GetValue("class=\"main-object__img\"", "</span></div>");

            var code = filteredDiv.GetValue("<div class=\"main-object__product-code\">", "</div>");
            var codeClear = code.GetValue("<span>", "</span>");
            var title = filteredDiv.GetValue("<div class=\"main-object__product-title\">", "</div>");
            var titleClear = title.GetValue(">", "</h1>");
            var price = filteredDiv.GetValue("<div class=\"main-object__price\">", "</div>");
            var priceClear = price.GetValue("<span>", "</span>");
            var qty = filteredDiv.GetValue("<div class = \"information__subdescription information__subdescription", "/div>");
            var qtyClear = qty.GetValue(">", "<");
            //var description = response.GetValue("<div id=\"object-tab_01\" class=\"object-tabs__block\">", "</div>");

            var result = new AkibaItemInformation()
            {
                Code = codeClear,
                Title = titleClear,
                Price = priceClear,
                Qty = qtyClear,
                //Description = description,
                Url = url
            };
            return result;
        }
        private void SaveLinks(List<Link> links)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Статус;Ссылка;");
            foreach (var link in links)
            {
                sb.Append($"{link.IsParsed};");
                sb.Append($"{link.Url};");
                sb.AppendLine();
            }
            sb.ToString().SaveToFile("Akiba_Links_");
        }
        private void SaveItemsInformation(List<AkibaItemInformation> items)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Код;Наименование;Цена;Количество;Ссылка;");
            foreach (var item in items)
            {
                sb.Append($"{item.Code};");
                sb.Append($"{item.Title};");
                sb.Append($"{item.Price};");
                sb.Append($"{item.Qty};");
                //sb.Append($"{item.Description};"); // Глючит, нужна отладка
                sb.Append($"{item.Url};");
                sb.AppendLine();
            }
            sb.ToString().SaveToFile("Akiba_");
        }
    }
}

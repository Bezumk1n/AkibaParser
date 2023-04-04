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
    internal class ComicstreetParserService
    {
        public async Task Parse()
        {
            Console.WriteLine("Начинаю парсинг Comicstreet.ru");

            using (var client = new HttpClient())
            {
                bool done = false;
                int page = 1;

                var links = new List<Link>();
                try
                {
                    while (done != true)
                    {
                        Console.WriteLine($"Собираю ссылки со страницы: {page}");
                        var response = await client.GetStringAsync($"https://www.comicstreet.ru/collection/all?page={page}").ConfigureAwait(true);
                        var foundedLinks = response.Split("<div class=\"product_card-title\">").Select(q => q.Trim());

                        var filteredLinks = foundedLinks.Where(q => q.StartsWith("<a href=\"/collection")).ToArray();

                        if (!filteredLinks.Any())
                        {
                            done = true;
                            break;
                        }

                        foreach (var link in filteredLinks)
                        {
                            var normalizedLink = link.GetValue("\"", "\">");
                            var url = $"https://www.comicstreet.ru{normalizedLink}";

                            links.Add(new Link()
                            {
                                Url = url,
                                IsParsed = false
                            });
                        }
                        page++;
                    }
                    Console.WriteLine($"Найдено ссылок: {links.Count()}");

                    var items = await ParseLinks(client, links);

                    SaveLinks(links);
                    SaveItemsInformation(items);
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
        private async Task<List<ComicstreetItemInformation>> ParseLinks(HttpClient client, List<Link> links)
        {
            var items = new List<ComicstreetItemInformation>();
            foreach (var link in links)
            {
                Console.WriteLine($"Обработано: {link.Url}");
                var result = await ScrapInformation(link.Url, client);
                items.Add(result);
                link.IsParsed = true;
            }
            return items;
        }
        private async Task<ComicstreetItemInformation> ScrapInformation(string url, HttpClient client)
        {
            var result = new ComicstreetItemInformation()
            {
                Url = url,
            };
            try
            {
                var response = await client.GetStringAsync(url);

                var title = response.GetValue("<h1 class=\"product-title\" itemprop=\"name\">", "</h1>");
                var code = response.GetValue("itemprop=\"gtin13\">", "</span>");
                var price = response.GetValue("<span class=\"product-price js-product-price\">", "</span>");

                var categoryMixed = response.GetValue("<div id=\"insales-section-breadcrumb\" class=\"insales-section insales-section-breadcrumb\">", "</div>");
                var splitedCategories = categoryMixed.Split("<span itemprop=\"name\"").Skip(1).Select(q => q.Trim());
                var splitedCategoriesClered = splitedCategories.Select(q => q.GetValue(">", "<")).ToArray();

                result.Code = code;
                result.Title = title;
                result.Price = price;
                result.Category = splitedCategoriesClered;
                result.IsCompleted = true;
            }
            catch (Exception ex)
            {
                result.IsCompleted = false;
                result.ErrorMessage = ex.Message;
            }
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
            sb.ToString().SaveToFile("Comicstreet_Links");
        }
        private void SaveItemsInformation(List<ComicstreetItemInformation> items)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Статус;Ошибка;Ссылка;Код;Наименование;Цена;Категории;");

            foreach (var item in items)
            {
                sb.Append($"{item.IsCompleted};");
                sb.Append($"{item.ErrorMessage};");
                sb.Append($"{item.Url};");
                sb.Append($"{item.Code};");
                sb.Append($"{item.Title};");
                sb.Append($"{item.Price};");

                foreach (var category in item.Category)
                {
                    sb.Append($"{category};");
                }
                sb.AppendLine();
            }

            sb.ToString().SaveToFile("Comicstreet_ParsedItems");
        }
    }
}

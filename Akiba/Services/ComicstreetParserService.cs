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

                    var linksCount = links.Count();

                    var streams = 2;
                    var firstPart = linksCount / streams;
                    var secondPart = linksCount - firstPart;

                    var items = new List<ComicstreetItemInformation>();

                    //var firstTask = Task.Run(async () =>
                    //{
                    //    var firstPartLinks = links.Take(firstPart).ToList();
                    //    var list = await ParseLinks(client, firstPartLinks);
                    //    items.AddRange(list);
                    //});
                    //var secondTask = Task.Run(async () =>
                    //{
                    //    var secondPartLinks = links.Skip(firstPart).ToList();
                    //    var list = await ParseLinks(client, secondPartLinks);
                    //    items.AddRange(list);
                    //});

                    //await Task.WhenAll(firstTask, secondTask);

                    SaveLinks(links);
                    //SaveItemsInformation(items);
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
    }
}

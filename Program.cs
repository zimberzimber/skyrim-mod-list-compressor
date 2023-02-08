using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Diagnostics;

namespace SkyrimModListCompressor
{
    class Program
    {
        const string OUTPUT_FILE_NAME = "skyrim_mod_list.html";
        const string TEMPLATE_FILE_NAME = "template.html";

        const string MOD_COUNT_TEMPLATE_START = "{{mod_count}}";
        const string LIST_TEMPLATE_START = "@list_template{{";
        const string NON_NEXUS_LIST_TEMPLATE_START = "@non_nexus_template{{";
        const string TEMPLATE_END = "}}";

        static void Main(string[] args)
        {
            string path = "";
            var modList = new List<Mod>();
            var modComparer = new ModComparer();

            var nonNexusList = new List<NonNexusMod>();
            var nonNexusComparer = new NonNexusModComparer();

            Console.WriteLine("Path to mods csv file: (or nothing to continue)");
            path = Console.ReadLine();

            while (!string.IsNullOrWhiteSpace(path))
            {
                if (!File.Exists(path))
                    Console.WriteLine($"File does not exist: {path}");
                else
                {
                    var fileName = Path.GetFileNameWithoutExtension(path);
                    using (var reader = new StreamReader(path))
                    {
                        using (var csvReader = new CsvReader(reader, new CsvConfiguration(CultureInfo.CurrentCulture)))
                        {
                            var records = csvReader.GetRecords<Mod>()
                                .Where(m =>
                                {
                                    if (string.IsNullOrWhiteSpace(m.NexusUrl))
                                    {
                                        Console.ForegroundColor = ConsoleColor.Yellow;
                                        Console.WriteLine($"Non-Nexus mod:\t{m.Name}");
                                        nonNexusList.Add(new NonNexusMod(m.Name, fileName));
                                        return false;
                                    }
                                    else if (modList.Contains(m, modComparer))
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine($"Duplicate mod:\t{m.Name}");
                                        return false;
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.WriteLine($"Adding mod:\t{m.Name}");
                                        return true;
                                    }
                                });


                            modList.AddRange(records);
                        }
                    }
                }

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Path to mods csv file: (or nothing to continue)");
                path = Console.ReadLine();
            }

            modList.Sort(modComparer);
            nonNexusList.Sort(nonNexusComparer);

            var html = File.ReadAllText(TEMPLATE_FILE_NAME);
            var modTemplateStart = html.IndexOf(LIST_TEMPLATE_START) + LIST_TEMPLATE_START.Length;
            var modTemplateLength = html.IndexOf(TEMPLATE_END, modTemplateStart) - modTemplateStart;
            var modTemplate = html.Substring(modTemplateStart, modTemplateLength);

            var modListStr = new StringBuilder();
            modList.ForEach(m => modListStr.Append(string.Format(modTemplate, m.Name, m.NexusUrl, m.Version)));
            html = html.Replace($"{LIST_TEMPLATE_START}{modTemplate}{TEMPLATE_END}", modListStr.ToString());

            var nonNexusTemplateStart = html.IndexOf(NON_NEXUS_LIST_TEMPLATE_START) + NON_NEXUS_LIST_TEMPLATE_START.Length;
            var nonNexusTemplateLength = html.IndexOf(TEMPLATE_END, nonNexusTemplateStart) - nonNexusTemplateStart;
            var nonNexusTemplate = html.Substring(nonNexusTemplateStart, nonNexusTemplateLength);

            var nonNexusListStr = new StringBuilder();
            nonNexusList.ForEach(m => nonNexusListStr.Append(string.Format(nonNexusTemplate, m.File, m.Name)));
            html = html.Replace($"{NON_NEXUS_LIST_TEMPLATE_START}{nonNexusTemplate}{TEMPLATE_END}", nonNexusListStr.ToString());

            html = html.Replace(MOD_COUNT_TEMPLATE_START, modList.Count().ToString());

            File.WriteAllText(OUTPUT_FILE_NAME, html);

            Process.Start(new ProcessStartInfo
            {
                FileName = OUTPUT_FILE_NAME,
                UseShellExecute = true
            });
        }
    }
}

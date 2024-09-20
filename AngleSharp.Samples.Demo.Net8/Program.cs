﻿namespace AngleSharp.Samples.Demos
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    class Program
    {
        static IEnumerable<ISnippet> FindSnippets()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var alltypes = assembly.GetTypes();
            var types = alltypes.Where(m => m.GetInterfaces().Contains(typeof(ISnippet)));
            return types.Select(m => m.GetConstructor(Type.EmptyTypes).Invoke(null) as ISnippet);
        }

        static void Main(String[] args)
        {
            var snippets = FindSnippets().Select(m => new
            {
                Name = m.GetType().Name,
                Run = (Func<Task>)m.Run
            }).ToList();
            var defaults = new
            {
                pause = false,
                clear = false,
                selected = new[] { "CustomEventScripting" },
                //selected = snippets.Select(m => m.Name).ToArray(),
            };
            var usepause = args.Contains("-p") || args.Contains("--pause") || defaults.pause;
            var clearscr = args.Contains("-c") || args.Contains("--clear") || defaults.clear;
            var pause = Switch(usepause, PauseConsole);
            var clear = Switch(clearscr, ClearConsole);

            RunSynchronously(async () =>
            {
                foreach (var snippet in snippets)
                {
                    if (defaults.selected.Contains(snippet.Name))
                    {
                        Console.WriteLine(">>> {0}", snippet.Name);
                        Console.WriteLine();

                        await snippet.Run();

                        Console.WriteLine();

                        pause();
                        clear();
                    }
                }
            });
        }

        static Action Switch(Boolean condition, Action active)
        {
            return condition ? active : () => { };
        }

        static void RunSynchronously(Func<Task> runner)
        {
            runner().Wait();
        }

        static void ClearConsole()
        {
            Console.Clear();
        }

        static void PauseConsole()
        {
            Console.WriteLine("Press any key to continue ...");
            Console.ReadKey(true);
        }
    }
}

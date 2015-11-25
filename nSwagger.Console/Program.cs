﻿namespace nSwagger.Console
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal class Program
    {
        static void Main(string[] args)
        {
            args = new[] { "/F", "..\\..\\..\\tests\\bookclubbing.json", "generated.cs" };
            Console.Title = "nSwagger.Console";
            Console.CursorVisible = false;
            Console.WriteLine("nSwagger.Console");
            if (args.Length < 2)
            {
                ShowHelp();
                return;
            }

            var sources = new List<string>();
            var target = args.Last();
            var allowOverride = false;
            var @namespace = "";
            for (var count = 0; count < args.Length-1; count++)
            {
                var argument = args[count];
                if (argument.Equals("/F", StringComparison.OrdinalIgnoreCase))
                {
                    allowOverride = true;
                    continue;
                }

                if (argument.Equals("/NAMESPACE", StringComparison.OrdinalIgnoreCase))
                {
                    count++;
                    @namespace = args[count];
                    continue;
                }

                sources.Add(argument);
            }

            if (sources.Count == 0)
            {
                Console.WriteLine("No source files found.");
                ShowHelp();
                return;
            }

            if (string.IsNullOrWhiteSpace(target))
            {
                Console.WriteLine("No target file found.");
                ShowHelp();
                return;
            }

            if (File.Exists(target))
            {
                if (!allowOverride)
                {
                    Console.WriteLine("Target file already exists and cannot be overwritten.");
                    ShowHelp();
                    return;
                }
                else
                {
                    File.Delete(target);
                }
            }

            var engine = Engine.Run(sources.ToArray());

            engine.Wait();

            var config = new Configuration();
            if (!string.IsNullOrWhiteSpace(@namespace))
            {
                config.Namespace = @namespace;
            }

            var ns = Generator.Begin(config);
            foreach (var spec in engine.Result)
            {
                ns = Generator.Go(ns, config, spec);
            }

            Generator.End(config, ns, target);
            Console.WriteLine("Finished producing code: " + target);
            Console.CursorVisible = true;
        }

        private static void WriteMessage(int offset, string message)
        {
            var fits = Console.WindowWidth >= offset + message.Length;
            if (fits)
            {

                return;
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Takes one or more Swagger specifications and produces a C# output.");
            Console.WriteLine();
            Console.WriteLine("nSwagger.Console.exe [/F] [/NAMESPACE namespace] sources target");
            Console.WriteLine();
            var messages = new Dictionary<string, string>
            {
                { "source", "One or more, space seperated, paths to Swagger definations. The paths can exist on disk or be a URL." },
                { "target", "The target to write the C# output to. NOTE: This MUST be last." },
                { "/F", "Force override of the target if it already exists." },
                { "/NAMESPACE", "The namespace for the target file to use.The namespace for the target file to use." },
            };

            WriteAlignedMessages(messages);

            Console.WriteLine("  Examples:");
            Console.WriteLine();
            var examples = new Dictionary<string, string>
            {
                {"\"nSwagger.Console http://demo.demo/docs/v1 demo.cs\"","This will take the Swagger specification from http://demo.demo/docs/v1 and produce a C# file named demo in the current folder." },
                {"\"nSwagger.Console swagger.json demo.cs\"","This will take the Swagger specification from the local swagger.json file and produce a C# file named demo in the current folder." }
            };

            WriteAlignedMessages(examples, 4);
        }

        private static void WriteAlignedMessages(Dictionary<string, string> messages, int keyOffset = 2)
        {
            var valueOffSet = messages.OrderByDescending(_ => _.Key.Length).First().Key.Length + 2 + keyOffset;
            var maxMessageLength = Console.WindowWidth - valueOffSet - 2;
            foreach (var message in messages)
            {
                Console.CursorLeft = 0;
                Console.Write(message.Key.PadLeft(keyOffset + message.Key.Length, ' '));
                Console.CursorLeft = valueOffSet;

                if (message.Value.Length > maxMessageLength)
                {
                    var print = "";
                    var words = message.Value.Split(' ');
                    for (var counter = 0; counter < words.Length; counter++)
                    {
                        if (print.Length + 1 + words[counter].Length > maxMessageLength)
                        {
                            Console.CursorLeft = valueOffSet;
                            Console.WriteLine(print);
                            print = "";
                        }

                        print += words[counter] + " ";
                    }

                    Console.CursorLeft = valueOffSet;
                    Console.WriteLine(print);

                }
                else
                {
                    Console.WriteLine(message.Value);
                }
            }

            Console.CursorLeft = 0;
        }
    }
}
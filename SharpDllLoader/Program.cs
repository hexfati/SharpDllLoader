using System;
using CommandLine;
using System.Reflection;

namespace SharpDllLoader
{
    public class Options
    {

        [Option('d', "dll", Required = true, HelpText = "DLL path")]
        public string Dll { get; set; }

        [Option('n', "namespace", Required = false, HelpText = "Namespace of the library [optional]")]
        public string Namespace { get; set; }

        [Option('c', "class", Required = true, HelpText = "Class to invoke")]
        public string Class { get; set; }

        [Option('m', "method", Required = true, HelpText = "Method to invoke")]
        public string Method { get; set; }

        [Option('a', "args", Required = false, HelpText = "Method to invoke")]
        public string Args { get; set; }
    }

    static class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            ParserResult<Options> result = Parser.Default.ParseArguments<Options>(args);
            if (result.Tag == ParserResultType.Parsed)
            {
                var options = ((Parsed<Options>)result).Value;
                string filepath = options.Dll;
                string ns = options.Namespace;
                string c = options.Class;
                string m = options.Method;
                string[] arguments = null;

                if(options.Args != null) { 
                    arguments = options.Args.Split();
                }

                Assembly assembly = Assembly.LoadFile(filepath);
                Type type = null;
                if (ns == null)
                {
                    type = assembly.GetType(c);
                }
                else
                {
                    type = assembly.GetType(ns + "." + c);
                }

                if (type != null)
                {
                    var cl = Activator.CreateInstance(type);
                    var method_obj = type.GetMethod(m);
                    if (method_obj != null)
                    {
                        ParameterInfo[] declaredparams = method_obj.GetParameters();
                        object[] tmp = null;
                        if (declaredparams.Length > 0) { 
                            if(arguments != null && declaredparams.Length == arguments.Length)
                            {
                                tmp = new object[arguments.Length];
                                for (int i=0; i<arguments.Length; i++)
                                {                                
                                    if(declaredparams[i].ParameterType.ToString() == "System.Int32")
                                    {
                                        int number;
                                        bool flag = Int32.TryParse((string) arguments[i], out number);
                                        if (flag) {
                                            tmp[i] =  number;
                                        }
                                        else
                                        {
                                            Console.WriteLine("Arguments type mismatch!");
                                            System.Environment.Exit(1);
                                        }
                                    }
                                    else
                                    {
                                        // assumes it is a string
                                        tmp[i] = arguments[i];
                                    }
                                }
                            }
                            else {
                                Console.WriteLine("Arguments mismatch!");
                                System.Environment.Exit(1);
                            }
                        }

                        method_obj.Invoke(cl, tmp);
                    }
                    else
                    {
                        Console.WriteLine("Method not found");
                    }
                }
                else
                {
                    Console.WriteLine("Class or namespace not found");
                }
            }
            else
            {
                System.Environment.Exit(1);
            }

        }
    }
}

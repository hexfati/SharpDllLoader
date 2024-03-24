using System;
using CommandLine;
using System.Reflection;
using System.Collections;

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

        [Option('s', "static", Required = false, Default = false, HelpText = "StaticMethod flag")]
        public bool IsStaticMethod { get; set; }

        [Option("margs", Required = false, HelpText = "Method args separated by space")]
        public string MethodArgs { get; set; }

        [Option("cargs", Required = false, HelpText = "Class args separated by space")]
        public string ClassArgs { get; set; }
    }

    static class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            
            Options options = ParseArguments(args);
            Type type = GetTypeFromAssembly(options.Dll, options.Namespace, options.Class);
            bool IsStaticMethod = options.IsStaticMethod;
            object class_obj = null;
            if (IsStaticMethod == false)
            {   
                if (options.ClassArgs == null)
                {
                    class_obj = GetClass(type);
                }
                else
                {
                    ParameterInfo[] declaredConstructorParams = type.GetConstructors()[0].GetParameters();
                    object[] classParamArray = MapParams(declaredConstructorParams, options.ClassArgs);
                    class_obj = GetClass(type, classParamArray);
                }
            }
            MethodInfo methodObj = GetMethod(type, options.Method);
            ParameterInfo[] declaredMethodParams = methodObj.GetParameters();
            object[] methodParamArray = MapParams(declaredMethodParams, options.MethodArgs);
            methodObj.Invoke(class_obj, methodParamArray);
        }

        private static Options ParseArguments(string[] arguments)
        {
            ParserResult<Options> result = Parser.Default.ParseArguments<Options>(arguments);
            if (result.Tag == ParserResultType.Parsed)
            {
                return ((Parsed<Options>)result).Value;
            }
            else
            {
                IEnumerator enumerator = result.Errors.GetEnumerator();
                enumerator.MoveNext();
                Error currentError = (Error) enumerator.Current;
                String error = currentError.ToString();
                System.Windows.Forms.MessageBox.Show("Error parsing argument: " + error);
                System.Environment.Exit(1);
                return null;
            }
        }

        private static Type GetTypeFromAssembly(string filepath, string nspace, string classname)
        {
            Assembly assembly = Assembly.LoadFile(filepath);
            Type type = null;
            if (nspace == null)
            {
                type = assembly.GetType(classname);
            }
            else
            {
                type = assembly.GetType(nspace + "." + classname);
            }

            if (type != null)
            {
                return type;
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Class or namespace not found");
                System.Environment.Exit(1);
                return null;
            }
        }
        private static object GetClass(Type type)
        {
            object classObj = null;
            if (type.IsAbstract == false)
            {
                classObj = Activator.CreateInstance(type);
            }
            return classObj;
        }

        private static object GetClass(Type type, object[] classParamArray)
        {
            object classObj = null;
            if (type.IsAbstract == false)
            {
                classObj = Activator.CreateInstance(type, classParamArray);
            }
            return classObj;
        }
        

        private static MethodInfo GetMethod(Type type, string methodname)
        {
            MethodInfo methodObj = type.GetMethod(methodname);
            if (methodObj != null)
            {
                return methodObj;
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Method not found");
                System.Environment.Exit(1);
                return null;
            }
        }

        private static object[] MapParams(ParameterInfo[] declaredparams, string argumentString)
        {
            string[] arguments = null;

            if (argumentString != null)
            {
                arguments = argumentString.Split();
            }

            object[] tmp = null;
            if (declaredparams.Length > 0)
            {
                if (arguments != null && declaredparams.Length == arguments.Length)
                {
                    tmp = new object[arguments.Length];
                    for (int i = 0; i < arguments.Length; i++)
                    {
                        if (declaredparams[i].ParameterType.ToString() == "System.Int32")
                        {
                            int number;
                            bool flag = Int32.TryParse((string)arguments[i], out number);
                            if (flag)
                            {
                                tmp[i] = number;
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
                    return tmp;
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Arguments mismatch");
                    System.Environment.Exit(1);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}

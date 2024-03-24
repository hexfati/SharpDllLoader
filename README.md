# SharpDllLoader
A simple C# executable that invokes an arbitrary method of an arbitrary C# DLL. **The project is useful to analyze malicious C# DLL through the popular tool dnSpy.** dnSpy does not allow to native debug a DLL because of the absence of a standard entrypoint. So you can debug the SharpDllLoader executable with proper parameters to jump into the specified method of the DLL that you want dynamically analyze.

### Update v2.0
The tool now supports Static Methods and Class Constructors.

## Usage

Import the executable SharpDllLoader.exe in dnSpy and click on Start.
Fill the form with the right parameters.

The supported parameters are:

`-d DLL_PATH [-n NAMESPACE] -c DLL_CLASS [--cargs "ARG1 ARG2"] -m METHOD [--margs "ARG1 ARG2"] [-s]`

The parameters delimited by `[]` are optional.

Parameters description:

`-d` Filepath to the DLL you want to debug.

`-n` Namespace containing the class you want to debug.

`-c` Name of the class you want to debug.

`--cargs` Class Constructor arguments (the tool supports only `int` and `strings` arguments).

`-m` Name of the method you want to debug.

`--margs` Method arguments (the tool supports only `int` and `strings` arguments).

`-s` Static flag; you have to use it if you want to debug a Static Method.


## Example of usage
Analyzing a malware you are faced with a DLL written in C# easily inspectable using some common tools like dnSpy. You know that the malware invokes (i.e. through a previous Powershell stage) the method `Bypass` of the class `Amsi`, but you're not able to debug the method to undestand what it does.
So, open SharpDllLoader.exe in your dnSpy instance, insert the correct parameters into Arguments field and select `Stop At: Entrypoint`.

[![](https://github.com/hexfati/SharpDllLoader/raw/master/images/1.png)](https://github.com/hexfati/SharpDllLoader/raw/master/images/1.png)

This is the entrypoint of the exe.

[![](https://github.com/hexfati/SharpDllLoader/raw/master/images/2.png)](https://github.com/hexfati/SharpDllLoader/raw/master/images/2.png)

Set a breakpoint on the illustrated instruction, or step over using F10 until you reach this instruction. Then click F11 to step into the invocation.

[![](https://github.com/hexfati/SharpDllLoader/raw/master/images/3.png)](https://github.com/hexfati/SharpDllLoader/raw/master/images/3.png)

F11 again

[![](https://github.com/hexfati/SharpDllLoader/raw/master/images/4.PNG)](https://github.com/hexfati/SharpDllLoader/raw/master/images/4.PNG)

Now click F10

[![](https://github.com/hexfati/SharpDllLoader/raw/master/images/5.PNG)](https://github.com/hexfati/SharpDllLoader/raw/master/images/5.PNG)

And again F10 until you reach the return instruction (illustrated). Then F11

[![](https://github.com/hexfati/SharpDllLoader/raw/master/images/6.PNG)](https://github.com/hexfati/SharpDllLoader/raw/master/images/6.PNG)

The same story: click F10 until the return instruction, then F11

[![](https://github.com/hexfati/SharpDllLoader/raw/master/images/8.PNG)](https://github.com/hexfati/SharpDllLoader/raw/master/images/8.PNG)

Finally you are in the desired method, you can proceed with debug as you know how.

[![](https://github.com/hexfati/SharpDllLoader/raw/master/images/9.PNG)](https://github.com/hexfati/SharpDllLoader/raw/master/images/9.PNG)

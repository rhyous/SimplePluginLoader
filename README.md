# SimplePluginLoader

A project that makes it easier to add plugins to C# applications.


Create a projects that loads plugin with one interface and two line of code. It is easy, just use three steps:

Step 1 - Create your plugin interface:

````
public interface IMyPlugin 
{
    string Name { get; set; }
    void DoSomething();
}
````

Step 2 - Load the plugins from a plugins directory in two lines of code:

````
    var pluginLoader = new PluginLoader<IMyPlugin>();
    var plugins = pluginLoader.LoadPlugins();
````

Step 3 - Do something with your plugins.
This loads the dlls. Now, each dll could have multiple plugins, so we get all the plugins from all the dlls like this.

````
    foreach (var plugun in plugins.SelectMany(plugin => plugin.PluginObjects))
    {
      plugin.DoSomething();
    }
````

Yeah. I know. All the complexity of plugins just disappeared. Your welcome!

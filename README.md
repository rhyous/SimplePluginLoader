# SimplePluginLoader

A project that makes it easier to add plugins to C# applications.


Create a projects that loads plugin with one interface and two line of code. It is easy, just use three steps:

Step 1 - Create your plugin interface:

````
public interface IMyObject 
{
    string Name { get; set; }
    void DoSomething();
}
````

Step 2 - Load the plugins from a plugins directory in two lines of code:

````
    var pluginLoader = new PluginLoader<IMyObject>();
    var plugins = pluginLoader.LoadPlugins();
````

Step 3 - Do something with your plugins.
This loads the dlls. Now, each dll could have multiple plugins, so we get all the plugins from all the dlls like this.

````
    foreach (var plugin in plugins)
    {
      foreach (var pluginObj in plugin.Objects) 
      {
        plugin.DoSomething();
      }
    }
````

Note: you could short this with linq if you want. Like this:

````
    foreach (var plugin in plugins.SelectMany(plugin => plugin.PluginObjects))
    {
      plugin.DoSomething();
    }
````

Yeah. I know. All the complexity of plugins just disappeared. Your welcome!

## Handling Dependencies ##

A plugin may have dependencies of it's own. By default these are not loaded.

````
    foreach (var plugin in plugins)
    {
      plugin.AddDependencyResolver();
      foreach (var pluginObj in plugin.Objects) 
      {
        plugin.DoSomething();
      }
      plugin.RemoveDependencyResolver();
    }
````

## Finding a plugin ##

You can require that plugins implement the very simple IName interface.  Then you can easily find one plugin and use it.

```
    using (var pluginFinder = new PluginFinder<IMyObject>()) 
    {
      var plugin = pluginFinder.FindPlugin("MyPluginName", @"c:\my\path\to\the\plugins");
      plugin.DoSomething();
    }
```

---
title: Getting Started
layout: default
has_children: true
nav_order: 0
---
# Creating a BSMLResourceViewController
A BSMLResourceViewController is simply a view controller that will load a BSML template from an embedded resource in your project, it's the simplest and most common way to make use of the library.
```csharp
public class ExampleViewController : BSMLResourceViewController
{
    public override string ResourceName => "Path.To.Resource.example.bsml";
}
```
# Making your template
Create a file such as `example.bsml` and add it somewhere in your project. To stay organized I recommend making a Views folder for all your BSML templates. Make sure to add it as an embedded resource so that it is included when you compile your plugin. Once you've done this if you're using a BSMLResourceViewController make sure to point it to your resource it should look something like `Namespace.Views.example.bsml`.

# Adding content to your template
When creating your templates you must only have one root node. Let's make a template with a header, some text, and a button.
```xml
<vertical child-control-height='false'>
  <horizontal bg='panel-top' pad-left='10' pad-right='10' horizontal-fit='PreferredSize'>
    <text text='Example' align='Center' font-size='10'></text>
  </horizontal>
  <horizontal bg='round-rect-panel' pad='8'>
    <text text='Looks at this fancy text.'></text>
  </horizontal>
  <button text='Test Button'></button>
</vertical>
```
The following will look like this in game
![](https://i.imgur.com/NYEakzd.png)
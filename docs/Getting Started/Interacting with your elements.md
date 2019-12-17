---
title: Interacting with your elements
layout: default
parent: Getting Started
---
# Accessing components in code
Say we want to change that text we made earlier in code, well to do that we're gonna need a reference to it. First, we're gonna need to give that text an `id`, let's give it the id `some-text`
```xml
<vertical child-control-height='false'>
  <horizontal bg='panel-top' pad-left='10' pad-right='10' horizontal-fit='PreferredSize'>
    <text text='Example' align='Center' font-size='10'></text>
  </horizontal>
  <horizontal bg='round-rect-panel' pad='8'>
    <text id='some-text' text='Looks at this fancy text.'></text>
  </horizontal>
  <button text='Test Button'></button>
</vertical>
```
Now to access the `TextMeshProUGUI` component of this text element we can just tag it with the `UIComponent` attribute
```csharp
public class ExampleViewController : BSMLResourceViewController
{
    public override string ResourceName => "Namespace.Views.example.bsml";

    [UIComponent("some-text")]
    private TextMeshProUGUI text;
}
```

# Triggering methods with templates
Now we have our text but how do we make clicking the button change it? Well, we can add an `on-click` event to it to trigger a method with the `UIAction` attribute. Let's make it trigger the `press` method
```xml
<vertical child-control-height='false'>
  <horizontal bg='panel-top' pad-left='10' pad-right='10' horizontal-fit='PreferredSize'>
    <text text='Example' align='Center' font-size='10'></text>
  </horizontal>
  <horizontal bg='round-rect-panel' pad='8'>
    <text id='some-text' text='Looks at this fancy text.'></text>
  </horizontal>
  <button on-click='press' text='Test Button'></button>
</vertical>
```
and now we can add the `UIAction` attribute to our method that changes the text
```csharp
public class ExampleViewController : BSMLResourceViewController
{
    public override string ResourceName => "Namespace.Views.example.bsml";

    [UIComponent("some-text")]
    private TextMeshProUGUI text;

    [UIAction("press")]
    private void ButtonPress()
    {
        text.text = "Hey look, the text changed";
    }
}
```
Now if we go in game and click the button our text will change.
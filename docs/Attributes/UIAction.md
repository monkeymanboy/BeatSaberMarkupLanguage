---
title: UIAction
layout: default
parent: Attributes
---
# About
The UIAction attribute can only be applied to methods. The parameters of the method you are applying it to depends on what you are actually using that action for.

You can also make a UIAction respond to events by making it's id a # followed by the event you want it to respond to. Events will never have any parameters.

# Example Usage
A button can take in a UIAction for it's on-click
```xml
<button text='Button' on-click='click'></button>
```
```csharp
[UIAction("click")]
private void ButtonClicked()
{
    Console.WriteLine("Button was clicked!");
}
```
This example will print `Button was clicked` to the console whenever the button is clicked.
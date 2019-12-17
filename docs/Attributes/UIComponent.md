---
title: UIComponent
layout: default
parent: Attributes
---
# About
The UIComponent attribute can only be applied to fields. The type of the field that it is applied to must be one of the components on the object with the corresponding id.

# Example Usage
```xml
<text id='whatever'></text>
```
```csharp
[UIComponent("whatever")]
private TextMeshProUGUI textComponent;
```
This example will initialize `textComponent` with the text tag's `TextMeshProUGUI` component.
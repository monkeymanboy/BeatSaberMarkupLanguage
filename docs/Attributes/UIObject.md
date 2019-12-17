---
title: UIObject
layout: default
parent: Attributes
---
# About
The UIObject attribute can only be applied to fields. The type of the field must be `GameObject`. This tag is used in the same way as UIComponent but instead will give you the `GameObject`.

# Example Usage
```xml
<text id='whatever'></text>
```
```csharp
[UIComponent("whatever")]
private GameObject textObj;
```
This example will initialize `textObj` with the text tag's `GameObject` component.
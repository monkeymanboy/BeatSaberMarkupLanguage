---
title: UIValue
layout: default
parent: Attributes
---
# About
The UIValue attribute can be applied to both fields and properties.

Besides being able to supply tag parameters that accept UIValue's you can also use them for any tag parameter by prefixing the id with a ~ on the actual tag property. This will place the corresponding UIValue's ToString respresentation into the tag parameter.

# Example Usage
All settings tags make use of UIValue so let's look at a bool-setting
```xml
<bool-setting text='Bool Setting' value='bool-value' apply-on-change='true'></bool-setting>
```
```csharp
[UIValue("bool-value")]
private bool boolVal = true;
```
In this example the `bool-setting` will start as `ON` and as soon as it get's switched to `OFF` boolVal will be set to `false`

## Using ~
Say we have for example a text component but we want it's text to be some string we have in code. Well then we can make use of the ~ prefix.
```xml
<text text='~some-text'></text>
```
```csharp
[UIValue("some-text")]
private string someText = "Text will come from here!";
```
In this example the text's text will become `Text will come from here!`
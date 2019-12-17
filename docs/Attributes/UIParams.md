---
title: UIParams
layout: default
parent: Attributes
---
# About
The UIParams attribute can only be applied to a field of type BSMLParserParams.

Receiving this object can be useful to emit ui events or actions through your own code.

# Example Usage
```csharp
[UIParams]
BSMLParserParams parserParams;
```
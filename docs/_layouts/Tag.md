---
title: {{page.type}}
layout: default
parent: Tags
---
# {{page.type}}

# Components and Properties
{% for component in page.components %}
{% assign typehandler = site.data.TypeHandlers | where: "type", component | first %}
## {{typehandler.type}}
{% for property in typehandler.properties %}
### {{property.aliases[0]}}
{{property.type}}
{: .label .label-green }
{{property.description}}
{% if property.aliases.size > 1 %}
#### Aliases:
{% for alias in property.aliases %}
* {{alias}}
{: .fs-3 }
{% endfor %}
{% endif %}
{% endfor %}
{% endfor %}

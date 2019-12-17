---
title: {{page.type}}
layout: default
parent: Handled Components
---
# {{page.type}}

{% for property in page.properties %}
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

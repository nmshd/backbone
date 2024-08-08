{{/*
Expand the name of the chart.
*/}}
{{- define "global.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "global.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Common labels
*/}}
{{- define "global.labels" -}}
helm.sh/chart: {{ include "global.chart" . }}
{{ include "global.selectorLabels" . }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end }}

{{/*
Selector labels
*/}}
{{- define "global.selectorLabels" -}}
app.kubernetes.io/name: {{ include "global.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end }}


{{/*
Converts a list of environment variables to a map
*/}}
{{- define "listToMap" -}}
  {{- $list := . -}}
  {{- $map := dict -}}
  {{- range $item := $list -}}
    {{- $map = set $map $item.name $item -}}
  {{- end -}}
  {{- toYaml $map -}}
{{- end -}}

{{/*
Converts a map of environment variables back to a list
*/}}
{{- define "mapToList" -}}
  {{- $map := . -}}
  {{- $list := list -}}
  {{- range $key, $value := $map -}}
    {{- $list = append $list $value -}}
  {{- end -}}
  {{- toYaml $list -}}
{{- end -}}

{{/*
This function merges global and resource-specific environment variables for your Helm chart.

The function takes two arguments:
    - arg0: a list of the global environment variables defined in the chart's values file (.Values.global.env)
    - arg1: a list of the resource-specific environment variables (.Values.{resource}.env)

Usage:  {{ include "mergeEnvValues" (list .Values.global.env .Values.{resource}.env) }}

In the example above, replace "{resource}" with the name of the specific resource where you want to apply the merged 
environment variables.

It should be noted that arg1 takes precedence over arg0. Any keys existing in both lists (arg0 and arg1) will have 
their values overridden by the key-value pairs from arg1. This allows resource-specific settings to override global 
settings.
*/}}
{{- define "mergeAndRenderEnv2" -}}
{{- $arg0 := index . 0 }}
{{- $arg1 := index . 1 }}

{{- $globalEnvMap := include "listToMap" $arg0 | fromYaml }}
{{- $adminuiEnvMap := include "listToMap" $arg1 | fromYaml }}
{{- $mergedEnvMap := merge $adminuiEnvMap $globalEnvMap}}

{{- range $item := $mergedEnvMap }}
            - name: {{ $item.name }}
                {{- if $item.value }}
              value: {{ $item.value }}
                {{- else if $item.valueFrom }}
              valueFrom:
                secretKeyRef:
                  name: {{ $item.valueFrom.secretKeyRef.name }}
                  key: {{ $item.valueFrom.secretKeyRef.key }}
            {{- end }}
            {{- end }}

{{- end }}



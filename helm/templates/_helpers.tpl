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

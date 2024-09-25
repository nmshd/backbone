@Integration
Feature: GET /RelationshipTemplates

User requests Relationship Templates

	Scenario Outline: GET Multiple
		Given Identities i1, i2, i3 and i4
		And Relationship Templates with the following properties
			| rTempName | rTempOwner | forIdentity | password |
			| rt1       | i1         | -           | -        |
			| rt2       | i2         | -           | -        |
			| rt3       | i1         | -           | -        |
			| rt4       | i2         | -           | -        |
			| rt5       | i1         | -           | password |
			| rt6       | i1         | -           | password |
			| rt7       | i2         | -           | password |
			| rt8       | i2         | -           | password |
			| rt9       | i1         | i1          | -        |
			| rt10      | i2         | i3          | -        |
			| rt11      | i2         | i2          | -        |
			| rt12      | i2         | i3          | -        |
			| rt13      | i2         | i3          | password |
			| rt14      | i2         | i3          | password |
			| rt15      | i2         | i3          | password |
		When <activeIdentity> sends a GET request to the /RelationshipTemplate endpoint with the following payloads
			| rTempName | passwordOnGet |
			| rt1       | -             |
			| rt2       | -             |
			| rt3       | password      |
			| rt4       | password      |
			| rt5       | password      |
			| rt6       | -             |
			| rt7       | password      |
			| rt8       | -             |
			| rt9       | -             |
			| rt10      | -             |
			| rt11      | -             |
			| rt12      | -             |
			| rt13      | password      |
			| rt14      | wordpass      |
			| rt15      | password      |
		Then the response contains <itemCount> Relationship Template(s)

		Examples:
		| activeIdentity | itemCount |
		| i1             | 1         |
		| i2             | 1         |
		| i3             | 1         |
		| i4             | 1         |

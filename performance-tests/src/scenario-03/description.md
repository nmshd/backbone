# 3. Entering into a relationship between app with new identity and connector with existing identity

Steps:

1.  Identity `I1` (e.g. Connector) creates a relationship template (content size: `1kB`).
2.  Wait 60 seconds (simulates user behavior).
3.  Identity `I2` is created (e.g. user).
4.  `I2` pushes 40 data wallet modifications (size of the content is `300B` each).
5.  Wait 5 seconds (simulates user behavior).
6.  `I2` fetches the relationship template from `I1`.
7.  `I2` pushes 5 data wallet modifications (content size is `300B` each).
8.  Wait 60 seconds (simulates user behavior).
9.  `I2` creates a relationship with this template (content size: `1kB`).
10. `I2` pushes 5 data wallet modifications (size of the content is `300B` each).
11. Wait 2 seconds (simulates time between two syncs of the connector).
12. `I1` performs a sync run and thereby receives the ID of the relationship.
13. `I1` uses the ID of the relationship to retrieve the relationship.
14. Wait 3 seconds (simulates a customer system that has to make the decision).
15. `I1` accepts the relationship.
16. Wait 20 seconds (simulates user behavior).
17. `I2` performs a sync run and receives the ID of the updated relationship.
18. `I2` uses the ID of the relationship to retrieve the relationship.
19. `I2` pushes 5 data wallet modifications (size of the content is 300B each).

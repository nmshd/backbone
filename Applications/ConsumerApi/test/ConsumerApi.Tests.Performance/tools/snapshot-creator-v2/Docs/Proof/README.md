# Proof of Correctness for Snapshot Creator v2

This folder contains proof of the correctness of the Snapshot Creator v2.

Each sub-folder includes a generated snapshot containing the input pool-configuration file and an Excel file per 
entity domain model (except `Devices.Identities` and `Devices.Devices`, which are in a single file) containing the 
created records in the `enmeshed` database for verification.

The proof involves comparing the generated snapshot with the expected snapshot found in the pool-configuration file (e.g., `.\PoolConfig\pool-config.test.xlsx`).

The proof is conducted for the following default performance test cases:

- Test
- Light
- Heavy

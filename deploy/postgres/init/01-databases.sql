-- One database per service. A single PostgreSQL instance is a deliberate MVP
-- simplification: the databases are logically independent, no service reads
-- another service's tables, and they can be split onto separate instances
-- without touching application code.

CREATE DATABASE warmhouse_identity;
CREATE DATABASE warmhouse_devices;
CREATE DATABASE warmhouse_heating;
CREATE DATABASE warmhouse_lighting;
CREATE DATABASE warmhouse_gates;
CREATE DATABASE warmhouse_monitoring;
CREATE DATABASE warmhouse_telemetry;
CREATE DATABASE warmhouse_scenarios;
CREATE DATABASE warmhouse_notifications;

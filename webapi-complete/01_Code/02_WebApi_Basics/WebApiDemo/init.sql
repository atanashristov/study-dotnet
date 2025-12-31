-- PostgreSQL initialization script
-- This runs when the PostgreSQL container starts for the first time
-- Creates utility tables that are NOT managed by Entity Framework

-- Health check table for monitoring database connectivity
CREATE TABLE IF NOT EXISTS health_check (
    id SERIAL PRIMARY KEY,
    check_name VARCHAR(100) NOT NULL,
    status VARCHAR(20) NOT NULL,
    checked_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    details TEXT
);

-- Insert initial health check record
INSERT INTO health_check (check_name, status, details)
VALUES ('database_initialization', 'success', 'Database initialized successfully via init.sql');

-- Create a database info table for troubleshooting
CREATE TABLE IF NOT EXISTS database_info (
    id SERIAL PRIMARY KEY,
    info_key VARCHAR(100) NOT NULL,
    info_value TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Insert database initialization info
INSERT INTO database_info (info_key, info_value) VALUES
('postgresql_version', version()),
('initialized_at', CURRENT_TIMESTAMP::text),
('initialization_method', 'init.sql'),
('docker_container', 'postgres-webapi');

-- Function to update health check (useful for monitoring)
CREATE OR REPLACE FUNCTION update_health_check(check_name_param VARCHAR, status_param VARCHAR, details_param TEXT DEFAULT NULL)
RETURNS void AS $$
BEGIN
    INSERT INTO health_check (check_name, status, details)
    VALUES (check_name_param, status_param, details_param);
END;
$$ LANGUAGE plpgsql;

-- Grant permissions (if needed)
-- GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO postgres;

-- Initialize the WebApiDemo database
-- This script runs automatically when the PostgreSQL container starts for the first time
-- Database context is already set via POSTGRES_DB environment variable

-- Create any initial tables or data here if needed
-- For now, this is just a placeholder file
-- Example: Create a simple health check table
CREATE TABLE IF NOT EXISTS health_check (
  id SERIAL PRIMARY KEY,
  status VARCHAR(50) DEFAULT 'OK',
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
INSERT INTO health_check (status)
VALUES ('Database initialized successfully');

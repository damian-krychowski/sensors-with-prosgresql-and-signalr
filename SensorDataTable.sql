CREATE TABLE IF NOT EXISTS sensor_data (
    id uuid PRIMARY KEY,
    timestamp BIGINT NOT NULL,
    data JSONB NOT NULL
);
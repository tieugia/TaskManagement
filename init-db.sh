#!/bin/bash
/opt/mssql/bin/sqlservr &
echo "Waiting for SQL Server to be available..."
sleep 20
echo "Running initialization SQL..."
sqlcmd -S localhost -U sa -P 123456@abc -i /Script.sql
# Keep container running
wait
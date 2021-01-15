#Postgres
docker run -p 5432:5432 --name service_db -d --restart=always -e PGDATA=‘/var/lib/pgsql/data/’  postgres:11

connect: localhost:5432 
DB: postgres
user: postgres
password: postgres

CREATE DATABASE catalogs_db;
CREATE USER catalogs_db WITH PASSWORD 'catalogs_db';
GRANT ALL PRIVILEGES ON DATABASE test_db TO catalogs_db;

--ef migrations
-- install Microsoft.EntityFrameworkCore.Tools

Add-Migration Initial 
Remove-Migration 

Update-Database
Script-Migration

Add-Migration Initial -Project CatalogApi -StartUpProject CatalogApi -Context DefaultDbContext
Remove-Migration -Project CatalogApi -StartUpProject CatalogApi -Context DefaultDbContext


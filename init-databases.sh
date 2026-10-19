#!/bin/bash
set -e

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "postgres" <<-EOSQL
    CREATE DATABASE "AppointmentDb";
    CREATE DATABASE "AuthDb";
    CREATE DATABASE "DocumentsDb";
    CREATE DATABASE "NotificationHistoryDb";
    CREATE DATABASE "OfficesDbOutbox";
    CREATE DATABASE "ProfilesDb";
    CREATE DATABASE "ServicesDb";
EOSQL
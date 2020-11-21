# all is wrong

# docker container inspect mssql || docker start mssql 

# while [ ! docker container inspect mssql ]; do
#     echo "SQL database is not running. Waiting 1 second..."
#     sleep 1
#     echo "Trying to start the SQL database..."
#     docker start mssql 
# done

# dotnet api.dll
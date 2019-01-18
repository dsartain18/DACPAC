# DACPAC Database Updates

This project began from a need to copy a database to the same database server and specify the name of the new database. This project makes use of the Microsoft.SqlServer.Dac NuGet package to accomplish this.

There are comments in the code that explain what we need to do to copy the database with a new name, and what I *think* is necessary to update a database with the same name but on a different server for the sake of deployment across different environments.

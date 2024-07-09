rem Delete all files in the Migrations folder
del /Q Migrations\*.*

rem Delete the database
del app.db

rem Create a new migration
dotnet ef database update 0 --context ApplicationDbContext
dotnet ef migrations add Initialize

rem Update the database
dotnet ef database update
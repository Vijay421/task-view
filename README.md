# TaskView
A collaborative to-do list app.

## Backend setup

1. Copy `appsettings.json.dist` and rename the copy to: `appsettings.json`
1. Add the postgres credentials near `DefaultConnection`
1. Add the test dd credentials near `TestDbConnection`
1. Create the db with: `dotnet ef database update`

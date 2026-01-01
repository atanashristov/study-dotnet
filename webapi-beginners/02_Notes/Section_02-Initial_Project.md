# Section 02: Initial Project

## Lesson 02.11: Create Project

Create API project named _RoyalVillaApi_ and a solution:

```sh
dotnet new webapi -n RoyalVillaApi --use-controllers --framework net10.0
dotnet new sln -n RoyalVilla
dotnet sln ./RoyalVilla.sln add ./RoyalVillaApi/RoyalVillaApi.csproj
```

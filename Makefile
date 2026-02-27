SOLUTION=GoFish.sln

.PHONY: restore build test run docker-up docker-down

restore:
	dotnet restore $(SOLUTION)

build:
	dotnet build $(SOLUTION) -c Release --no-restore

test:
	dotnet test $(SOLUTION) -c Release --no-build

run:
	dotnet run --project src/GoFish.Api/GoFish.Api.csproj --no-launch-profile

docker-up:
	docker compose up --build

docker-down:
	docker compose down

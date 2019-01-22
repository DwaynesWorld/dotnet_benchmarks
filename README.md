# dotnet_benchmarks

dotnet run -c Release

## mac / linux

./bombardier -c 200 -n 600000 http://localhost:5000/api/values/

## pc

.\bombardier.exe -c 200 -n 600000 http://localhost:5000/api/values/

.\bombardier.exe -c 200 -n 600000 http://localhost:56862/api/values/

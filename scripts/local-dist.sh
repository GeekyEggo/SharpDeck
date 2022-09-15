SCRIPTPATH="$( cd -- "$(dirname "$0")" >/dev/null 2>&1 ; pwd -P )"

pushd $SCRIPTPATH/..

dotnet clean

dotnet pack -c Release -p:ContinuousIntegrationBuild=true -p:Version=$1
dotnet nuget push ./dist/*.nupkg -s Local --skip-duplicate
dotnet add ./examples/Counter/Counter.csproj package StreamDeck -v $1

dotnet restore

popd
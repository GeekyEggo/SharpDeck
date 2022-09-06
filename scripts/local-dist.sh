SCRIPTPATH="$( cd -- "$(dirname "$0")" >/dev/null 2>&1 ; pwd -P )"

pushd $SCRIPTPATH/..

dotnet clean
dotnet pack ./src/StreamDeck/StreamDeck.csproj -c Debug -p:Version=$1

# This must be registered as alocal  NuGet source.
mv ./dist/StreamDeck.* ~/AppData/Local/NuGet/Local/

pushd ./examples/Counter
dotnet add package StreamDeck -v $1
dotnet restore
popd

popd
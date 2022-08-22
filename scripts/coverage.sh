SCRIPTPATH="$( cd -- "$(dirname "$0")" >/dev/null 2>&1 ; pwd -P )"

dotnet test \
    -p:CollectCoverage=true \
    -p:CoverletOutput="$SCRIPTPATH/../tests/.coverage/" \
    -p:CoverletOutputFormat=cobertura \
    -p:Excludebyattribute="ExcludeFromCodeCoverage" \
    --no-build \
    --verbosity normal

 
reportgenerator \
    -reports:"$SCRIPTPATH/../tests/.coverage/coverage.cobertura.xml" \
    -targetdir:"$SCRIPTPATH/../tests/.coverage/report" \
    -reporttypes:Html

SCRIPTPATH="$( cd -- "$(dirname "$0")" >/dev/null 2>&1 ; pwd -P )"

pushd "$SCRIPTPATH/../tests/"
rm -rf .coverage
popd

dotnet test \
    -p:Exclude=\"[*.Tests]*\" \
    -p:Excludebyattribute="ExcludeFromCodeCoverage" \
    -p:CollectCoverage=true \
    -p:CoverletOutput="$SCRIPTPATH/../tests/.coverage/" \
    -p:CoverletOutputFormat=\"cobertura,json\" \
    -p:MergeWith="$SCRIPTPATH/../tests/.coverage/coverage.json" \
    -l:"console;verbosity=detailed" \
    --blame \
    --blame-hang-timeout:"10s"
    -m:1 \
    --no-build \

reportgenerator \
    -reports:"$SCRIPTPATH/../tests/.coverage/coverage.cobertura.xml" \
    -targetdir:"$SCRIPTPATH/../tests/.coverage/report" \
    -reporttypes:Html

start "$SCRIPTPATH/../tests/.coverage/report/index.html"
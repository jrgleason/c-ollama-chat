#!/bin/bash
# Script to run tests and generate coverage reports for both .NET and JS projects

echo "Starting coverage generation..."
echo "------------------------------------"

# --- JavaScript (client) coverage ---
echo "[JS] Navigating to client directory..."
cd ./client || exit 1

echo "[JS] Running npm run coverage..."
npm run coverage
if [ $? -ne 0 ]; then
    echo "[JS] ERROR: Coverage generation failed." >&2
else
    echo "[JS] JavaScript coverage report generated in client/coverage/"
fi

cd ..
echo "[JS] Returned to root directory."
echo "------------------------------------"
echo ""

# --- .NET (src) coverage ---
echo "[.NET] Generating .NET coverage report (src)..."
# Assuming the script is run from the root of the c-ollama-chat directory
dotnet test "./src.Tests/src.Tests.csproj" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
if [ $? -ne 0 ]; then
    echo "[.NET] ERROR: Coverage generation failed." >&2
else
    echo "[.NET] .NET coverage report (opencover.xml) should be in src.Tests/TestResults/ somewhere"
fi
echo "------------------------------------"
echo ""

echo "All coverage report generation attempts finished."

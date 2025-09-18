@echo off
set appId=%~1
set buildFolder=%~2
if "%buildFolder%"=="" set buildFolder=TicTac3D

start npx @yandex-games/sdk-dev-proxy -p ".artifacts/Web/%buildFolder%" -i="%appId%" -с
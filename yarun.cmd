@echo off
REM Запускаем прокси Яндекс Игр
start npx @yandex-games/sdk-dev-proxy -p ".artifacts/Web/%~1" -i="%~2" -с
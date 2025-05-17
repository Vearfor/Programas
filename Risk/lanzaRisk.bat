@echo off
::--------------------------------------------------------------------------
:: Lanzador de procesos Risk
::--------------------------------------------------------------------------
set mipwd=%cd%
set tiempo=15
echo ----------------------------------------------------------------------------
echo.
echo   mipwd %mipwd%
echo.

::--------------------------------------------------------------------------
:: No le da la gana de funcionar
:: for %A in (uno dos tres cuatro) do (
::    echo %A
:: )
::--------------------------------------------------------------------------
:: Lanzamos 4 comandos de Risk
:: Tenemos que probar si terminan interactuando entre ellos:
:: - les damos un parametro para que ellos internamente les den nombres
::   diferentes, y utilicen un fichero de log diferente cada uno
:: Metemos una espera de 1 o 2 segundos entre lanzamiento:
::--------------------------------------------------------------------------
::
:: Ya estamos en el directorio donde esta el ejecutable
:: cd  ..\..\misexes
::--------------------------------------------------------------------------

if "%1" == "uno" goto Uno
::--------------------------------------------------------------------------

if "%1" == "dos" goto Dos
::--------------------------------------------------------------------------

if "%1" == "todos" goto Todos
::--------------------------------------------------------------------------

if "%1" == "" goto Ayuda
::--------------------------------------------------------------------------
goto Otro
::--------------------------------------------------------------------------
:Ayuda
echo    lanzaRisk uno           lanza 'Risk uno'
echo    lanzaRisk dos           lanza 'Risk uno' y 15 segundos despues 'Risk dos'
echo    lanzaRisk todos         lanza Risk 4 veces con 15 segundos de intervalo:
echo                           'Risk uno', 'Risk dos', 'Risk tres' y 'Risk cuatro'
echo.
echo    lanzaRisk 'lo_que_sea'  lanza Risk 1 vez con el nombre dado en 'lo_que_sea'
echo.
goto Fin
::--------------------------------------------------------------------------
:Todos
start risk.exe uno
echo lanzado uno

timeout %tiempo% /nobreak

start risk.exe dos
echo lanzado dos

timeout %tiempo% /nobreak

start risk.exe tres
echo lanzado tres

timeout %tiempo% /nobreak

start risk.exe cuatro
echo lanzado cuatro

goto Fin
::--------------------------------------------------------------------------
:Otro
start risk.exe %1
echo lanzado %1

goto Fin
::--------------------------------------------------------------------------
:Uno
start risk.exe uno
echo lanzado uno

goto Fin
::--------------------------------------------------------------------------
:Dos
start risk.exe uno
echo lanzado uno

timeout %tiempo% /nobreak

start risk.exe dos
echo lanzado dos

goto Fin
::--------------------------------------------------------------------------

:Fin
echo ----------------------------------------------------------------------------
cd %mipwd%
::--------------------------------------------------------------------------

@echo off
@echo Deleting all BIN and OBJ and .VS folders...
for /d /r . %%d in (bin obj .vs) do @if exist "%%d" rd /s/q "%%d"
@echo BIN and OBJ and .VS folders successfully deleted :) Close the window.
pause > nul
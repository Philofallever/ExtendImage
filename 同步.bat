echo .meta> exclude.txt
xcopy /S /F /Y  /EXCLUDE:exclude.txt ExtendUI  D:\workspace\Proj_dpj\client-extendui
del exclude.txt
pause
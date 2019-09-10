echo .meta> exclude.txt
xcopy /S /F /Y  /EXCLUDE:exclude.txt ExtendUI  D:\workspace\Proj_dpj\unity-ugui-ext
del exclude.txt
pause
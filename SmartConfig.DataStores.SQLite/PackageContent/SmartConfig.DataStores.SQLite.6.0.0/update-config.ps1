.\sqlite3 config.db ".read config.sql"
Write-Host
if ($lastexitcode -eq 0)
{
    .\sqlite3 config.db "select * from setting;"
    Write-Host
    Write-Host "*** Configuration successfuly updated ;-)" -foregroundcolor "green"
}
else
{
    Write-Host "*** Error updating configuration ;-(" -foregroundcolor "red"
}
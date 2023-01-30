# Count number of lines of files in a folder and subs
dir -Recurse *.* | Get-Content | Measure-Object -Line
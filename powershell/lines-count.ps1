###
# Count number of lines of files in a folder and subs folders
###

dir -Recurse *.* | Get-Content | Measure-Object -Line
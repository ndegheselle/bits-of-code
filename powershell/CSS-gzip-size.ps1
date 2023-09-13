###
# Size comparison of files before and after gzip
###

$in = "c:\Users\user\Documents\"
$out = "$($in)\gziped"

Remove-Item "$($in)\gziped" -Recurse

# Minified files Length
get-childitem $in -File | 
    Sort-Object -Property Length | 
    select-object Name, @{name='Length (KB)';expression= {[math]::Round($_.Length/1KB, 2)}} |
    Format-Table -AutoSize

# Copy files to out folder
New-Item -ItemType Directory -Path $out | Out-Null
Get-ChildItem -Path $in -File | Copy-Item -Destination $out

# gzip all files
Get-ChildItem -Path $out -File | ForEach-Object {
  gzip $_.FullName
}

# Gzip files Length
get-childitem $out -File | 
    Sort-Object -Property Length | 
    select-object Name, @{name='Length (KB)';expression= {[math]::Round($_.Length/1KB, 2)}} |
    Format-Table -AutoSize
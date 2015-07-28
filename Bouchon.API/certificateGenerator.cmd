makecert.exe ^
-n "CN=DevelopmentCertificate" ^
-r ^
-pe ^
-a sha512 ^
-len 2048 ^
-cy authority ^
-sv DevelopmentCertificate.pvk ^
-sr CurrentUser ^
-ss Root ^
DevelopmentCertificate.cer
 
pvk2pfx.exe ^
-pvk DevelopmentCertificate.pvk ^
-spc DevelopmentCertificate.cer ^
-pfx DevelopmentCertificate.pfx ^
-po password
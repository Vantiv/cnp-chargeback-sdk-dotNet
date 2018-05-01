IF "%1"==[] (
	echo "Requires command line argument to be version to zip"
	exit /b
)

copy ChargebackSdkForNet\bin\Release\ChargebackSdkForNet.dll .\
"C:\Program Files\7-Zip\7z.exe" a ChargebackSdkForNet-%1.zip CHANGELOG LICENSE ChargebackSdkForNet.dll README.md 
del ChargebackSdkForNet.dll
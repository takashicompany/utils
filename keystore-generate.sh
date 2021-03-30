# .keystoreを生成しつつKey Hashも生成しちゃうシェルスクリプト

if [ $# -ne 3 ]; then
	echo "引数が不正です。以下のコマンドラインを参考にしてください。\n\n.keystoreの生成とKey Hashの出力:\n$ sh keystore-generate.sh generate [filename] [alias]\n\nKey Hashの出力のみ:\n$ sh keystore-generate.sh keyhash [filename] [alias]"
	exit 1
fi

filename=$2
alias=$3

if [ $1 = generate ]; then
	# .keystoreを生成する
	keytool -genkeypair -v -keystore ${filename}.keystore -alias ${alias} -keyalg RSA -keysize 2048 -validity 18250 -dname "O=Kayac.Inc, C=jp" -storetype PKCS12
fi

if [ $1 = generate ] || [ $1 = keyhash ]; then
	# .keystoreからKey Hashを生成する
	keytool -exportcert -alias ${alias} -keystore ${filename}.keystore | openssl sha1 -binary | openssl base64
else
	echo "第一引数が不正です。"
	exit 1
fi
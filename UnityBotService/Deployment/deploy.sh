# Read Secrets
ACR_PASSWORD=$1
TWITTER_APIKEY=$2
TWITTER_APISECRET=$3
TWITTER_ACCESSTOKEN=$4
TWITTER_ACCESSTOKENSECRET=$5
TWITTER_BEARERTOKEN=$6
RESOURCE_GROUP=$7

# create parameters json from template.
cp 'parameters.template.json' 'parameters.json'
sed -i "s/{ACR_PASSWORD}/${ACR_PASSWORD}/g" parameters.json

ENCODED_TWITTER_APIKEY=$(echo $TWITTER_APIKEY | base64)
ENCODED_TWITTER_APISECRET=$(echo $TWITTER_APISECRET | base64)
ENCODED_TWITTER_ACCESSTOKEN=$(echo $TWITTER_ACCESSTOKEN | base64)
ENCODED_TWITTER_ACCESSTOKENSECRET=$(echo $TWITTER_ACCESSTOKENSECRET | base64)
ENCODED_TWITTER_BEARERTOKEN=$(echo $TWITTER_BEARERTOKEN | base64)

cp 'template.template.json' 'template.json'
sed -i "s/{TWITTER_APIKEY}/${ENCODED_TWITTER_APIKEY}/g" template.json
sed -i "s/{TWITTER_APISECRET}/${ENCODED_TWITTER_APISECRET}/g" template.json
sed -i "s/{TWITTER_ACCESSTOKEN}/${ENCODED_TWITTER_ACCESSTOKEN}/g" template.json
sed -i "s/{TWITTER_ACCESSTOKENSECRET}/${ENCODED_TWITTER_ACCESSTOKENSECRET}/g" template.json
sed -i "s/{TWITTER_BEARERTOKEN}/${ENCODED_TWITTER_BEARERTOKEN}/g" template.json

# use azure CLI to trigger deployment.
az deployment group create -g $RESOURCE_GROUP -f template.json -p parameters.json
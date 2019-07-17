# vs-cqrs

## Instructions SQL Server

```shell
# Initialisation de la banque de données et insertion des données
cat debezium-sqlserver-init/inventory.sql | docker exec -i tutorial_sqlserver_1 bash -c '/opt/mssql-tools/bin/sqlcmd -U sa -P $SA_PASSWORD'

# Démarrage SQL Server connector
curl -i -X POST -H "Accept:application/json" -H  "Content-Type:application/json" http://localhost:8083/connectors/ -d @connecteur-sqlserver.json

# Consommation de données à partir de la topic Debezium
docker-compose -f docker-compose-sqlserver.yaml exec kafka /kafka/bin/kafka-console-consumer.sh \
    --bootstrap-server kafka:9092 \
    --from-beginning \
    --property print.key=true \
    --topic server1.dbo.customers

# Modification d'un enregistrement via SQL Server client
docker-compose -f docker-compose-sqlserver.yaml exec sqlserver bash -c '/opt/mssql-tools/bin/sqlcmd -U sa -P $SA_PASSWORD -d testDB'

```

## Avrogen
dotnet tool install -g Confluent.Apache.Avro.AvroGen

Répertoire où est installé les outils : $HOME/.dotnet/tools (Mac) ou %USERPROFILE%\.dotnet\tools (Windows)

ex.: avrogen -s LogMessage.V2.asvc .
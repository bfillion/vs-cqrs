# vs-cqrs

## Instructions SQL Server

```shell
# Initialisation de la banque de donn�es et insertion des donn�es
cat debezium-sqlserver-init/inventory.sql | docker exec -i tutorial_sqlserver_1 bash -c '/opt/mssql-tools/bin/sqlcmd -U sa -P $SA_PASSWORD'

# D�marrage SQL Server connector
curl -i -X POST -H "Accept:application/json" -H  "Content-Type:application/json" http://localhost:8083/connectors/ -d @connecteur-sqlserver.json

# Consommation de donn�es � partir de la topic Debezium
docker-compose -f docker-compose-sqlserver.yaml exec kafka /kafka/bin/kafka-console-consumer.sh \
    --bootstrap-server kafka:9092 \
    --from-beginning \
    --property print.key=true \
    --topic server1.dbo.customers

# Modification d'un enregistrement via SQL Server client
docker-compose -f docker-compose-sqlserver.yaml exec sqlserver bash -c '/opt/mssql-tools/bin/sqlcmd -U sa -P $SA_PASSWORD -d testDB'

```

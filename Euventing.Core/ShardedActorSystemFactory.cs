using System.Collections.Generic;
using Akka.Actor;
using Akka.Configuration;

namespace Eventing.Core
{
    public class ShardedActorSystemFactory : IActorSystemFactory
    {
        private readonly int portNumber;
        private readonly string akkaSystemName;
        private readonly string shardRepo;
        private readonly string[] seedNodes;

        public ShardedActorSystemFactory(int portNumber, string akkaSystemName, string shardRepo, params string[] seedNodes)
        {
            this.portNumber = portNumber;
            this.akkaSystemName = akkaSystemName;
            this.shardRepo = shardRepo;
            this.seedNodes = seedNodes;
        }

        public ActorSystem GetActorSystem()
        {
            string seedNodeString = GetSeedNodeList(akkaSystemName, seedNodes);
            var configString = MainConfig.
                Replace("{portNumber}", portNumber.ToString()).
                Replace("{persistencePlugin}", shardRepo).
                Replace("{seedNodes}", seedNodeString);
            var config = ConfigurationFactory.ParseString(configString);
            var system = ActorSystem.Create(akkaSystemName, config);

            return system;
        }

        private string GetSeedNodeList(string akkaSystemName, params string[] seedNodes)
        {
            List<string> fullSeedNodes = new List<string>();

            foreach (var seedNode in seedNodes)
            {
                fullSeedNodes.Add("\"akka.tcp://" + akkaSystemName + "@" + seedNode + "\"");
            }

            return string.Join(",", fullSeedNodes);
        }

        private string MainConfig = @"
            akka {
                #loglevel = DEBUG
                #log-config-on-start = on 
                actor.debug.unhandled = on
                loggers = [""Akka.Logger.NLog.NLogLogger, Akka.Logger.NLog""]
                akka.extensions = [""akka.contrib.pattern.DistributedPubSubExtension""]
                actor {
                  provider = ""Akka.Cluster.ClusterActorRefProvider, Akka.Cluster""
                  serializers {
                    akka-singleton = ""Akka.Cluster.Tools.Singleton.Serialization.ClusterSingletonMessageSerializer, Akka.Cluster.Tools""
                    wire = ""Akka.Serialization.WireSerializer, Akka.Serialization.Wire""               
                  }   
                  serialization-bindings {
                    ""Akka.Cluster.Tools.Singleton.ClusterSingletonMessage, Akka.Cluster.Tools"" = akka-singleton
                    ""System.Object"" = wire                 
                  }
                  serialization-identifiers {
                    ""Akka.Cluster.Tools.Singleton.Serialization.ClusterSingletonMessageSerializer, Akka.Cluster.Tools"" = 14
                  }
            debug {  
              receive = on 
              autoreceive = on
              lifecycle = on
              event-stream = on
              unhandled = on   }    
                }
                remote {
                  log-remote-lifecycle-events = DEBUG
                  log-received-messages = on
              
                  helios.tcp {
                    transport-class = ""Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote""
		                applied-adapters = []
		                transport-protocol = tcp
                    #will be populated with a dynamic host-name at runtime if left uncommented
                    #public-hostname = ""POPULATE STATIC IP HERE""
                    hostname = ""127.0.0.1""
                    port = {portNumber}
                  }
                }
                cluster {
                    seed-nodes = [{seedNodes}]
				    roles = [cluster]
                    coordinator-singleton = ""singleton""
                    singleton {
                      singleton-name = ""singleton""
                      role = ""cluster""
                      hand-over-retry-interval = 1s
                      min-number-of-hand-over-retries = 10
                    }
                    singleton-proxy {
                      # The actor name of the singleton actor that is started by the ClusterSingletonManager
                      singleton-name = ""singleton""
                      role = """"
                      singleton-identification-interval = 1s
                      buffer-size = 1000 
                    }
                    sharding {
                        guardian-name = sharding
                        coordinator-singleton = ""akka.cluster.singleton""
                        role = ""cluster""
                        remember-entities = off
                        coordinator-failure-backoff = 5s
                        retry-interval = 2s
                        buffer-size = 100000
                        handoff-timeout = 60s
                        shard-start-timeout = 10s
                        shard-failure-backoff = 10s
                        entity-restart-backoff = 10s
                        rebalance-interval = 10s
                        journal-plugin-id = ""akka.persistence.journal.{persistencePlugin}""
                        snapshot-plugin-id = ""akka.persistence.snapshot-store.{persistencePlugin}""
                        state-store-mode = ""persistence""
                        snapshot-after = 1000
                          least-shard-allocation-strategy {
                            rebalance-threshold = 10
                            max-simultaneous-rebalance = 3
                          }
                        }  
                     }
                    persistence {
                        journal {
                            plugin = ""akka.persistence.journal.{persistencePlugin}""
                            sqlite {
                                class = ""Akka.Persistence.Sqlite.Journal.SqliteJournal, Akka.Persistence.Sqlite""
                                plugin-dispatcher = ""akka.actor.default-dispatcher""
                                table-name = event_journal
                                auto-initialize = on
                                connection-string = ""Data Source=c:\\data\\store.db;Version=3;""
                            }
                            sql-server {
                                class = ""Akka.Persistence.SqlServer.Journal.SqlServerJournal, Akka.Persistence.SqlServer""
                                plugin-dispatcher = ""akka.actor.default-dispatcher""
                                table-name = eventjournal
                                auto-initialize = true
                                connection-string = ""Server=localhost\\SQLEXPRESS;Database=myDataBase;User Id=myUsername;Password=myPassword;""
                            }
                            local {
                                class = ""Akka.Persistence.Journal.MemoryJournal, Akka.Persistence""
                                plugin-dispatcher = ""akka.actor.default-dispatcher""
                            }
                        }
                        snapshot-store {
                            plugin = ""akka.persistence.snapshot-store.{persistencePlugin}""
                            sqlite {
                                class = ""Akka.Persistence.Sqlite.Snapshot.SqliteSnapshotStore, Akka.Persistence.Sqlite""
                                plugin-dispatcher = ""akka.actor.default-dispatcher""
                                table-name = snapshot_store
                                auto-initialize = on
                                connection-string = ""Data Source=c:\\data\\store1.db;Version=3;""
                            }
                            local {
                                class = ""Akka.Persistence.Snapshot.LocalSnapshotStore, Akka.Persistence""
                                plugin-dispatcher = ""akka.actor.default-dispatcher""
                            }
                            sql-server {
                                class = ""Akka.Persistence.SqlServer.Snapshot.SqlServerSnapshotStore, Akka.Persistence.SqlServer""
                                plugin-dispatcher = ""akka.actor.default-dispatcher""
                                table-name = snapshot_store
                                auto-initialize = true
                                connection-string = ""Server=localhost\\SQLEXPRESS;Database=myDataBase;User Id=myUsername;Password=myPassword;""
                            }
                        }
                    }
            }";
    }
}

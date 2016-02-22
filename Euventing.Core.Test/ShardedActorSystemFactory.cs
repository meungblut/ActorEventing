using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Akka.Persistence.Sqlite;
using Akka.Persistence;

namespace Euventing.Core.Test
{
    public class ShardedActorSystemFactory
    {
        public ActorSystem GetActorSystem(int portNumber, string akkaSystemName, params string[] seedNodes)
        {
            string seedNodeString = GetSeedNodeList(akkaSystemName, seedNodes);
            var config = ConfigurationFactory.ParseString(MainConfig.
                Replace("{portNumber}", portNumber.ToString()).
                Replace("{seedNodes}", seedNodeString));
            return ActorSystem.Create(akkaSystemName, config);
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
                loglevel = DEBUG
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
                        journal-plugin-id = ""akka.persistence.journal.sqlite""
                        snapshot-plugin-id = ""akka.persistence.snapshot-store.sqlite""
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
                            plugin = ""akka.persistence.journal.sqlite""
                            sqlite {
                                class = ""Akka.Persistence.Sqlite.Journal.SqliteJournal, Akka.Persistence.Sqlite""
                                plugin-dispatcher = ""akka.actor.default-dispatcher""
                                table-name = event_journal
                                auto-initialize = on
                                connection-string = ""Data Source=c:\\data\\store1.db;Version=3;""
                            }
                            inmem {
                                # Class name of the plugin.
                                class = ""Euventing.InMemoryPersistence.InMemoryJournal, Euventing.InMemoryPersistence""
                                # Dispatcher for the plugin actor.
                                plugin-dispatcher = ""akka.actor.default-dispatcher""
                            }
                        }
                        snapshot-store {
                            plugin = ""akka.persistence.snapshot-store.sqlite""
                            sqlite {
                                class = ""Akka.Persistence.Sqlite.Snapshot.SqliteSnapshotStore, Akka.Persistence.Sqlite""
                                plugin-dispatcher = ""akka.actor.default-dispatcher""
                                table-name = snapshot_store
                                auto-initialize = on
                                connection-string = ""Data Source=c:\\data\\store1.db;Version=3;""
                            }
                            inmem {
                                class = ""Euventing.InMemoryPersistence.InMemorySnapshotStore, Euventing.InMemoryPersistence""
                                # Dispatcher for the plugin actor.
                                plugin-dispatcher = ""akka.persistence.dispatchers.default-plugin-dispatcher""
                                # Dispatcher for streaming snapshot IO.
                                stream-dispatcher = ""akka.persistence.dispatchers.default-stream-dispatcher""
                            }
                        }
                    }
            }";
    }
}

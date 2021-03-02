using FluentMigrator;
using NzbDrone.Core.Datastore.Migration.Framework;

namespace NzbDrone.Core.Datastore.Migration
{
     [Migration(80)]
     public class add_justwatchflicksurls : NzbDroneMigrationBase
     {
          protected override void MainDbUpgrade()
          {
               if (!Schema.Schema("dbo").Table("Movies").Column("NetflixUrl").Exists())
               {
                    Alter.Table("Movies").AddColumn("NetflixUrl").AsString().Nullable();
               }

               if (!Schema.Schema("dbo").Table("Movies").Column("PrimeVideoUrl").Exists())
               {
                    Alter.Table("Movies").AddColumn("PrimeVideoUrl").AsString().Nullable();
               }

               if (!Schema.Schema("dbo").Table("Movies").Column("HooplaUrl").Exists())
               {
                    Alter.Table("Movies").AddColumn("HooplaUrl").AsString().Nullable();
               }

               if (!Schema.Schema("dbo").Table("Movies").Column("TubiTVUrl").Exists())
               {
                    Alter.Table("Movies").AddColumn("TubiTVUrl").AsString().Nullable();
               }

               if (!Schema.Schema("dbo").Table("Movies").Column("JustwatchUrl").Exists())
               {
                    Alter.Table("Movies").AddColumn("JustwatchUrl").AsString().Nullable();
               }
          }
     }
}

using Demo.Models;
using OpenOrm;
using OpenOrm.SQLite;
using OpenOrm.Migration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Sql.Migrations
{
    public class _001_init : OpenOrmMigration
    {
        public _001_init()
        {
            //Renseigner le numéro de version suivant (premier chiffre de la classe)
            Version = "1";
            Comment = "Initial database state";
        }

        //Mise à jour de la base de données
        public override void Up(OpenOrmDbConnection db)
        {
            //Init GlobalConfig
            if (!db.TableExists<GlobalConfig>())
            {
                db.CreateTable<GlobalConfig>();
            }
        }

        //Retour à la version précédente
        public override void Down(OpenOrmDbConnection db)
        {
            if (db.TableExists<GlobalConfig>())
            {
                db.DropTable<GlobalConfig>();
            }
            
        }
    }
}

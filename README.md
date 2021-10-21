# OpenOrm

OpenOrm is a light-weight, CodeFirst object-relational mapper (ORM) that aims to speed up .net develoment with a forever-free solution

Written in C#, netstandard2.0 
Can be used in projects : .NetCore, .NetFramework, Xamarin (android, ios), Mono (linux)

Supports : SqlServer (2008 -> 2019+), SQLite and MySql 
Plans to support : As needed, ask in issues !

## Features : 
- Easy to use Supports Lambda expressions to generate WHERE clause 
- Fully parameterized queries (strong vs sql injections) 
- Sql migrations (code first)

## Basic usage
Having a class named MyObjectClass as :
````cs
using OpenOrm;

namespace my_solution.Models
{
    public class MyObjectClass
    {
        [DbPrimaryKey, DbAutoIncrement]
        public long Id { get; set; }
        public string SomeStringProperty { get; set; }
        public DateTime CreationDate { get; set; }
        public bool BitColumn { get; set; }
    }
}
````


Add usings in your cs files as needed
```cs
using OpenOrm;

using OpenOrm.SqlServer;
// or using OpenOrm.SQLite;
// or using OpenOrm.MySql;
````

Using OpenOrm with my models :
```cs
using OpenOrm;
using OpenOrm.SqlServer;


using var db = new OpenOrmConfiguration 
{ 
    ConnectionString = @"my_connection_string", 
    Connector = Connector.SqlServer, 
    PutIdFieldAtFirstPosition = true, 
    Schema = "dbo" 
}.GetConnection();

//Create table based on class properties
if(!db.TableExists<MyObjectClass>())
{
    db.CreateTable<MyObjectClass>();
}

//Inserting first row
long inserted_id = db.Insert(new MyObjectClass { SomeStringProperty = "test" } );

//Base select, returns a list of MyObjectClass type 
var my_objects_list = db.Select<MyObjectClass>(); 
var first_object = db.SelectFirst<MyObjectClass>(x => x.Id == 1);

//Updates and Deletes are based on PrimaryKey
MyObjectClass o = db.SelectById(1); 
o.SomeStringProperty = "updated_field";
o.CreationDate = DateTime.Now;
db.Update(o);
db.Delete(o);
```

## Atttributes

You can use some attributes to personalize the result in database

````cs
    //Define a class than will be "linked" to a table named CORE_User 
    //instead of User (Naming table by class name is the default behaviour)
    [DbModel("CORE_User")] 
    public class User
    {
        //Define Id Column as primaryKey with auto increment
        [DbPrimaryKey, DbAutoIncrement]
        public long Id { get; set; }
        
        //Define RoleId as ForeignKey of Id Property in a Role named class
        [DbForeignKey(typeof(Role), "Id")]
        public long RoleId { get; set; }
        
        //Define Code column as Unique and not null in database
        [DbUnique, DbNotNull]
        public string Code { get; set; }
        
        //Define a property that will be ignored in table creation, selects and updates
        [DbIgnore]
        public List<Right> Rights { get; set; }
        
        //Will auto load from table UserConfig where UserConfig.UserId = [this object].Id
        //Set auto_load to false to save time if you have a lot of nested objects 
        //and only need User object when selecting User
        //You can then force loading nested objets when selecting : db.Select<User>(forceLoadNestedObjects: true);
        [DbLoadNestedList(child_type: typeof(UserConfig), child_foreign_key_property_name:  "UserId", parent_primary_key_property_name: "Id", auto_load: true)]
        public List<UserConfig> UserConfigs { get; set; }
        
        //Define a column as not null with false as default value
        //Then you can juste insert new object without having to set property value
        [DbNotNull("test")]
        public string TestString { get; set; }
        
        //Set default value for this column to 0
        //Will also update existing rows if column is added after initial table creation
        [DbDefault(0)]
        public int ClientScreenSize { get; set; }
        
        //Give another name to the column in database
        [DbColumnName("UserLastLogin")]
        public DateTime LastLogin { get; set; }
    }
````


## Database migrations
First, find a place where to create your migration files (ex ProjectRoot\Sql\Migrations\001_initial_migration.cs)

- Migration files are sorted using a "Natural Sort" on the Version property (see in the below exemple class contstructor)
(That means that 2 will come before 10 instead of 1, 10, 2, 3.....)

- To apply migrations, call db.Migrate when your program is initializing.
(ex: in Program.cs, Startup.cs, or in Form_Load event)

- Your migration files (.cs) does not have to be in the same project as the one calling db.Migrate()
- Migrations are inserted in the OpenOrmMigration table

To migrate your database, call :
````cs
using var db = [my code to get a connection];
db.Migrate();
````

To migrate your database to a specific version :
(Where "my_specific_version" is the Version property value)
````cs
using var db = [my code to get a connection];
db.Migrate("my_specific_version");
````
This will call the Down method of all superior versions except the version passed in parameter
Then you can call db.Migrate() again to call Up methods to the latest version.

Let the Version property empty to "disable" the migartion until the file is ready.

Migration file, basic exemple :

````cs
using OpenOrm;
using OpenOrm.SqlServer;
using OpenOrm.Migration;
using .....;

namespace MyGreatProject.Server.Sql.Migrations
{
    public class _002_adding_role_to_user : OpenOrmMigration
    {
        public _002_adding_role_to_user()
        {
            Version = "0.1.2";
            Comment = "Adding Role to user";
        }

        public override void Up(OpenOrmDbConnection db)
        {
            if (!db.TableExists<Role>())
            {
                db.CreateTable<Role>();

                db.Insert(new Role { Name = "Super Admin", Code = "ROLE_SUPER_ADMIN", IsDefault = false });
                db.Insert(new Role { Name = "Admin", Code = "ROLE_ADMIN", IsDefault = false });
                db.Insert(new Role { Name = "User", Code = "ROLE_USER", IsDefault = true });
            }
            
            //If public property exists in class and is not decorated with the attribute [DbIgnore]
            // select will fail as the column RoleId does not exists yet in database
            // Create it first before db.Select<User>();
            if(!db.ColumnExists<User>("RoleId"))
            {
                //Where RoleId is a public property of the User class
                db.AddColumn(typeof(User), "RoleId");
            }
            
            var users = db.Select<User>();
            var userRole = db.SelectFirst<Role>(x => x.Code == "ROLE_USER");
            foreach(var user in users)
            {
                user.RoleId = userRole.Id;
            }
            db.Update(users);
            
            ..........
        }

        public override void Down(OpenOrmDbConnection db)
        {
            if(db.ColumnExists<User>("RoleId"))
            {
                //Where RoleId is a public property of the User class
                db.DropColumn(typeof(User), "RoleId");
            }
        
            if (db.TableExists<Role>())
            {
                db.DropTable<Role>();
            }
        }
    }
}

````



## Complete documentation : 
(not yet)



Feel free to contribute (bug report, ideas, ...) !
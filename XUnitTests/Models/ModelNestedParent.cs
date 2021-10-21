using OpenOrm;
using System;
using System.Collections.Generic;
using System.Text;

namespace XUnitTests.Models
{
    public class ModelNestedParent : DbModel
    {
        [DbPrimaryKey, DbAutoIncrement]
        public long Id { get; set; }
        public string emptyProp { get; set; }

        //nested unique primitive
        public string code_lib { get; set; }
        //Get in Libelle, the value of Model20Props.str2 where Model20Props.str1 = this.code_lib
        [DbLoadNestedList(typeof(Model20Props), "str1", "code_lib", "str2")]
        public string Libelle { get; set; }

        //nested list primitive
        public string code_list_str { get; set; }
        [DbLoadNestedList(typeof(Model20Props), "str3", "code_list_str", "str4")]
        public List<string> str_list { get; set; }

        //nested unique object
        public long id_child { get; set; }
        [DbLoadNestedList(typeof(ModelNestedChild), "Id", "id_child")]
        public ModelNestedChild UniqueNestedTest { get; set; }

        //nested list object
        [DbLoadNestedList(typeof(ModelNestedChild), "ParentId", "Id", true)]
        public List<ModelNestedChild> NestedTest { get; set; }


        //nested, not auto loaded



        public ModelNestedParent()
        {
            NestedTest = new List<ModelNestedChild>();
        }
    }
}

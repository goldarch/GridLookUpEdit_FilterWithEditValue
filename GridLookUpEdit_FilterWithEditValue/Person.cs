using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GridLookUpEdit_FilterWithEditValue
{
    public class Person
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        //以下故意设置为字段，而不是属性！
        //字段，绑定时不报错，但也不显示值！
        public string Address;
    }

    public class SelectPerson
    {
        //属性！
        public int Id { get; set; }
        //绑定时，使用字段会报错
        //public int Id;
    }
}

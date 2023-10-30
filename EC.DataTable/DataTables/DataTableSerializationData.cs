using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace EC.DataTable.DataTables
{
    /// <summary>
    /// This class is used as helper object to serialize data
    /// to json with standard .net mechanism.
    /// </summary>
    [DataContract]
    internal class DataTableSerializationData
    {

        [DataMember(Name = "sEcho")]
        public string sEcho { get; set; }


        [DataMember(Name = "iTotalRecords")]
        public int iTotalRecords { get; set; }


        [DataMember(Name = "iTotalDisplayRecords")]
        public int iTotalDisplayRecords { get; set; }


        [DataMember(Name = "sColumns")]
        public string sColumns { get; set; }

        [DataMember(Name = "aaData")]
        public List<List<string>> aaData { get; set; }
    }
}

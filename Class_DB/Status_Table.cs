using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_DesktopDev_Antoine_Richard.Class_DB
{
    public class Status_Table
    {
        [Key]
        public int status_id { get; set; }

        public string status_name { get; set; }

    }
}

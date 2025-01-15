using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_DesktopDev_Antoine_Richard.Class_DB
{
    public class Game_Table
    {
        [Key]
        public int game_id { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public string genre { get; set; }

        public string plateforme { get; set; }

        public string annee { get; set; }

        public string image { get; set; }

        public Status_Table status { get; set; } = new Status_Table();  // Initialisation par défaut

    }
}

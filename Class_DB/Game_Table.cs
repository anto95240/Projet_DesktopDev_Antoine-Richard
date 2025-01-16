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

        public string Name { get; set; }

        public string Description { get; set; }

        public string Genre { get; set; }

        public string Plateforme { get; set; }

        public string Annee { get; set; }

        public string Image { get; set; }

        public Status_Table status { get; set; } = new Status_Table();

    }
}

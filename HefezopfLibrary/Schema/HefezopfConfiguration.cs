using Hefezopf.Fundament.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HefezopfLibrary.Schema
{
    public class HefezopfConfiguration
    {
        public readonly HZDBSchema Hefezopf;
        public readonly HZDBTable Hefezopf_ZombieState;

        public HefezopfConfiguration()
        {
            this.Hefezopf = new HZDBSchema() { Name = "Hefezopf" };
            this.Hefezopf_ZombieState = this.Hefezopf.NewDBTable("ZombieState");
            this.Hefezopf_ZombieState.AddColumn("Name");
        }
    }
}

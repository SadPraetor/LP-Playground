using Microsoft.EntityFrameworkCore;

namespace CrpDbCalls
{
    class CrpContext : DbContext
    {
        private readonly string _cnnString;

        public CrpContext(string cnnString) : base()
        {            
            _cnnString = cnnString;
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_cnnString);
            base.OnConfiguring(optionsBuilder);
        }


        public virtual DbSet<ChecklistItem> Checklist { get; set; }

        public virtual DbSet<Template> Templates { get; set; }
    }
}

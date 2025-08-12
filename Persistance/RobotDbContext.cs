
using robot_controller_api;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence
{
    public class RobotDbContext : DbContext
    {
        public RobotDbContext(DbContextOptions<RobotDbContext> options) : base(options) { }

        public ISet<UserModel> Users { get; set; }

        internal async Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }

    
}

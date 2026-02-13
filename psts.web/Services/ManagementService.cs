using Microsoft.EntityFrameworkCore;
using psts.web.Domain.Enums;
using psts.web.Dto;
using Psts.Web.Data;

namespace psts.web.Services
{
    public class ManagementService : IManagementService
    {

        private readonly PstsDbContext _db;

        public ManagementService(PstsDbContext db)
        {
            _db = db;
        }
    }
}

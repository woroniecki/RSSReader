using DataLayer.Models;
using DbAccess.Core;
using LogicLayer._GenericActions;
using System.Threading.Tasks;

namespace ServiceLayer._CQRS.PostQueries
{
    public class PostAndUserDataSelection
    {
        public Post Post { get; set; }
        public UserPostData UserPostData { get; set; }
    }
}

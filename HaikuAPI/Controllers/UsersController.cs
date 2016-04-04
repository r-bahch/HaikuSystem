using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using HaikuAPI.DAL;
using HaikuAPI.Models;
using HaikuAPI.DTO;
using Microsoft.Practices.Unity;
using System;
using Newtonsoft.Json.Linq;

namespace HaikuAPI.Controllers
{
    public class UsersController : ApiController
    {
        [Dependency]
        public EFDbContext db { get; set; }

        //GET api/users
        /// <summary>
        /// Returns a paged list of users
        /// </summary>
        /// <param name="skip">users to skip</param>
        /// <param name="take">users to take</param>
        /// <param name="sortby">sort order {name (default), name_desc, rating, rating_desc}</param>
        /// <returns></returns>
        public IHttpActionResult GetUsers([FromUri] int? skip = null, int? take = null, string sortby = "name")
        {
            //get all users
            var users = from u in db.Users
                        select new UserDTO()
                        {
                            Username = u.Username,
                            Haikus = u.Haikus.Select(h => new HaikuDTO() { ID = h.ID, Text = h.Text, Rating = h.Ratings.Count == 0 ? 0 : h.Ratings.Average(r => r.Value) })
                        };

            //sort
            switch (sortby)
            {
                case "rating_desc":
                    users = users.ToList().OrderByDescending(u => u.Rating).AsQueryable();
                    break;
                case "rating":
                    users = users.ToList().OrderBy(u => u.Rating).AsQueryable();
                    break;
                case "name_desc":
                    users = users.OrderByDescending(s => s.Username);
                    break;
                default:
                    users = users.OrderBy(s => s.Username);
                    break;
            }

            //page
            if (skip != null)
            {
                users = users.Skip((int)skip);
            }
            if (take != null)
            {
                users = users.Take((int)take);
            }
            return Ok(users);
        }

        // GET: api/Users/Username
        /// <summary>
        /// Gets details for specified user (username, rating, haikus)
        /// </summary>
        /// <param name="username">Username of the user</param>
        /// <returns></returns>
        [ResponseType(typeof(UserDTO))]
        public IHttpActionResult GetUser(string username)
        {
            //get user
            var users = from u in db.Users
                        where u.Username == username
                        select new UserDTO()
                        {
                            Username = u.Username,
                            Haikus = u.Haikus.Select(h => new HaikuDTO() { ID = h.ID, Text = h.Text, Rating = h.Ratings.Count == 0 ? 0 : h.Ratings.Average(r => r.Value) })
                        };
            UserDTO user = users.FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }


        // POST: api/Users
        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="user">User that is created</param>
        /// <returns></returns>
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> PostUser(UserCreateDTO user)
        {

            if (db.Users.Any(u => u.PublishCode == user.PublishCode))
            {
                ModelState.AddModelError("PublishCode", "Publish code already exist!");
            }
            if (db.Users.Any(u => u.Username == user.Username))
            {
                ModelState.AddModelError("Username", "Username already exist!");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Users.Add(new User() { Username = user.Username, PublishCode = user.PublishCode });
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { username = user.Username }, user);
        }

        //DELETE: api/users/username/haikus
        /// <summary>
        /// Delete all haikus from user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="publishCode"></param>
        /// <returns></returns>
        [HttpDelete]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> DeleteAll([FromUri] string username, [FromBody] UserCreateDTO publishCode)
        {
            User user;
            try
            {
                user = await db.Users.FirstAsync(u => u.Username == username);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }

            if (user.PublishCode != publishCode.PublishCode)
            {
                ModelState.AddModelError("PublishCode", String.Format("{0} correct:{1} sent:{2}", "Wrong publish code!", user.PublishCode, publishCode));
                return BadRequest(ModelState);
            }

            db.Haikus.RemoveRange(db.Haikus.Where(c => c.UserID == user.ID));
            await db.SaveChangesAsync();

            publishCode.Username = user.Username;
            return Ok(publishCode);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(int id)
        {
            return db.Users.Count(e => e.ID == id) > 0;
        }
    }
}
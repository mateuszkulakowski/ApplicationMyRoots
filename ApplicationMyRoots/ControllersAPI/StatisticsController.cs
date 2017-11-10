using ApplicationMyRoots.DAL;
using ApplicationMyRoots.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ApplicationMyRoots.ControllersAPI
{
    public class StatisticsController : ApiController
    {
        [HttpGet]
        public int GetCountTreeNodes()
        {
            try
            {
                using (var db = new DbContext())
                {
                    return db.UserTreeNodes.Count();
                }

            }catch(Exception e)
            {
                DbContext db = new DbContext();
                db.Errors.Add(new Error { DateThrow = DateTime.Now, Message = " GetCountTreeNodes() StatisticsControllerAPI - " + e.Message, StackTrace = e.StackTrace });
                db.SaveChanges();
            }

            return 0;
        }

        [HttpGet]
        public int GetCountTree()
        {
            try
            {
                using (var db = new DbContext())
                {
                    return db.UserTrees.Count();
                }
            }
            catch (Exception e)
            {
                DbContext db = new DbContext();
                db.Errors.Add(new Error { DateThrow = DateTime.Now, Message = " GetCountTree() StatisticsControllerAPI - " + e.Message, StackTrace = e.StackTrace });
                db.SaveChanges();
            }

            return 0;
        }

        [HttpGet]
        public int GetCountUser()
        {
            try
            {
                using (var db = new DbContext())
                {
                    return db.Users.Count();
                }
            }
            catch (Exception e)
            {
                DbContext db = new DbContext();
                db.Errors.Add(new Error { DateThrow = DateTime.Now, Message = " GetCountUser() StatisticsControllerAPI - " + e.Message, StackTrace = e.StackTrace });
                db.SaveChanges();
            }

            return 0;
        }


    }
}
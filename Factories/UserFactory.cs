using System.Collections.Generic;
using System.Linq;
using Dapper;
using System.Data;
using MySql.Data.MySqlClient;
using auctionboard.Models;


namespace auctionboard.Factory
{
    public class UserFactory : IFactory<User>
    {
        private string connectionString;

        public UserFactory()
        {
            connectionString = "server=localhost;userid=root;password=root;port=8889;database=auction;SslMode=None";
        }

        internal IDbConnection Connection
        {
            get {
                return new MySqlConnection(connectionString);
            }
        }

        public void Add(User item)
        {
            using (IDbConnection dbConnection = Connection) {
                string query =  "INSERT INTO users (username, first_name, last_name, password, money) VALUES (@username, @first_name, @last_name, @password, 1000.00)";
                dbConnection.Open();
                dbConnection.Execute(query, item);
            }
        }
        public void AddAuction(Auction item)
        {
            using (IDbConnection dbConnection = Connection) {
                string query =  "INSERT INTO products (name, description, bid, end_date, user_id, bidder_id) VALUES (@name, @description, @bid, @end_date, @user_id, @bidder_id)";
                dbConnection.Open();
                dbConnection.Execute(query, item);
            }
        }

        public IEnumerable<Auction> FindAllAuctions()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<Auction>("SELECT products.id, name, description, bid, end_date, first_name, user_id FROM products JOIN users ON users.id = products.user_id ORDER BY end_date");
            }
        }
        public Auction FindAuction(int id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<Auction>("SELECT products.id, name, description, bid, end_date, first_name, user_id, bidder_id FROM products JOIN users ON users.id = products.user_id WHERE products.id =" + id).FirstOrDefault();
            }
        }
        public Auction FindBidder(int id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<Auction>("SELECT first_name, last_name, bidder_id, money FROM products JOIN users ON users.id = products.bidder_id WHERE products.id =" + id).FirstOrDefault();
            }
        }
        public User FindMoney(int id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<User>("SELECT first_name, last_name, money from users where id = " + id).FirstOrDefault();
            }
        }
        public User FindByUsername(string username)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<User>("SELECT * FROM users WHERE username ='" + username + "'").FirstOrDefault();
            }
        }

        public User FindByID(int id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<User>("SELECT * FROM users WHERE id = @Id", new { Id = id }).FirstOrDefault();
            }
        } 

        public void RefundUser(int id , decimal money)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string query = "UPDATE users SET money = money + " + money + " WHERE id = " + id;
                dbConnection.Open();
                dbConnection.Execute(query);
            }
        } 
        public void ChargeUser(int id , decimal money)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string query = "UPDATE users SET money = money - " + money + " WHERE id = " + id;
                dbConnection.Open();
                dbConnection.Execute(query);
            }
        }
        public void UpdateAuction(int userid ,int id , decimal money)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string query = "UPDATE products SET bid =" + money + ", bidder_id = " + userid + " WHERE id = " + id;
                dbConnection.Open();
                dbConnection.Execute(query);
            }
        }

        public void DeleteAuction(int id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string query = "DELETE FROM products WHERE id = " + id;
                dbConnection.Open();
                dbConnection.Execute(query);
            }
        }

    }
}
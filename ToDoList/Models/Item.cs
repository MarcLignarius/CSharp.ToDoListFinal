using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace ToDoList.Models
{
    public class Item
    {
        private string _description;
        private DateTime _dueDate;
        private bool _completed;
        private int _id;

        public Item (string description, DateTime dueDate, bool completed = false, int id = 0)
        {
            _description = description;
            _dueDate = dueDate;
            _completed = completed;
            _id = id;
        }

        public string GetDescription()
        {
            return _description;
        }

        public void SetDescription(string newDescription)
        {
            _description = newDescription;
        }

        public string GetDueDate()
        {
            var dueDateToString = _dueDate.ToString("D");
            return dueDateToString;
        }

        public void SetDueDate(DateTime newDueDate)
        {
            _dueDate = newDueDate;
        }

        public bool GetCompleted()
        {
            return _completed;
        }

        public void SetCompleted(bool newCompleted)
        {
            _completed = newCompleted;
        }

        public int GetId()
        {
            return _id;
        }

        public static List<Item> GetAll()
        {
            List<Item> allItems = new List<Item> {};
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT * FROM items;";
            var rdr = cmd.ExecuteReader() as MySqlDataReader;
            while(rdr.Read())
            {
                int itemId = rdr.GetInt32(0);
                string itemDescription = rdr.GetString(1);
                DateTime itemDueDate = rdr.GetDateTime(2);
                bool itemCompleted = rdr.GetBoolean(3);
                Item newItem = new Item(itemDescription, itemDueDate, itemCompleted, itemId);
                allItems.Add(newItem);
            }
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
            return allItems;
        }

        public static void ClearAll()
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"DELETE FROM items;";
            cmd.ExecuteNonQuery();
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
        }

        public static Item Find(int id)
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT * FROM items WHERE id = (@searchId);";
            MySqlParameter searchId = new MySqlParameter();
            searchId.ParameterName = "@searchId";
            searchId.Value = id;
            cmd.Parameters.Add(searchId);
            var rdr = cmd.ExecuteReader() as MySqlDataReader;
            int itemId = 0;
            string itemName = "";
            DateTime itemDueDate = new DateTime(1999, 12, 24);
            bool itemCompleted = false;
            while(rdr.Read())
            {
                itemId = rdr.GetInt32(0);
                itemName = rdr.GetString(1);
                itemDueDate = rdr.GetDateTime(2);
                itemCompleted = rdr.GetBoolean(3);
            }
            Item newItem = new Item(itemName, itemDueDate, itemCompleted, itemId);
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
            return newItem;
        }

        public override bool Equals(System.Object otherItem)
        {
            if (!(otherItem is Item))
            {
                return false;
            }
            else
            {
                Item newItem = (Item) otherItem;
                bool idEquality = this.GetId() == newItem.GetId();
                bool descriptionEquality = this.GetDescription() == newItem.GetDescription();
                bool dueDateEquality = this.GetDueDate() == newItem.GetDueDate();
                bool completedEquality = this.GetCompleted() == newItem.GetCompleted();
                return (idEquality && descriptionEquality && dueDateEquality && completedEquality);
            }
        }

        public void Save()
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"INSERT INTO items (description, due_date, completed) VALUES (@description, @due_date, @completed);";
            MySqlParameter description = new MySqlParameter();
            description.ParameterName = "@description";
            description.Value = this._description;
            cmd.Parameters.Add(description);
            MySqlParameter dueDate = new MySqlParameter();
            dueDate.ParameterName = "@due_date";
            dueDate.Value = this._dueDate;
            cmd.Parameters.Add(dueDate);
            MySqlParameter completed = new MySqlParameter();
            completed.ParameterName = "@completed";
            completed.Value = this._completed;
            cmd.Parameters.Add(completed);
            cmd.ExecuteNonQuery();
            _id = (int) cmd.LastInsertedId;
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
        }

        public void Edit(string newDescription, DateTime newDueDate, bool newCompleted)
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"UPDATE items SET description = @new_description, due_date = @new_due_date, completed = @new_completed WHERE id = @search_id;";
            MySqlParameter searchId = new MySqlParameter();
            searchId.ParameterName = "@search_id";
            searchId.Value = _id;
            cmd.Parameters.Add(searchId);
            MySqlParameter description = new MySqlParameter();
            description.ParameterName = "@new_description";
            description.Value = newDescription;
            cmd.Parameters.Add(description);
            MySqlParameter dueDate = new MySqlParameter();
            dueDate.ParameterName = "@new_due_date";
            dueDate.Value = newDueDate;
            cmd.Parameters.Add(dueDate);
            MySqlParameter completed = new MySqlParameter();
            completed.ParameterName = "@new_completed";
            completed.Value = newCompleted;
            cmd.Parameters.Add(completed);
            cmd.ExecuteNonQuery();
            _description = newDescription;
            _dueDate = newDueDate;
            _completed = newCompleted;
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
        }

        public void Delete()
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"DELETE FROM items WHERE id = @item_id; DELETE FROM categories_items WHERE item_id = @item_id;";
            MySqlParameter itemIdParameter = new MySqlParameter();
            itemIdParameter.ParameterName = "@item_id";
            itemIdParameter.Value = this.GetId();
            cmd.Parameters.Add(itemIdParameter);
            cmd.ExecuteNonQuery();
            if (conn != null)
            {
              conn.Close();
            }
        }

        public List<Category> GetCategories()
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT categories.* FROM items
                JOIN categories_items ON (items.id = categories_items.item_id)
                JOIN categories ON (categories_items.category_id = categories.id)
                WHERE items.id = @item_id;";
            MySqlParameter itemIdParameter = new MySqlParameter();
            itemIdParameter.ParameterName = "@item_id";
            itemIdParameter.Value = _id;
            cmd.Parameters.Add(itemIdParameter);
            var rdr = cmd.ExecuteReader() as MySqlDataReader;
            List<Category> categories = new List<Category> {};
            while(rdr.Read())
            {
                int thisCategoryId = rdr.GetInt32(0);
                string categoryName = rdr.GetString(1);
                Category foundCategory = new Category(categoryName, thisCategoryId);
                categories.Add(foundCategory);
            }
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
            return categories;
        }

        public void AddCategory(Category newCategory)
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"INSERT INTO categories_items (category_id, item_id) VALUES (@category_id, @item_id);";
            MySqlParameter category_id = new MySqlParameter();
            category_id.ParameterName = "@category_id";
            category_id.Value = newCategory.GetId();
            cmd.Parameters.Add(category_id);
            MySqlParameter item_id = new MySqlParameter();
            item_id.ParameterName = "@item_id";
            item_id.Value = _id;
            cmd.Parameters.Add(item_id);
            cmd.ExecuteNonQuery();
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
        }
    }
}

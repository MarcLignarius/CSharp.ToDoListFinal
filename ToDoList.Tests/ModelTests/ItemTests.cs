using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToDoList.Models;
using System.Collections.Generic;
using System;

namespace ToDoList.Tests
{
    [TestClass]
    public class ItemTest : IDisposable
    {

        public void Dispose()
        {
          Item.ClearAll();
          Category.ClearAll();
        }

        public ItemTest()
        {
            DBConfiguration.ConnectionString = "server=localhost;user id=root;password=root;port=8889;database=to_do_list_test;";
        }

        [TestMethod]
        public void ItemConstructor_CreatesInstanceOfItem_Item()
        {
            DateTime dueDate =  new DateTime(1999, 12, 24);
            Item newItem = new Item("test", dueDate);
            Assert.AreEqual(typeof(Item), newItem.GetType());
        }

        [TestMethod]
        public void GetDescription_ReturnsDescription_String()
        {
            //Arrange
            DateTime dueDate =  new DateTime(1999, 12, 24);
            string description = "Walk the dog.";
            Item newItem = new Item("test", dueDate);

            //Act
            string result = newItem.GetDescription();

            //Assert
            Assert.AreEqual(description, result);
        }

        [TestMethod]
        public void SetDescription_SetDescription_String()
        {
            //Arrange
            DateTime dueDate =  new DateTime(1999, 12, 24);
            string description = "Walk the dog.";
            Item newItem = new Item("test", dueDate);

            //Act
            string updatedDescription = "Do the dishes";
            newItem.SetDescription(updatedDescription);
            string result = newItem.GetDescription();

            //Assert
            Assert.AreEqual(updatedDescription, result);
        }

        [TestMethod]
        public void GetCompleted_ReturnsCompleted_Boolean()
        {
            //Arrange
            DateTime dueDate =  new DateTime(1999, 12, 24);
            bool completed = false;
            Item newItem = new Item("test", dueDate, completed);

            //Act
            bool result = newItem.GetCompleted();

            //Assert
            Assert.AreEqual(completed, result);
        }

        [TestMethod]
        public void SetCompleted_SetCompleted_Boolean()
        {
            //Arrange
            DateTime dueDate =  new DateTime(1999, 12, 24);
            Item newItem = new Item("test", dueDate);

            //Act
            bool updatedCompleted = true;
            newItem.SetCompleted(updatedCompleted);
            bool result = newItem.GetCompleted();

            //Assert
            Assert.AreEqual(updatedCompleted, result);
        }

        [TestMethod]
        public void GetAll_ReturnsEmptyList_ItemList()
        {
            //Arrange
            List<Item> newList = new List<Item> { };

            //Act
            List<Item> result = Item.GetAll();

            //Assert
            CollectionAssert.AreEqual(newList, result);
        }

        [TestMethod]
        public void GetAll_ReturnsItems_ItemList()
        {
            //Arrange
            DateTime dueDate =  new DateTime(1999, 12, 24);
            string description01 = "Walk the dog";
            string description02 = "Wash the dishes";
            Item newItem1 = new Item(description01, dueDate);
            newItem1.Save();
            Item newItem2 = new Item(description02, dueDate);
            newItem2.Save();
            List<Item> newList = new List<Item> { newItem1, newItem2 };

            //Act
            List<Item> result = Item.GetAll();

            //Assert
            CollectionAssert.AreEqual(newList, result);
        }

        [TestMethod]
        public void Find_ReturnsCorrectItemFromDatabase_Item()
        {
            //Arrange
            DateTime dueDate =  new DateTime(1999, 12, 24);
            Item testItem = new Item("Mow the lawn", dueDate);
            testItem.Save();

            //Act
            Item foundItem = Item.Find(testItem.GetId());

            //Assert
            Assert.AreEqual(testItem, foundItem);
        }

        [TestMethod]
        public void Equals_ReturnsTrueIfDescriptionsAreTheSame_Item()
        {
            // Arrange, Act
            DateTime dueDate =  new DateTime(1999, 12, 24);
            Item firstItem = new Item("Mow the lawn", dueDate);
            Item secondItem = new Item("Mow the lawn", dueDate);

            // Assert
            Assert.AreEqual(firstItem, secondItem);
        }

        [TestMethod]
        public void Save_SavesToDatabase_ItemList()
        {
            //Arrange
            DateTime dueDate =  new DateTime(1999, 12, 24);
            Item testItem = new Item("Mow the lawn", dueDate);

            //Act
            testItem.Save();
            List<Item> result = Item.GetAll();
            List<Item> testList = new List<Item>{testItem};

            //Assert
            CollectionAssert.AreEqual(testList, result);
        }

        [TestMethod]
        public void Save_AssignsIdToObject_Id()
        {
            //Arrange
            DateTime dueDate =  new DateTime(1999, 12, 24);
            Item testItem = new Item("Mow the lawn", dueDate);

            //Act
            testItem.Save();
            Item savedItem = Item.GetAll()[0];

            int result = savedItem.GetId();
            int testId = testItem.GetId();

            //Assert
            Assert.AreEqual(testId, result);
        }

        [TestMethod]
        public void Edit_UpdatesItemInDatabase_String()
        {
            //Arrange
            DateTime dueDate =  new DateTime(1999, 12, 24);
            Item testItem = new Item("Walk the Dog", dueDate);
            testItem.Save();
            string secondDescription = "Mow the lawn";

            //Act
            testItem.Edit(secondDescription);
            string result = Item.Find(testItem.GetId()).GetDescription();

            //Assert
            Assert.AreEqual(secondDescription, result);
        }

        [TestMethod]
        public void GetCategories_ReturnsAllItemCategories_CategoryList()
        {
            //Arrange
            DateTime dueDate =  new DateTime(1999, 12, 24);
            Item testItem = new Item("Mow the lawn", dueDate);
            testItem.Save();
            Category testCategory1 = new Category("Home stuff");
            testCategory1.Save();
            Category testCategory2 = new Category("Work stuff");
            testCategory2.Save();

            //Act
            testItem.AddCategory(testCategory1);
            List<Category> result = testItem.GetCategories();
            List<Category> testList = new List<Category> {testCategory1};

            //Assert
            CollectionAssert.AreEqual(testList, result);
        }

        [TestMethod]
        public void AddCategory_AddsCategoryToItem_CategoryList()
        {
            //Arrange
            DateTime dueDate =  new DateTime(1999, 12, 24);
            Item testItem = new Item("Mow the lawn", dueDate);
            testItem.Save();
            Category testCategory = new Category("Home stuff");
            testCategory.Save();

            //Act
            testItem.AddCategory(testCategory);

            List<Category> result = testItem.GetCategories();
            List<Category> testList = new List<Category>{testCategory};

            //Assert
            CollectionAssert.AreEqual(testList, result);
        }

        [TestMethod]
        public void Delete_DeletesItemAssociationsFromDatabase_ItemList()
        {
            //Arrange
            DateTime dueDate =  new DateTime(1999, 12, 24);
            Category testCategory = new Category("Home stuff");
            testCategory.Save();
            string testDescription = "Mow the lawn";
            Item testItem = new Item(testDescription, dueDate);
            testItem.Save();

            //Act
            testItem.AddCategory(testCategory);
            testItem.Delete();
            List<Item> resultCategoryItems = testCategory.GetItems();
            List<Item> testCategoryItems = new List<Item> {};

            //Assert
            CollectionAssert.AreEqual(testCategoryItems, resultCategoryItems);
        }
    }
}

// using Microsoft.VisualStudio.TestTools.UnitTesting;
// using ToDoList.Models;
// using System.Collections.Generic;
// using System;
//
// namespace ToDoList.Tests
// {
//     [TestClass]
//     public class ItemTest : IDisposable
//     {
//         public void Dispose()
//         {
//             Item.ClearAll();
//         }
//
//         public ItemTest()
//         {
//             DBConfiguration.ConnectionString = "server=localhost;user id=root;password=root;port=8889;database=to_do_list_test;";
//         }
//
//         [TestMethod]
//         public void ItemConstructor_CreatesInstanceOfItem_Item()
//         {
//             //Arrange
//             DateTime itemDueDate =  new DateTime(1999, 12, 24);
//             Item newItem = new Item("test", itemDueDate, 1);
//
//             //Assert
//             Assert.AreEqual(typeof(Item), newItem.GetType());
//         }
//
//         [TestMethod]
//         public void GetDescription_ReturnsDescription_String()
//         {
//             //Arrange
//             string description = "Walk the dog.";
//             DateTime itemDueDate =  new DateTime(1999, 12, 24);
//             Item newItem = new Item(description, itemDueDate, 1);
//
//             //Act
//             string result = newItem.GetDescription();
//
//             //Assert
//             Assert.AreEqual(description, result);
//         }
//
//         [TestMethod]
//         public void SetDescription_SetDescription_String()
//         {
//             //Arrange
//             string description = "Walk the dog.";
//             DateTime itemDueDate =  new DateTime(1999, 12, 24);
//             Item newItem = new Item(description, itemDueDate, 1);
//
//             //Act
//             string updatedDescription = "Do the dishes";
//             newItem.SetDescription(updatedDescription);
//             string result = newItem.GetDescription();
//
//             //Assert
//             Assert.AreEqual(updatedDescription, result);
//         }
//
//         [TestMethod]
//         public void GetAll_ReturnsEmptyListFromDatabase_ItemList()
//         {
//             //Arrange
//             List<Item> newList = new List<Item> { };
//
//             //Act
//             List<Item> result = Item.GetAll();
//
//             //Assert
//             CollectionAssert.AreEqual(newList, result);
//         }
//
//         [TestMethod]
//         public void GetAll_ReturnsItems_ItemList()
//         {
//             //Arrange
//             string description01 = "Walk the dog";
//             string description02 = "Wash the dishes";
//             DateTime itemDueDate =  new DateTime(1999, 12, 24);
//             Item newItem1 = new Item(description01, itemDueDate, 1);
//             newItem1.Save();
//             Item newItem2 = new Item(description02, itemDueDate, 1);
//             newItem2.Save();
//             List<Item> newList = new List<Item> { newItem1, newItem2 };
//
//             //Act
//             List<Item> result = Item.GetAll();
//
//             //Assert
//             CollectionAssert.AreEqual(newList, result);
//         }
//
//         [TestMethod]
//         public void Save_AssignsIdToObject_Id()
//         {
//             //Arrange
//             DateTime itemDueDate =  new DateTime(1999, 12, 24);
//             Item testItem = new Item("Mow the lawn", itemDueDate, 1);
//
//             //Act
//             testItem.Save();
//             Item savedItem = Item.GetAll()[0];
//
//             int result = savedItem.GetId();
//             int testId = testItem.GetId();
//
//             //Assert
//             Assert.AreEqual(testId, result);
//         }
//
//         /*[TestMethod]
//         public void GetId_ItemsInstantiateWithAnIdAndGetterReturns_Int()
//         {
//             //Arrange
//             string description = "Walk the dog.";
//             Item newItem = new Item(description, 1);
//
//             //Act
//             int result = newItem.GetId();
//
//             //Assert
//             Assert.AreEqual(1, result);
//         }*/
//
//         [TestMethod]
//         public void Find_ReturnsCorrectItemFromDatabase_Item()
//         {
//             //Arrange
//             DateTime itemDueDate =  new DateTime(1999, 12, 24);
//             Item testItem = new Item("Mow the lawn", itemDueDate, 1);
//             testItem.Save();
//
//             //Act
//             Item foundItem = Item.Find(testItem.GetId());
//
//             //Assert
//             Assert.AreEqual(testItem, foundItem);
//         }
//
//         [TestMethod]
//         public void Equals_ReturnsTrueIfDescriptionsAreTheSame_Item()
//         {
//             // Arrange, Act
//             DateTime itemDueDate =  new DateTime(1999, 12, 24);
//             Item firstItem = new Item("Mow the lawn", itemDueDate, 1);
//             Item secondItem = new Item("Mow the lawn", itemDueDate, 1);
//
//             // Assert
//             Assert.AreEqual(firstItem, secondItem);
//         }
//
//         [TestMethod]
//         public void Save_SavesToDatabase_ItemList()
//         {
//             //Arrange
//             DateTime itemDueDate =  new DateTime(1999, 12, 24);
//             Item testItem = new Item("Mow the lawn", itemDueDate, 1);
//
//             //Act
//             testItem.Save();
//             List<Item> result = Item.GetAll();
//             List<Item> testList = new List<Item>{testItem};
//
//             //Assert
//             CollectionAssert.AreEqual(testList, result);
//         }
//
//         [TestMethod]
//         public void Edit_UpdatesItemInDatabase_String()
//         {
//             //Arrange
//             DateTime itemDueDate =  new DateTime(1999, 12, 24);
//             string firstDescription = "Walk the Dog";
//             Item testItem = new Item(firstDescription, itemDueDate, 1);
//             testItem.Save();
//             string secondDescription = "Mow the lawn";
//
//             //Act
//             testItem.Edit(secondDescription);
//             string result = Item.Find(testItem.GetId()).GetDescription();
//
//             //Assert
//             Assert.AreEqual(secondDescription, result);
//         }
//
//         [TestMethod]
//         public void Delete_DeletesItemFromDatabase_List()
//         {
//             //Arrange
//             DateTime itemDueDate =  new DateTime(1999, 12, 24);
//             Category newCategory = new Category("Home Tasks");
//             Item newItem1 = new Item("Walk the dog.", itemDueDate, newCategory.GetId());
//             Item newItem2 = new Item("Clean the room", itemDueDate, newCategory.GetId());
//             newItem1.Save();
//             newItem2.Save();
//             //Act
//             newItem1.Delete();
//             List<Item> result = Item.GetAll();
//             List<Item> newList = new List<Item> { newItem2 };
//             //Assert
//             CollectionAssert.AreEqual(newList, result);
//         }
//
//         [TestMethod]
//       public void GetCategories_ReturnsAllItemCategories_CategoryList()
//       {
//           //Arrange
//           DateTime itemDueDate =  new DateTime(1999, 12, 24);
//           Item testItem = new Item("Mow the lawn", itemDueDate);
//           testItem.Save();
//           Category testCategory1 = new Category("Home stuff");
//           testCategory1.Save();
//           Category testCategory2 = new Category("Work stuff");
//           testCategory2.Save();
//
//           //Act
//           testItem.AddCategory(testCategory1);
//           List<Category> result = testItem.GetCategories();
//           List<Category> testList = new List<Category> {testCategory1};
//
//           //Assert
//           CollectionAssert.AreEqual(testList, result);
//       }
//
//
//
//         [TestMethod]
//         public void Delete_DeletesItemAssociationsFromDatabase_ItemList()
//         {
//             //Arrange
//             DateTime itemDueDate =  new DateTime(1999, 12, 24);
//             Category testCategory = new Category("Home stuff");
//             testCategory.Save();
//             string testDescription = "Mow the lawn";
//             Item testItem = new Item(testDescription, itemDueDate);
//             testItem.Save();
//
//             //Act
//             testItem.AddCategory(testCategory);
//             testItem.Delete();
//             List<Item> resultCategoryItems = testCategory.GetItems();
//             List<Item> testCategoryItems = new List<Item> {};
//
//             //Assert
//             CollectionAssert.AreEqual(testCategoryItems, resultCategoryItems);
//         }
//     }
// }

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {
        static HumaneSocietyDataContext db = new HumaneSocietyDataContext();
        internal static List<USState> GetStates()
        {
             HumaneSocietyDataContext  db = new HumaneSocietyDataContext();

            List<USState> allStates = db.USStates.ToList();

            return allStates;
        }

        internal static Client GetClient(string userName, string password)
        {
             HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).FirstOrDefault();

            return client;
        }

        internal static List<Client> GetClients()
        {
             HumaneSocietyDataContext  db = new HumaneSocietyDataContext();

            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
             HumaneSocietyDataContext  db = new HumaneSocietyDataContext();

            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.AddressLine2 = null;
                newAddress.Zipcode = zipCode;
                newAddress.USStateId = stateId;

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

       

        internal static void UpdateClient(Client clientWithUpdates)
        {
             HumaneSocietyDataContext  db = new HumaneSocietyDataContext();

            // find corresponding Client from Db
            Client clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();

            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.AddressLine2 = null;
                newAddress.Zipcode = clientAddress.Zipcode;
                newAddress.USStateId = clientAddress.USStateId;

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }

        internal static Room GetRoom(int animalId)// UI 
        {
            throw new NotImplementedException();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
             HumaneSocietyDataContext  db = new HumaneSocietyDataContext();

            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if(employeeFromDb == null)
            {
                throw new NullReferenceException();            
            }
            else
            {
                return employeeFromDb;
            }            
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
             HumaneSocietyDataContext  db = new HumaneSocietyDataContext();

            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
             HumaneSocietyDataContext  db = new HumaneSocietyDataContext();

            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName == null;
        }

     
        internal static void AddUsernameAndPassword(Employee employee)
        {
             HumaneSocietyDataContext  db = new HumaneSocietyDataContext();

            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

      
        //action will be one of four things(create, read, update or delete) switch/default statement
        internal static void RunEmployeeQueries(Employee employee, string action)//admin
        {

        }

        internal static Animal GetAnimalByID(int iD)//customer
        {
            throw new NotImplementedException();

        }

        internal static void Adopt(Animal animal, Client client)//customer
        {
            throw new NotImplementedException();
        }

        internal static IQueryable<Animal> SearchForAnimalByMultipleTraits()
        {
            var animals = db.Animals.Where(s => s.Gender == "Male").Select(p => p);
            return animals;
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()//meg
        {
            var requiredData = db.Adoptions.Where(a => a.ApprovalStatus == "Pending");
            return requiredData;
        }

        internal static void UpdateAdoption(bool x, Adoption adoption)// complete -M
        {
           
            var requiredData =
                (from y in db.Adoptions
                 where y.AdoptionId == adoption.AdoptionId
                 select y).First();
            var animal =
                (from z in db.Animals
                 where z.AnimalId == adoption.AnimalId
                 select z).First();
            if (x)
            {
                requiredData.ApprovalStatus = "Approved";
                animal.AdoptionStatus = "Approved";         
            }
            else
            {
                requiredData.ApprovalStatus = "Denied";
                animal.AdoptionStatus = "Pending";
            }

            db.SubmitChanges();
              

        }

        internal static void EnterAnimalUpdate(Animal animal, Dictionary<int, string> updates)//write
        {
            throw new NotImplementedException();
        }

        internal static IQueryable<AnimalShot> GetShots(Animal animal)//complete -M
        {
            var requiredData =
                from x in db.AnimalShots
                where x.AnimalId == animal.AnimalId
                select x;

            return requiredData;            
        }

        internal static void UpdateShot(string newShot, Animal animal)//complete -M
        {
            
            var requiredData =
                (from x in db.AnimalShots
                 where x.Name == newShot
                 select x).First();

            AnimalShot newAnimalShot = new AnimalShot();
            newAnimalShot.AnimalId = animal.AnimalId;
            newAnimalShot.ShotId = requiredData.ShotId;
            db.AnimalShots.InsertOnSubmit(newAnimalShot);
            db.SubmitChanges();
        }

        internal static void RemoveAnimal(Animal animal)//complete -M
        {
            var requiredData =
                (from x in db.Animals
                 where x.AnimalId == animal.AnimalId
                 select x).First();
            
            if(requiredData != null)
            {
                db.Animals.DeleteOnSubmit(requiredData);
                db.SubmitChanges();
            }
        }

        internal static int? GetCategoryId()//write
        {
            throw new NotImplementedException();
        }

        internal static int? GetDietPlanId()//write
        {
            throw new NotImplementedException();
        }

        internal static void AddAnimal(Animal animal)//write
        {
            throw new NotImplementedException();
        }
    }
}
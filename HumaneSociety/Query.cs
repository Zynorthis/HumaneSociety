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

            List<USState> allStates = db.USStates.ToList();

            return allStates;
        }

        internal static Client GetClient(string userName, string password)
        {

            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).SingleOrDefault();

            return client;
        }

        internal static List<Client> GetClients()
        {

            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {

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
            if (updatedAddress == null)
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

        internal static Room GetRoom(int animalId)
        {
            Room result = new Room();
            result = db.Rooms.Where(r => r.AnimalId == animalId).FirstOrDefault();
            return result;
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {

            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
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

            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {

            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName == null;
        }

        internal static void AddUsernameAndPassword(Employee employee)
        {

            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }


        // action will be one of four things (create, read, update, or delete) - Jacob
        internal static void RunEmployeeQueries(Employee employee, string action)//admin 
        {

        }
        internal static Animal GetAnimalByID(int iD)//customer
        {
            Animal result = new Animal();
            result = db.Animals.Where(a => a.AnimalId == iD).FirstOrDefault();
            return result;
        }

        internal static void Adopt(Animal animal, Client client)//customer
        {
            Adoption newAdoption = new Adoption();
            newAdoption.ClientId = client.ClientId;
            newAdoption.AnimalId = animal.AnimalId;
            newAdoption.ApprovalStatus = "Pending";
            newAdoption.AdoptionFee = 75;
            newAdoption.PaymentCollected = false;

            db.Adoptions.InsertOnSubmit(newAdoption);
            db.SubmitChanges();
        }

        internal static List<Animal> SearchForAnimalByMultipleTraits()
        {
            bool keepSearching = true;
            var animals = db.Animals.ToList();
            while (keepSearching == true)
            {
                List<string> options = new List<string>() {
                    "What trait would you like to search for?",
                    "----------------------------------------",
                    "1. Name",
                    "2. Category",
                    "3. Weight",
                    "4. Age",
                    "5. Diet",
                    "6. Demeanor",
                    "7. Kid Friendliness",
                    "8. Pet Friendliness",
                    "9. Gender"
                };
                UserInterface.DisplayUserOptions(options);
                var userInput = UserInterface.GetIntegerData();
                switch (userInput)
                {
                    case 1:
                        UserInterface.DisplayUserOptions("Enter a Name to search by:");
                        string searchName = UserInterface.GetUserInput();
                        animals = animals.Where(n => n.Name == searchName).Select(n => n).ToList();
                        break;
                    case 2:
                        UserInterface.DisplayUserOptions("Enter a Category to search by:");
                        string searchCategory = UserInterface.GetUserInput();
                        animals = animals.Where(c => c.Category.Name == searchCategory).Select(c => c).ToList();
                        break;
                    case 3:
                        UserInterface.DisplayUserOptions("Enter a Weight to search by:");
                        int searchWeight = UserInterface.GetIntegerData();
                        animals = animals.Where(n => n.Weight == searchWeight).Select(n => n).ToList();
                        break;
                    case 4:
                        UserInterface.DisplayUserOptions("Enter a Age to search by:");
                        int searchAge = UserInterface.GetIntegerData();
                        animals = animals.Where(n => n.Age == searchAge).Select(n => n).ToList();
                        break;
                    case 5:
                        UserInterface.DisplayUserOptions("Enter the name of the Diet to search by:");
                        string searchDiet = UserInterface.GetUserInput();
                        animals = animals.Where(n => n.DietPlan.Name == searchDiet).Select(n => n).ToList();
                        break;
                    case 6:
                        UserInterface.DisplayUserOptions("Enter a animal Demeanor to search by:");
                        string searchDemeanor = UserInterface.GetUserInput();
                        animals = animals.Where(n => n.Demeanor == searchDemeanor).Select(n => n).ToList();
                        break;
                    case 7:
                        bool? searchKidFriendliness = UserInterface.GetBitData("the animal ", "Kid Freindly");
                        animals = animals.Where(n => n.KidFriendly == searchKidFriendliness).Select(n => n).ToList();
                        break;
                    case 8:
                        bool? searchPetFriendliness = UserInterface.GetBitData("the animal ", "Pet Freindly");
                        animals = animals.Where(n => n.PetFriendly == searchPetFriendliness).Select(n => n).ToList();
                        break;
                    case 9:
                        UserInterface.DisplayUserOptions("Enter a Gender to search by:");
                        string searchGender = UserInterface.GetUserInput();
                        animals = animals.Where(n => n.Gender == searchGender.ToLower()).Select(n => n).ToList();
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error: not a valid option, please try again.");
                        Console.ResetColor();
                        Console.ReadKey();
                        break;
                }
                Console.WriteLine("Would you like to enter in another search criteria?");
                string answer = UserInterface.GetUserInput();
                if (answer.ToLower() == "yes" || answer.ToLower() == "y")
                {
                    keepSearching = true;
                }
                else if (answer.ToLower() == "no" || answer.ToLower() == "n")
                {
                    keepSearching = false;
                }
                else
                {
                    Console.Clear();
                    UserInterface.DisplayUserOptions("Input not recognized please try again");
                }
            }
            return animals;
        }
        internal static IQueryable<Adoption> GetPendingAdoptions()//M
        {
            var requiredData =
                from x in db.Adoptions
                where x.ApprovalStatus == "Pending"
                select x;
            return requiredData;

        }
        internal static void UpdateAdoption(bool x, Adoption adoption)//M
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
        internal static IQueryable<AnimalShot> GetShots(Animal animal)//M
        {
            var requiredData =
                from x in db.AnimalShots
                where x.AnimalId == animal.AnimalId
                select x;

            return requiredData;
        }

        internal static void EnterAnimalUpdate(Animal animal, Dictionary<int, string> updates)//write
        {
            throw new NotImplementedException();
        }

        internal static void UpdateShot(string newShot, Animal animal)//M
        {
            var requiredData =
                (from x in db.Shots
                 where x.Name == newShot
                 select x).First();

            AnimalShot newAnimalShot = new AnimalShot();
            newAnimalShot.AnimalId = animal.AnimalId;
            newAnimalShot.ShotId = requiredData.ShotId;
            db.AnimalShots.InsertOnSubmit(newAnimalShot);
            db.SubmitChanges();

        }

        internal static void RemoveAnimal(Animal animal)//M
        {
            var requiredData =
                (from x in db.Animals
                 where x.AnimalId == animal.AnimalId
                 select x).First();

            if (requiredData != null)
            {
                db.Animals.DeleteOnSubmit(requiredData);
                db.SubmitChanges();
            }
        }

        internal static void AddAnimal(Animal animal)//M
        {
            db.Animals.InsertOnSubmit(animal);
            db.SubmitChanges();         
        }

     

        internal static int? GetDietPlanId()
        {
            throw new NotImplementedException();
        }

        internal static int? GetCategoryId()//M
        {
          
        }
    }                
}





       
  
